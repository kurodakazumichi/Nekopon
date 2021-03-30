using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Particle;

namespace MyGame.Unit.Effect
{
  public class Leaves : EffectBase<Leaves.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Spiral,
      Burst,
      Vanish,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 葉っぱの枚数
    /// </summary>
    private const int LEAF_COUNT = 10;

    /// <summary>
    /// 螺旋の時間
    /// </summary>
    private const float SPIRAL_TIME = 1f;

    /// <summary>
    /// BURSTに要する時間
    /// </summary>
    private const float BURST_TIME = 1f;

    /// <summary>
    /// 葉っぱパーティクルの設定
    /// </summary>
    private static readonly Props LEAF_PROPS = new Props() {
      IsSelfDestructive = false,
    };

    /// <summary>
    /// 葉っぱパーティクル(軌跡)の設定
    /// </summary>
    private static readonly Props LEAF_TRACE_PROPS = new Props() {
      AlphaAcceleration = -3f,
      ScaleAcceleration = -1f,
    };

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 葉っぱパーティクル配列
    /// </summary>
    private IParticle[] lLeaves = new IParticle[LEAF_COUNT];
    private IParticle[] rLeaves = new IParticle[LEAF_COUNT];

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 葉っぱのテクスチャ
    /// </summary>
    public static Sprite Sprite1 = null;
    public static Sprite Sprite2 = null;

    /// <summary>
    /// 加算合成用マテリアル
    /// </summary>
    public static Material Material;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Tre.01.sprite", pre, done, (res) => { Sprite1 = res; });
      rs.Load<Sprite>("Skill.Tre.02.sprite", pre, done, (res) => { Sprite2 = res; });
      rs.Load<Material>("Common.Additive.material", pre, done, (res) => { Material = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Ice.01.sprite");
      rs.Unload("Skill.Ice.02.sprite");
      rs.Unload("Common.Additive.material");
      Sprite1 = null;
      Sprite2 = null;
      Material = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Spiral, OnSpiralEnter, OnSpiralUpdate);
      this.state.Add(State.Burst, OnBurstEnter, OnBurstUpdate, OnBurstExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // IEffectの実装

    /// <summary>
    /// Spriteを設定する
    /// </summary>
    public override void Setup()
    {
      var pm = ParticleManager.Instance;

      for(int i = 0; i < LEAF_COUNT; ++i) 
      {
        LEAF_PROPS.Sprite = LEAF_TRACE_PROPS.Sprite = Sprite1;
        this.lLeaves[i] = pm.Create(ParticleManager.Type.Standard);
        this.lLeaves[i].Setup(LEAF_PROPS);
        this.lLeaves[i].SetTrace(LEAF_TRACE_PROPS, 0.05f);

        LEAF_PROPS.Sprite = LEAF_TRACE_PROPS.Sprite = Sprite2;
        this.rLeaves[i] = pm.Create(ParticleManager.Type.Standard);
        this.rLeaves[i].Setup(LEAF_PROPS);
        this.rLeaves[i].SetTrace(LEAF_TRACE_PROPS, 0.05f);
      }
    }

    /// <summary>
    /// 発動
    /// </summary>
    public override void Fire(Vector3 position)
    {
      base.Fire(position);
      this.state.SetState(State.Spiral);
    }

    //-------------------------------------------------------------------------
    // Spiral
    // 葉っぱをらせん状に出現させる

    private void OnSpiralEnter()
    {
      for(int i = 0; i < LEAF_COUNT; ++i) {
        this.lLeaves[i].Fire(CacheTransform.position, Vector3.zero);
        this.rLeaves[i].Fire(CacheTransform.position, Vector3.zero);
      }
      
      this.timer = 0;
    }

    private void OnSpiralUpdate()
    {
      const float COUNT_BIAS    = 0.1f; // 葉っぱの枚数に少し偏りを持たせる
      const float ROTATION_BIAS = 180f; // 右側の葉っぱはスプライトの回転を180度増やす

      // 進捗
      var rate     = this.timer / SPIRAL_TIME;
      var interval = SPIRAL_TIME / (LEAF_COUNT + COUNT_BIAS);
      
      for (int i = 0; i < LEAF_COUNT; ++i) 
      {
        var progress = Mathf.Max(0, rate - interval * i);

        if (progress <= 0) continue;

        Spiral(this.lLeaves[i], progress, Vector3.left, 0);
        Spiral(this.rLeaves[i], progress, Vector3.right, ROTATION_BIAS);
      }

      UpdateTimer();

      if (SPIRAL_TIME < this.timer) {
        this.state.SetState(State.Burst);
      }
    }

    /// <summary>
    /// 螺旋の動きを計算して葉っぱの位置や回転を調整する
    /// </summary>
    private void Spiral(IParticle leaf, float progress, Vector3 direction, float rotationBias)
    {
      const float SPIRAL_ROTATION = 180f *3f; // スパイラルする回転量
      const float SPIRAL_POWER = 4f;          // スパイラルの強さ
      const float POSITION_BIAS = 0.05f;      // 位置の偏り
      const float POSITION_ADJUST = 0.5f;     // 位置調整値
      const float MIN_SCALE = 0.5f;           // 最小サイズ
      const float ROTATION = 540f;            // 回転量

      // 位置を計算
      var rotation 
        = Quaternion.AngleAxis(progress * SPIRAL_ROTATION, Vector3.forward);

      var length 
        = (POSITION_BIAS + progress * POSITION_ADJUST + Mathf.Pow(progress, SPIRAL_POWER));

      leaf.CacheTransform.position
        = rotation * direction * length;

      // スケールを計算
      var scale = Mathf.Min(1, progress + MIN_SCALE);

      leaf.CacheTransform.localScale
        = Vector3.one * scale;

      // 回転を計算
      var angle = progress * ROTATION + rotationBias;

      leaf.CacheTransform.rotation 
        = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    //-------------------------------------------------------------------------
    // Burst
    // 葉っぱをはじけさせる

    private void OnBurstEnter()
    {
      for(int i = 0; i < LEAF_COUNT; ++i) {
        Burst(this.lLeaves[i]);
        Burst(this.rLeaves[i]);
      }

      Action?.Invoke();

      this.timer = 0;
    }

    private void OnBurstUpdate()
    {
      UpdateTimer();

      if (BURST_TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnBurstExit()
    {
      for(int i = 0; i < LEAF_COUNT; ++i) {
        this.lLeaves[i] = null;
        this.rLeaves[i] = null;
      }

      EffectManager.Instance.Release(EffectManager.Type.Leaves, this);
    }
    /// <summary>
    /// BURST時のパーティクル設定
    /// 移動速度、フェード速度、回転速度
    /// </summary>

    /// <summary>
    /// 葉っぱをはじけさせる
    /// </summary>
    private void Burst(IParticle left)
    {
      const float BURST_SPEED          = 2f;
      const float BURST_FADE_SPEED     = -2f;
      const float BURST_ROTATION_SPEED = 720f;

      var v = left.CacheTransform.position - CacheTransform.position;
      left.IsSelfDestructive = true;
      left.Velocity = v.normalized * BURST_SPEED;
      left.AlphaAcceleration = BURST_FADE_SPEED;
      left.RotationAcceleration = BURST_ROTATION_SPEED;
      left.LifeTime = BURST_TIME;
      left.SetTrace(null);
    }
  }
}
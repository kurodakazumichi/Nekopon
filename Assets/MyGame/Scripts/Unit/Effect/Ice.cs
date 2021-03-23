using MyGame.Unit.Particle;
using UnityEngine;

namespace MyGame.Unit.Effect
{
  /// <summary>
  /// 氷のエフェクト
  /// </summary>
  public class Ice : EffectBase<Ice.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Circle,
      Burst,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 回転の時間
    /// </summary>
    private const float CIRCLE_TIME = 1f;

    /// <summary>
    /// 内側の結晶の数
    /// </summary>
    private const int INNER_COUNT = 5;

    /// <summary>
    /// 内側の半径
    /// </summary>
    private const float INNER_RADIUS = 0.13f;

    /// <summary>
    /// 内側の移動速度
    /// </summary>
    private const float INNER_VELOCITY = 180f;

    /// <summary>
    /// 外側の結晶の数
    /// </summary>
    private const int OUTER_COUNT = 10;

    /// <summary>
    /// 外側の半径
    /// </summary>
    private const float OUTER_RADIUS = 0.3f;

    /// <summary>
    /// 外側の移動速度
    /// </summary>
    private const float OUTER_VELOCITY = -180f;

    /// <summary>
    /// バーストに要する時間
    /// </summary>
    private const float BURST_TIME = 0.3f;

    /// <summary>
    /// バースト時の移動速度
    /// </summary>
    private const float BURST_VELOCITY = 5f;

    /// <summary>
    /// 円運動中の軌跡を出す間隔
    /// </summary>
    private const float CURCLE_TRACE_TIME = 0.15f;

    /// <summary>
    /// バースト時：軌跡を出す感覚
    /// </summary>
    private const float BURST_TRACE_TIME = 0.5f;

    /// <summary>
    /// バースト時：雪が降る速度を決めるための重力(最大・最小)
    /// </summary>
    const float MIN_BURST_GRAVITY = 1f;
    const float MAX_BURST_GRAVITY = 5f;

    /// <summary>
    /// Inner Particleの設定
    /// </summary>
    private static Props INNER_PROPS = new Props() 
    {
      IsSelfDestructive = false,
      RotationAcceleration = 360f,
    };

    /// <summary>
    /// Outer Particleの設定
    /// </summary>
    private static Props OUTER_PROPS = new Props() {
      IsSelfDestructive = false,
      RotationAcceleration = -360f,
    };

    /// <summary>
    /// サークル中の軌跡設定
    /// </summary>
    private static Props CIRCLE_TRACE_PROPS = new Props() {
      ScaleAcceleration = -8f,
      Brightness = 0.1f,
      AlphaAcceleration = -3f
    };

    /// <summary>
    /// バースト中の軌跡設定
    /// </summary>
    private static Props BURST_TRACE_PROPS = new Props() {
      MainIsEnabled = false,
      ScaleAcceleration = -1f,
      AlphaAcceleration = -0.5f,
      Brightness = 0.5f,
      RotationAcceleration = 360f,
      IsSelfDestructive = true,
      LifeTime = 1f
    };

    //-------------------------------------------------------------------------
    // メンバ変数

    private IParticle[] inners = new IParticle[INNER_COUNT];
    private IParticle[] outers = new IParticle[OUTER_COUNT];

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雫のテクスチャ
    /// </summary>
    public static Sprite OuterSprite = null;
    public static Sprite InnerSprite = null;

    /// <summary>
    /// 加算合成用マテリアル
    /// </summary>
    public static Material Material;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Ice.01.sprite", pre, done, (res) => { OuterSprite = res; });
      rs.Load<Sprite>("Skill.Ice.02.sprite", pre, done, (res) => { InnerSprite = res; });
      rs.Load<Material>("Common.Additive.material", pre, done, (res) => { Material = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Ice.01.sprite");
      rs.Unload("Skill.Ice.02.sprite");
      rs.Unload("Common.Additive.material");
      OuterSprite = null;
      InnerSprite = null;
      Material = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Circle, OnCircleEnter, OnCircleUpdate);
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
      // Particleの設定
      INNER_PROPS.Sprite = InnerSprite;
      OUTER_PROPS.Sprite = OuterSprite;
      CIRCLE_TRACE_PROPS.Sprite = InnerSprite;

      var pm = ParticleManager.Instance;

      // 内側のParticleを生成
      for (int i = 0; i < INNER_COUNT; ++i) 
      {
        var p = pm.Create(ParticleManager.Type.Standard);

        p.Setup(INNER_PROPS);
        p.SetTrace(CIRCLE_TRACE_PROPS, CURCLE_TRACE_TIME);

        this.inners[i] = p;
      }

      // 外側のパーティクルを生成
      for (int i = 0; i < OUTER_COUNT; ++i) 
      {
        var p = pm.Create(ParticleManager.Type.Standard);

        p.Setup(INNER_PROPS);
        p.SetTrace(CIRCLE_TRACE_PROPS, CURCLE_TRACE_TIME);

        this.outers[i] = p;
      }
    }

    /// <summary>
    /// 発動
    /// </summary>
    public override void Fire(Vector3 position)
    {
      base.Fire(position);
      this.state.SetState(State.Circle);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    //-------------------------------------------------------------------------
    // Circle
    // 氷の結晶をくるくるさせる処理

    private void OnCircleEnter()
    {
      Util.ForEach(this.inners, (particle, _) => {
        particle.Fire(CacheTransform.position);
      });
      Util.ForEach(this.outers, (particle, _) => {
        particle.Fire(CacheTransform.position);
      });

      this.timer = 0;
    }

    private void OnCircleUpdate()
    {
      Util.ForEach(this.inners, (particle, index) => {
        Circle(particle, index, INNER_COUNT, INNER_RADIUS, INNER_VELOCITY);
      });

      Util.ForEach(this.outers, (particle, index) => {
        Circle(particle, index, OUTER_COUNT, OUTER_RADIUS, OUTER_VELOCITY);
      });

      UpdateTimer();

      if (CIRCLE_TIME < this.timer) {
        this.state.SetState(State.Burst);
      }
    }

    /// <summary>
    /// 円運動
    /// </summary>
    private void Circle(IParticle particle, int index, int count, float radius, float velocity)
    {
      // Position制御
      var a = 360f / count * index;
      var q = Quaternion.AngleAxis(a + (this.timer * velocity), Vector3.forward);
      var p = q * Vector3.right * radius;
      particle.CacheTransform.position = CacheTransform.position + p;
    }

    //-------------------------------------------------------------------------
    // Burst
    // 氷の結晶がはじける演出

    private void OnBurstEnter() 
    {
      Action?.Invoke();

      this.timer = 0;
      Util.ForEach(this.inners, (particle, _) => { Burst(particle); });
      Util.ForEach(this.outers, (particle, _) => { Burst(particle); });
    }

    private void OnBurstUpdate()
    {
      UpdateTimer();

      if (this.timer <= BURST_TIME) return;

      this.state.SetState(State.Idle);
    }

    private void OnBurstExit()
    {
      for(int i = 0; i < INNER_COUNT; ++i) {
        this.inners[i] = null;
      }

      for(int i = 0; i < OUTER_COUNT; ++i) {
        this.outers[i] = null;
      }

      EffectManager.Instance.Release(EffectManager.Type.Ice, this);
    }

    /// <summary>
    /// はじける動き
    /// </summary>
    private void Burst(IParticle particle)
    {
      // 原点からParticleに向かうベクトルから速度を決定
      var p1 = CacheTransform.position;
      var p2 = particle.CacheTransform.position;
      var v = (p2 - p1).normalized * BURST_VELOCITY;
      particle.Velocity = v;

      // その他設定
      BURST_TRACE_PROPS.Sprite = particle.Sprite;
      BURST_TRACE_PROPS.Gravity = Random.Range(MIN_BURST_GRAVITY, MAX_BURST_GRAVITY);
            
      particle.SetTrace(BURST_TRACE_PROPS, BURST_TRACE_TIME);
    }
  }
}
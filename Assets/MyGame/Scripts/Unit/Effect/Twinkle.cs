using MyGame.Unit.Particle;
using UnityEngine;

namespace MyGame.Unit.Effect
{
  public class Twinkle : EffectBase<Twinkle.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Active
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// キラキラの数
    /// </summary>
    private const int PARTICLE_COUNT = 10;

    /// <summary>
    /// アクティブ状態の時間
    /// </summary>
    private const float ACTIVE_TIME = 1f;

    /// <summary>
    /// パーティクル設定
    /// </summary>
    private static readonly Props PROPS = new Props() {
      AlphaAcceleration = -1f,
      Brightness = 0.2f,
      LifeTime = ACTIVE_TIME,
    };

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// パーティクル配列
    /// </summary>
    private IParticle[] particles = new IParticle[PARTICLE_COUNT];

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// きらきらsプライと
    /// </summary>
    private static Sprite Sprite = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Effect.Twinkle.sprite", pre, done, (res) => { Sprite = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Effect.Twinkle.sprite");
      Sprite = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Active, OnActiveEnter, OnActiveUpdate, OnActiveExit);
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

      for (int i = 0; i < PARTICLE_COUNT; ++i) 
      {
        // 回転とスケールの加速度
        float rotationAccele = (720f / PARTICLE_COUNT * i) + 72f;
        float scaleAccele    = -0.1f * i - 1f;

        this.particles[i] = pm.Create(ParticleManager.Type.Standard);
        this.particles[i].Setup(PROPS);
        this.particles[i].Sprite = Sprite;
        this.particles[i].RotationAcceleration = rotationAccele;
        this.particles[i].ScaleAcceleration = scaleAccele;
      }
    }

    /// <summary>
    /// 発動
    /// </summary>
    public override void Fire(Vector3 position)
    {
      base.Fire(position);
      this.state.SetState(State.Active);
    }

    //-------------------------------------------------------------------------
    // Active

    private void OnActiveEnter()
    {
      Action?.Invoke();

      Util.ForEach(this.particles, (p, i) => { 
        p.Fire(CacheTransform.position);
      });

      this.timer = 0;
    }

    private void OnActiveUpdate()
    {
      UpdateTimer();

      if (ACTIVE_TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnActiveExit()
    {
      for(int i = 0; i < PARTICLE_COUNT; ++i) {
        this.particles[i] = null;
      }

      EffectManager.Instance.Release(EffectManager.Type.Twinkle, this);
    }
  }
}
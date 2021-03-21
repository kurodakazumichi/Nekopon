using UnityEngine;

namespace MyGame.Unit.Particle
{

  public class Standard : Base<Standard.State>
  {
    public enum State { 
      Idle,
      Usual,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 加算合成用マテリアル
    /// </summary>
    private static Material Material = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Material>("Common.Additive.material", pre, done, (res) => { Material = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Common.Additive.material");
      Material = null;
    }

    //-------------------------------------------------------------------------
    // プロパティ

    //-------------------------------------------------------------------------
    // IParticleの実装

    /// <summary>
    /// セットアップ
    /// </summary>
    public override void Setup()
    {
      GlowMaterial = Material;
      Brightness = 0;
    }

    /// <summary>
    /// 発動
    /// </summary>
    public override void Fire(Vector3 position)
    {
      CacheTransform.position = position;
      this.state.SetState(State.Usual);
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // MainとGlowのSpriteオブジェクトを生成
      this.main = MyGameObject.Create<SpriteRenderer>("Main", CacheTransform);
      this.main.sortingOrder = Define.Layer.Order.Layer00;

      this.glow = MyGameObject.Create<SpriteRenderer>("Glow", CacheTransform);
      this.glow.sortingOrder = Define.Layer.Order.Layer00 + 1;

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Usual, OnUsualEnter, OnUsualUpdate, OnUsualExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // Usual

    private void OnUsualEnter()
    {
      this.timer = 0;
      this.traceTimer = 0;
    }

    private void OnUsualUpdate()
    {
      float deltaTime = TimeSystem.Instance.DeltaTime;

      OperatePosition(deltaTime);
      OperateAlpha(deltaTime);
      OperateRotation(deltaTime);
      OperationTrace(deltaTime);
      UpdateTimer();

      if (NeedToIdle) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnUsualExit()
    {
      this.trace = null;
      ParticleManager.Instance.Release(ParticleManager.Type.Standard, this);
    }

    /// <summary>
    /// 座標の移動
    /// </summary>
    /// <param name="deltaTime"></param>
    private void OperatePosition(float deltaTime)
    {
      CacheTransform.position += Velocity * deltaTime;
    }

    /// <summary>
    /// アルファ加速度
    /// </summary>
    private void OperateAlpha(float deltaTime)
    {
      Alpha += AlphaAcceleration * deltaTime;
    }

    private void OperateRotation(float deltaTime)
    {
      CacheTransform.Rotate(Vector3.forward, RotationAcceleration * deltaTime);
    }

    private void OperationTrace(float deltaTime)
    {
      if (this.trace == null) return;

      if (TraceTime < this.traceTimer) {
        var p = ParticleManager.Instance.Create(ParticleManager.Type.Standard);
        p.Setup();
        p.Setup(this.trace);
        p.Fire(CacheTransform.position);
        this.traceTimer = 0;
      }

      this.traceTimer += deltaTime;
    }

    /// <summary>
    /// Idleになる必要があるか
    /// </summary>
    private bool NeedToIdle
    {
      get {
        if (this.Alpha < 0) return true;
        if (this.LifeTime < this.timer) return true;
        return false;
      }
    }
  }
}
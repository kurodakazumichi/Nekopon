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
    public override void Fire(
      Vector3 position, 
      Vector3? scale = null, 
      Quaternion? rotation = null
    )
    {
      base.Fire(position, scale, rotation);
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

    /// <summary>
    /// 破壊
    /// </summary>
    public override void Destory()
    {
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
      OperateScale(deltaTime);
      OperateRotation(deltaTime);
      OperateAlpha(deltaTime);
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
    
    private void OperateScale(float deltaTime)
    {
      if (ScaleAcceleration == 0) return;
     
      CacheTransform.localScale += Vector3.one * ScaleAcceleration * deltaTime;

      if (
        CacheTransform.localScale.x < 0 ||
        CacheTransform.localScale.y < 0 ||
        CacheTransform.localScale.z < 0
      ) {
        CacheTransform.localScale = Vector3.zero;
      }
    }

    private void OperateRotation(float deltaTime)
    {
      if (RotationAcceleration == 0) return;
      CacheTransform.Rotate(Vector3.forward, RotationAcceleration * deltaTime);
    }

    /// <summary>
    /// アルファ加速度
    /// </summary>
    private void OperateAlpha(float deltaTime)
    {
      if (AlphaAcceleration == 0) return;
      Alpha += AlphaAcceleration * deltaTime;
    }

    private void OperationTrace(float deltaTime)
    {
      if (this.trace == null) return;

      if (TraceTime < this.traceTimer) {
        var p = ParticleManager.Instance.Create(ParticleManager.Type.Standard);
        p.Setup();
        p.Setup(this.trace);
        p.Fire(CacheTransform.position, CacheTransform.localScale, CacheTransform.rotation);
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
        // 自己破壊しない設定の場合は外部から破壊しない限りIdleにならない
        if (!this.IsSelfDestructive) return false;
        if (this.Alpha < 0) return true;
        if (this.LifeTime < this.timer) return true;
        return false;
      }
    }
  }
}
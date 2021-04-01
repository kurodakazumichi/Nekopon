using UnityEngine;

namespace MyGame.Unit.Particle
{

  public class Standard : Base<Standard.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
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
    public override void Setup(ParticleManager.Type type)
    {
      base.Setup(type);
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
      OperateGravity(deltaTime);
      OperateScale(deltaTime);
      OperateRotation(deltaTime);
      OperateAlpha(deltaTime);
      OperateTrace(deltaTime);
      OperateLifeTime(deltaTime);
      OperateBound();

      if (NeedToIdle) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnUsualExit()
    {
      isTraceEnabled = false;
      ParticleManager.Instance.Release(this);
    }
  }
}
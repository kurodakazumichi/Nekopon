﻿using UnityEngine;

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
      this.state.Add(State.Idle, OnIdleEnter);
      this.state.Add(State.Usual, OnUsualEnter, OnUsualUpdate);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // Idle

    private void OnIdleEnter()
    {
      ParticleManager.Instance.Release(ParticleManager.Type.Standard, this);
    }

    //-------------------------------------------------------------------------
    // Usual

    private void OnUsualEnter()
    {
      this.timer = 0;
    }

    private void OnUsualUpdate()
    {
      float deltaTime = TimeSystem.Instance.DeltaTime;

      OperatePosition(deltaTime);
      OperateAlpha(deltaTime);

      UpdateTimer();

      if (NeedToIdle) {
        this.state.SetState(State.Idle);
      }
    }

    private void OperatePosition(float deltaTime)
    {
      CacheTransform.position += Velocity * deltaTime;
    }

    private void OperateAlpha(float deltaTime)
    {
      Alpha += AlphaAcceleration * deltaTime;
    }

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
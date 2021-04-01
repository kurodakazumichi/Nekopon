using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Particle;

namespace MyGame.Unit.Effect
{
  public class Spark : EffectBase<Spark.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Spark,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 火花
    /// </summary>
    private const int COUNT = 30;

    /// <summary>
    /// パーティクルの設定
    /// </summary>
    private static readonly Props PROPS = new Props() {
      Gravity = 1f,
      IsBoundEnabled = true,
      Elasticity = 0.2f,
      ScaleAcceleration = -1f,
      Brightness = 0.3f
    };

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 火花のテクスチャ
    /// </summary>
    public static Sprite Sprite = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Fir.02.sprite", pre, done, (res) => { Sprite = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Fir.02.sprite");
      Sprite = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 状態の構築
      this.state.Add(State.Idle);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // IEffectの実装

    public override void Setup()
    {
      PROPS.Sprite = Sprite;
    }

    /// <summary>
    /// 発動
    /// </summary>
    public override void Fire(Vector3 position)
    {
      base.Fire(position);
      CreateParticles();
      EffectManager.Instance.Release(EffectManager.Type.Spark, this);
    }

    /// <summary>
    /// パーティクル生成
    /// </summary>
    private void CreateParticles()
    {
      const float SPEED = 2f;
      const float MIN_SCALE = 0.5f;
      const float MAX_SCALE = 1f;

      var pm = ParticleManager.Instance;

      // 火花パーティクルを生成
      for (int i = 0; i < COUNT; ++i) {
        var p = pm.Create(ParticleManager.Type.Standard);
        p.Setup(PROPS);
        p.Velocity = MyVector3.Random() * SPEED;
        p.CacheTransform.localScale *= Random.Range(MIN_SCALE, MAX_SCALE);
        p.Fire(CacheTransform.position);
      }
    }
  }
}
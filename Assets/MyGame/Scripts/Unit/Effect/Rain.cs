using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Particle;

namespace MyGame.Unit.Effect
{
  public class Rain : EffectBase<Rain.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State {
      Idle,
      Rain,
      Wait,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 雨が降り注ぐ時間
    /// </summary>
    private const float ACTIVE_TIME = 0.7f;

    /// <summary>
    /// 雫の落ちる時間(最大)
    /// </summary>
    private const float RAIN_TIME = 1f;

    /// <summary>
    /// 雫のパーティクル設定
    /// </summary>
    private static readonly Props DROP_PROPS = new Props(){ 
      Gravity = 5f,
      Brightness = 0.2f,
      LifeTime = RAIN_TIME
    };

    /// <summary>
    /// 雫の軌跡設定
    /// </summary>
    private static readonly Props DROP_TRACE_PROPS = new Props() {
      MainIsEnabled = false,
      Gravity = 0.1f,
      ScaleAcceleration = -1.5f,
      AlphaAcceleration = -1.5f,
      Brightness = 1f,
      LifeTime = RAIN_TIME * 0.5f,
    };

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雫のテクスチャ
    /// </summary>
    public static List<Sprite> Sprites = new List<Sprite>();

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Wat.01.sprite", pre, done, (res) => { Sprites.Add(res); });
      rs.Load<Sprite>("Skill.Wat.02.sprite", pre, done, (res) => { Sprites.Add(res); });
      Mover.Glow.Load(pre, done);
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Wat.01.sprite");
      rs.Unload("Skill.Wat.02.sprite");
      Mover.Glow.Unload();
      Sprites.Clear();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Rain, OnRainEnter, OnRainUpdate);
      this.state.Add(State.Wait, OnWaitEnter, OnWaitUpdate, OnWaitExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // IEffectの実装

    /// <summary>
    /// 発動
    /// </summary>
    public override void Fire(Vector3 position)
    {
      base.Fire(position);
      this.state.SetState(State.Rain);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    //-------------------------------------------------------------------------
    // Rain
    // 雨を降らす(Effect的にはこの状態以外の時はIdleとして扱う)

    private void OnRainEnter()
    {
      this.timer = 0;
    }

    private void OnRainUpdate()
    {
      CreateDrop();

      UpdateTimer();

      if (ACTIVE_TIME < this.timer) {
        Action?.Invoke();
        this.state.SetState(State.Wait);
      }
    }

    //-------------------------------------------------------------------------
    // Wait
    // 雨が降り終わるのを待って、終わったら返却する

    private void OnWaitEnter()
    {
      this.timer = 0;
    }

    private void OnWaitUpdate()
    {
      UpdateTimer();

      if (RAIN_TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnWaitExit()
    {
      EffectManager.Instance.Release(EffectManager.Type.Rain, this);
    }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 雨の雫を生成する
    /// </summary>
    private void CreateDrop()
    {
      const float DROP_WIDTH     = 0.3f;  // 雨が1点から降ると不自然なのでばらつきを持たせるための幅
      const float TRACE_MIN_TIME = 0.05f; // 軌跡を生成する時間(最速)
      const float TRACE_MAX_TIME = 0.1f;  // 軌跡を生成する時間(最遅)

      // 位置決め
      Vector3 position = CacheTransform.position;
      position.x += Random.Range(-DROP_WIDTH, DROP_WIDTH);

      // Sprite決定
      var sprite = Util.GetRandom(Sprites);

      // パーティクル生成
      var p = ParticleManager.Instance.Create(ParticleManager.Type.Standard);
      DROP_PROPS.Sprite = sprite;
      p.Setup(DROP_PROPS);

      DROP_TRACE_PROPS.Sprite = sprite;
      p.SetTrace(DROP_TRACE_PROPS, Random.Range(TRACE_MIN_TIME, TRACE_MAX_TIME));

      // 発動
      p.Fire(position);
    }
  }

}


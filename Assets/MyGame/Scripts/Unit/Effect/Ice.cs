using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Effect
{
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
    // メンバ変数

    /// <summary>
    /// 回転の時間
    /// </summary>
    private const float CIRCLE_TIME = 1f;

    /// <summary>
    /// 内側の結晶の数
    /// </summary>
    private const int INNER_COUNT = 5;

    /// <summary>
    /// 外側の結晶の数
    /// </summary>
    private const int OUTER_COUNT = 10;

    /// <summary>
    /// 内側の半径
    /// </summary>
    private const float INNER_RADIUS = 0.13f;

    /// <summary>
    /// 内側の移動速度
    /// </summary>
    private const float INNER_VELOCITY = 180f;

    /// <summary>
    /// 内側の回転速度
    /// </summary>
    private const float INNER_ANGULAR = 360f;

    /// <summary>
    /// 外側の半径
    /// </summary>
    private const float OUTER_RADIUS = 0.3f;

    /// <summary>
    /// 外側の移動速度
    /// </summary>
    private const float OUTER_VELOCITY = -180f;

    /// <summary>
    /// 外側の回転速度
    /// </summary>
    private const float OUTER_ANGULAR = -360f;

    /// <summary>
    /// 輝きの周期
    /// </summary>
    private const float FLASH_CYCLE = 5f;

    /// <summary>
    /// 最大輝度
    /// </summary>
    private const float MAX_BRIGHTNESS = 0.4f;

    /// <summary>
    /// バーストに要する時間
    /// </summary>
    private const float BURST_TIME = 0.3f;

    /// <summary>
    /// バースト時の移動速度
    /// </summary>
    private const float BURST_VELOCITY = 2f;

    //-------------------------------------------------------------------------
    // メンバ変数

    private Props.GlowMover[] inners = new Props.GlowMover[INNER_COUNT];
    private Props.GlowMover[] outers = new Props.GlowMover[OUTER_COUNT];

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
      for (int i = 0; i < INNER_COUNT; ++i) {
        this.inners[i] = MyGameObject.Create<Props.GlowMover>("Inner", CacheTransform);
      }

      for (int i = 0; i < OUTER_COUNT; ++i) {  
        this.outers[i] = MyGameObject.Create<Props.GlowMover>("Outer", CacheTransform);
      }

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Circle, OnCircleEnter, OnCircleUpdate);
      this.state.Add(State.Burst, OnBurstEnter, OnBurstUpdate, OnBurstExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // IEffectの実装

    /// <summary>
    /// 氷がぐるぐるしてる間以外はIdle扱いとしよう
    /// </summary>
    public override bool IsIdle => (this.state.StateKey != State.Circle);

    /// <summary>
    /// Spriteを設定する
    /// </summary>
    public override void Setup()
    {
      Util.ForEach(this.inners, (mover, _) => { 
        mover.Setup(InnerSprite, Material, Define.Layer.Sorting.Effect);
      });

      Util.ForEach(this.outers, (mover, _) => {
        mover.Setup(OuterSprite, Material, Define.Layer.Sorting.Effect);
      });
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
      Util.ForEach(this.inners, (mover, _) => {
        mover.SetFlash(FLASH_CYCLE, 0, MAX_BRIGHTNESS);
        mover.ToUsual();
      });
      Util.ForEach(this.outers, (mover, _) => {
        mover.SetFlash(FLASH_CYCLE, 0, MAX_BRIGHTNESS);
        mover.ToUsual();
      });

      this.timer = 0;
    }

    private void OnCircleUpdate()
    {
      Util.ForEach(this.inners, (mover, index) => {
        Circle(mover, index, INNER_COUNT, INNER_RADIUS, INNER_VELOCITY, INNER_ANGULAR);
      });

      Util.ForEach(this.outers, (mover, index) => {
        Circle(mover, index, OUTER_COUNT, OUTER_RADIUS, OUTER_VELOCITY, OUTER_ANGULAR);
      });

      UpdateTimer();

      if (CIRCLE_TIME < this.timer) {
        this.state.SetState(State.Burst);
      }
    }

    /// <summary>
    /// 円運動
    /// </summary>
    private void Circle(Props.GlowMover mover, int index, int count, float radius, float velocity, float angular)
    {
      // Position制御
      var a = 360f / count * index;
      var q = Quaternion.AngleAxis(a + (this.timer * velocity), Vector3.forward);
      var p = q * Vector3.right * radius;
      mover.CacheTransform.position = CacheTransform.position + p;

      // Rotation制御
      mover.CacheTransform.Rotate(Vector3.forward, angular * TimeSystem.Instance.DeltaTime);
    }

    //-------------------------------------------------------------------------
    // Burst
    // 氷の結晶がはじける演出

    private void OnBurstEnter() 
    {
      this.timer = 0;
      Util.ForEach(this.inners, (mover, _) => { Burst(mover); });
      Util.ForEach(this.outers, (mover, _) => { Burst(mover); });
    }

    private void OnBurstUpdate()
    {
      UpdateTimer();

      if (this.timer <= BURST_TIME) return;

      this.state.SetState(State.Idle);
    }

    private void OnBurstExit()
    {
      EffectManager.Instance.Release(EffectManager.Type.Ice, this);
    }

    /// <summary>
    /// はじける動き
    /// </summary>
    private void Burst(Props.GlowMover mover)
    {
      var p1 = CacheTransform.position;
      var p2 = mover.CacheTransform.position;
      var v = (p2 - p1).normalized * BURST_VELOCITY;
      mover.Tween = Tween.Type.EaseUniform;
      mover.ToMove(p2, p2 + v, BURST_TIME);
    }

  }
}
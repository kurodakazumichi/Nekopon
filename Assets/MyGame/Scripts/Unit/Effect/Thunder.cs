using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Effect
{
  /// <summary>
  /// 雷のエフェクト
  /// </summary>
  public class Thunder : EffectBase<Thunder.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Flash,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 落雷に要する時間
    /// </summary>
    public const float TIME = 0.7f;

    /// <summary>
    /// 落雷の点滅サイクル
    /// </summary>
    public const float CYCLE = 30f;

    /// <summary>
    /// 落雷の最小アルファ値
    /// </summary>
    public const float MIN_ALPHA = 0.5f;

    /// <summary>
    /// 雷の数
    /// </summary>
    private const int THUNDER_COUNT = 2;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 雷ユニット
    /// </summary>
    private Mover.Glow[] movers = new Mover.Glow[THUNDER_COUNT];

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雷のテクスチャ
    /// </summary>
    public static Sprite Sprite = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Thu.01.sprite", pre, done, (res) => { Sprite = res; });
      Mover.Glow.Load(pre, done);
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Thu.01.sprite");
      Mover.Glow.Unload();
      Sprite = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 雷を生成
      for (int i = 0; i < THUNDER_COUNT; ++i) {
        this.movers[i] = MyGameObject.Create<Mover.Glow>("Thunder", CacheTransform);
      }

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Flash, OnFlashEnter, OnFlashUpdate, OnFlashExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // IEffectの実装

    public override void Setup()
    {
      Util.ForEach(this.movers, (thunder, index) => {
        thunder.Setup(Sprite, Define.Layer.Sorting.Effect);
        thunder.SetActive(false);
      });
    }

    public override void Fire(Vector3 position)
    {
      base.Fire(position);
      this.state.SetState(State.Flash);
    }

    //-------------------------------------------------------------------------
    // Flash
    // 雷がぴかぴかする

    private void OnFlashEnter()
    {
      this.timer = 0;

      // 雷をフラッシュ
      Util.ForEach(this.movers, (mover, _) => {
        mover.SetActive(true);
        mover.SetFlash(CYCLE, MIN_ALPHA, 1f);
        mover.ToFlash(TIME);
      });
    }

    private void OnFlashUpdate()
    {
      UpdateTimer();

      if (this.timer < TIME) return;
      this.state.SetState(State.Idle);
    }

    private void OnFlashExit()
    {
      EffectManager.Instance.Release(EffectManager.Type.Thunder, this);
    }
  }
}
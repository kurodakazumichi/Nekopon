using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Effect
{

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
    private const float STRIKE_TIME = 0.7f;

    /// <summary>
    /// 落雷の点滅サイクル
    /// </summary>
    private const float STRIKE_FLASH_CYCLE = 30f;

    /// <summary>
    /// 落雷の最小アルファ値
    /// </summary>
    private const float STRIKE_MIN_ALPHA = 0.5f;

    /// <summary>
    /// 雷の数
    /// </summary>
    private const int THUNDER_COUNT = 2;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 雷ユニット
    /// </summary>
    private Props.GlowMover[] movers = new Props.GlowMover[THUNDER_COUNT];

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雷のテクスチャ
    /// </summary>
    public static Sprite Sprite = null;

    /// <summary>
    /// 加算合成用マテリアル
    /// </summary>
    public static Material Material = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Thu.01.sprite", pre, done, (res) => { Sprite = res; });
      rs.Load<Material>("Common.Additive.material", pre, done, (res) => { Material = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Thu.01.sprite");
      rs.Unload("Common.Additive.material");
      Sprite = null;
      Material = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 雷を生成
      for (int i = 0; i < THUNDER_COUNT; ++i) {
        this.movers[i] = MyGameObject.Create<Props.GlowMover>("Thunder", CacheTransform);
      }

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Flash, OnFlashEnter, OnFlashUpdate, OnFlashExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // IEffectの実装

    public override bool IsIdle => (this.state.StateKey == State.Idle);

    public override void Setup()
    {
      Util.ForEach(this.movers, (thunder, index) => {
        thunder.Setup(Sprite, Material, Define.Layer.Sorting.Effect);
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
        mover.SetFlash(STRIKE_FLASH_CYCLE, STRIKE_MIN_ALPHA, 1f);
        mover.ToFlash(STRIKE_TIME);
      });
    }

    private void OnFlashUpdate()
    {
      UpdateTimer();

      if (this.timer < STRIKE_TIME) return;
      this.state.SetState(State.Idle);
    }

    private void OnFlashExit()
    {
      EffectManager.Instance.Release(EffectManager.Type.Thunder, this);
    }
  }
}
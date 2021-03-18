using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class SkillThu : SkillBase<SkillThu.State>
  {
    public enum State
    {
      Idle,
      Create,
      Strike,
      Clear,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 雲が誕生するのに要する時間
    /// </summary>
    private const float CREATE_TIME = 0.2f;

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
    /// 落雷時の雲の最小アルファ値
    /// </summary>
    private const float CLOUD_MAX_ALPHA = 0.3f;

    /// <summary>
    /// 雲が消えるのに要する時間
    /// </summary>
    private const float CLEAR_TIME = 0.5f;

    /// <summary>
    /// 雷の数
    /// </summary>
    private const int THUNDER_COUNT = 2;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 雲ユニット
    /// </summary>
    private Props.GlowMover cloud = null;

    /// <summary>
    /// 雷ユニット
    /// </summary>
    private Props.GlowMover[] thunders = new Props.GlowMover[THUNDER_COUNT];

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雲のスプライト
    /// </summary>
    private static Sprite CloudSprite = null;

    /// <summary>
    /// 雷のスプライト
    /// </summary>
    private static Sprite ThunderSprite = null;

    /// <summary>
    /// 加算合成用マテリアル
    /// </summary>
    private static Material Material;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Cloud.sprite", pre, done, (res) => { CloudSprite = res; });
      rs.Load<Sprite>("Skill.Thu.01.sprite", pre, done, (res) => { ThunderSprite = res; });
      rs.Load<Material>("Common.Additive.material", pre, done, (res) => { Material = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Cloud.sprite");
      rs.Unload("Skill.Thu.01.sprite");
      rs.Unload("Common.Additive.material");
      CloudSprite = null;
      ThunderSprite = null;
      Material = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // ユニット生成
      this.cloud = MyGameObject.Create<Props.GlowMover>("Cloud", CacheTransform);

      for(int i = 0; i < THUNDER_COUNT; ++i) {
        this.thunders[i] = MyGameObject.Create<Props.GlowMover>("Thunder", CacheTransform);
      }

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Create, OnCreateEnter, OnCreateUpdate);
      this.state.Add(State.Strike, OnStrikeEnter, OnStrikeUpdte);
      this.state.Add(State.Clear, OnClearEnter, OnClearUpdate, OnClearExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // ISkillの実装

    public override void Setup()
    {
      this.cloud.Setup(CloudSprite, Material, Define.Layer.Sorting.Effect);

      Util.ForEach(this.thunders, (thunder, index) => {
        thunder.Setup(ThunderSprite, Material, Define.Layer.Sorting.Default);
        thunder.SetActive(false);
      });
    }

    public override void Fire(Player owner, Player target)
    {
      base.Fire(owner, target);
      this.state.SetState(State.Create);
    }

    //-------------------------------------------------------------------------
    // Create
    // 雲が登場するところ

    private void OnCreateEnter()
    {
      // 雷を無効化
      SetActiveThunders(false);

      // 雲を登場させる
      this.cloud.CacheTransform.position = this.target.Location.Top;
      this.cloud.Tween = Tween.Type.EaseOutBack;
      this.cloud.ToScale(Vector3.zero, Vector3.one, CREATE_TIME);
    }

    private void OnCreateUpdate()
    {
      if (!this.cloud.IsIdle) return;
      this.state.SetState(State.Strike);
    }

    //-------------------------------------------------------------------------
    // Strike
    // 雷が発生しする

    private void OnStrikeEnter()
    {
      // 雷をフラッシュ
      Util.ForEach(this.thunders, (thunder, _) => {
        thunder.SetActive(true);
        thunder.CacheTransform.position = this.target.Location.Center;
        thunder.MinAlpha = STRIKE_MIN_ALPHA;
        thunder.ToFlash(STRIKE_TIME, STRIKE_FLASH_CYCLE, 1f);
      });

      // 雲をフラッシュ
      this.cloud.CacheTransform.localScale = Vector3.one;
      this.cloud.ToFlash(STRIKE_TIME, STRIKE_FLASH_CYCLE, CLOUD_MAX_ALPHA);

      if (this.target.IsInvincible) {
        // 無敵ならガードSE
      } else {
        this.target.Paralyze();
      }
    }

    private void OnStrikeUpdte()
    {
      if (!this.cloud.IsIdle) return;
      this.state.SetState(State.Clear);
    }

    //-------------------------------------------------------------------------
    // Clear
    // 雲が消える
    
    private void OnClearEnter()
    {
      // 雷を無効化
      SetActiveThunders(false);

      // 雲を縮小
      this.cloud.Tween = Tween.Type.EaseInBack;
      this.cloud.ToScale(Vector3.one, Vector3.zero, CLEAR_TIME);
    }

    private void OnClearUpdate()
    {
      if (!this.cloud.IsIdle) return;
      this.state.SetState(State.Idle);
    }

    private void OnClearExit()
    {
      // スキルを返却
      SkillManager.Instance.Release(Define.App.Attribute.Thu, this);
    }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 雷の有効無効
    /// </summary>
    private void SetActiveThunders(bool isActive)
    {
      Util.ForEach(this.thunders, ((thunder, _) => { thunder.SetActive(isActive); }));
    }
  }
}


using MyGame.Define;
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
    /// 落雷時の雲の最小アルファ値
    /// </summary>
    private const float CLOUD_MAX_ALPHA = 0.3f;

    /// <summary>
    /// 雲が消えるのに要する時間
    /// </summary>
    private const float CLEAR_TIME = 0.5f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 雲ユニット
    /// </summary>
    private Mover.Glow cloud = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雲のスプライト
    /// </summary>
    private static Sprite Sprite = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Cloud.sprite", pre, done, (res) => { Sprite = res; });
      Mover.Glow.Load(pre, done);
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Cloud.sprite");
      Mover.Glow.Unload();
      Sprite = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // ユニット生成
      this.cloud = MyGameObject.Create<Mover.Glow>("Cloud", CacheTransform);

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
      this.cloud.Setup(Sprite, Define.Layer.Sorting.Effect);
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
      // 雷を生成
      EffectManager.Instance.Create(EffectManager.Type.Thunder)
        .Fire(this.target.Location.Center);

      // 雲をフラッシュ
      this.cloud.CacheTransform.localScale = Vector3.one;
      this.cloud.SetFlash(Effect.Thunder.CYCLE, 0f, CLOUD_MAX_ALPHA);
      this.cloud.ToFlash(Effect.Thunder.TIME);

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
      SkillManager.Instance.Release(this);
    }
  }
}


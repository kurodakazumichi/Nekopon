using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public partial class SkillWat : SkillBase<SkillWat.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State { 
      Idle,
      Create,
      Rain,
      Clear,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 最初に雲が出るのに要する時間
    /// </summary>
    private const float CREATE_TIME = 0.2f;

    /// <summary>
    /// 雲の片付けに要する時間
    /// </summary>
    const float CLEAN_TIME = 1f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    /// <summary>
    /// 雨のエフェクト
    /// </summary>
    IEffect effect = null;

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
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Cloud.sprite");
      Sprite = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // SpriteRenderer追加
      this.spriteRenderer = AddComponent<SpriteRenderer>();

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Create, OnCreateEnter, OnCreateUpdate);
      this.state.Add(State.Rain, OnRainEnter, null, OnRainExit);
      this.state.Add(State.Clear, OnCleanEnter, OnCleanUpdate, OnCleanExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // ISkillの実装

    public override void Setup()
    {
      this.spriteRenderer.sprite = Sprite;
      this.spriteRenderer.sortingLayerName = Define.Layer.Sorting.Effect;
      this.spriteRenderer.sortingOrder = Define.Layer.Order.Layer10;
    }

    public override void Fire(Player owner, Player target)
    {
      base.Fire(owner, target);
      this.state.SetState(State.Create);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    //-------------------------------------------------------------------------
    // Create
    // 雲を生成する

    private void OnCreateEnter()
    {
      CacheTransform.position = this.target.Location.Top;
      CacheTransform.localScale = Vector3.zero;
      this.timer = 0;
    }

    private void OnCreateUpdate()
    {
      float rate = this.timer / CREATE_TIME;

      CacheTransform.localScale
        = MyVector3.Lerp(Vector3.zero, Vector3.one, Tween.EaseOutBack(rate));

      UpdateTimer();

      if (CREATE_TIME < this.timer) {
        this.state.SetState(State.Rain);
      }
    }

    //-------------------------------------------------------------------------
    // Rain
    // 一定時間雨を降らし、最後にプレイヤーの状態を回復する

    private void OnRainEnter()
    {
      this.timer = 0;

      // 雨のエフェクトを生成
      this.effect = EffectManager.Instance.Create(EffectManager.Type.Rain);

      // エフェクト効果発動
      this.effect.Action = () => 
      {
        // 状態回復
        this.target.Cure();
        this.state.SetState(State.Clear);
      };

      this.effect.Fire(this.target.Location.Top);

    }

    private void OnRainExit()
    {
      this.effect.Action = null;
      this.effect = null;
    }

    //-------------------------------------------------------------------------
    // Clean
    // 雲を片付けて終了

    private void OnCleanEnter()
    {
      this.timer = 0; 
    }

    private void OnCleanUpdate()
    {
      float rate = this.timer / CLEAN_TIME;

      CacheTransform.localScale
        = MyVector3.Lerp(Vector3.one, Vector3.zero, Tween.EaseInBack(rate));

      UpdateTimer();

      if (CLEAN_TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnCleanExit()
    {
      SkillManager.Instance.Release(this);
    }
  }
}
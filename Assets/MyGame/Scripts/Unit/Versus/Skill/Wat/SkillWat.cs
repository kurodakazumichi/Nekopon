using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Versus.Skill;

namespace MyGame.Unit.Versus
{
  public partial class SkillWat : SkillBase<SkillWat.State>, SkillManager.ISkill 
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
    /// 雨が降り注ぐ時間
    /// </summary>
    private const float RAIN_TIME = 0.7f;

    /// <summary>
    /// 雫の落ちる時間(最小)
    /// </summary>
    private const float MIN_RAIN_TIME = 0.5f;

    /// <summary>
    /// 雫の落ちる時間(最大)
    /// </summary>
    private const float MAX_RAIN_TIME = 1f;

    /// <summary>
    /// 雫をランダム配置する際のばらつき幅(半分)
    /// </summary>
    private const float DROP_WIDTH = 0.3f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    /// <summary>
    /// 雫ユニットプール
    /// </summary>
    private ObjectPool<Drop> drops = new ObjectPool<Skill.Drop>();

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
      Drop.Load(pre, done);
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Cloud.sprite");
      Drop.Unload();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // SpriteRenderer追加
      this.spriteRenderer = AddComponent<SpriteRenderer>();

      // 雫ユニットのオブジェクトプール初期設定
      this.drops.SetGenerator(() => { 
        return MyGameObject.Create<Drop>("Drop", CacheTransform);
      });

      // 50個予約しとく
      this.drops.Reserve(50);

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Create, OnCreateEnter, OnCreateUpdate);
      this.state.Add(State.Rain, OnRainEnter, OnRainUpdate, OnRainExit);
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
    }

    private void OnRainUpdate()
    {
      // 1フレームで2雫作っておくか、可変フレーム対応になってないけどまぁヨシッ
      CreateDrop();
      CreateDrop();

      UpdateTimer();

      if (RAIN_TIME < this.timer) {
        this.state.SetState(State.Clear);
      }
    }

    private void OnRainExit()
    {
      // 状態回復
      this.target.Cure();
    }

    /// <summary>
    /// 雨の雫を生成する
    /// </summary>
    private void CreateDrop()
    {
      // 雫を作る
      var drop = this.drops.Create();

      // セットアップ(雫が落ち終わった時に呼ばれるコールバックを設定)
      drop.Setup((unit) => {
        this.drops.Release(unit, CacheTransform);
      });

      // 開始地点と到達地点を決定する
      Vector3 start = this.target.Location.Top;
      start.x += Random.Range(-DROP_WIDTH, DROP_WIDTH);
      Vector3 end = start;
      end.y = -1;

      // 発動
      drop.Fire(start, end, Random.Range(MIN_RAIN_TIME, MAX_RAIN_TIME));
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
      float rate = this.timer / MAX_RAIN_TIME;

      CacheTransform.localScale
        = MyVector3.Lerp(Vector3.one, Vector3.zero, Tween.EaseInBack(rate));

      UpdateTimer();

      if (MAX_RAIN_TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnCleanExit()
    {
      SkillManager.Instance.Release(Define.App.Attribute.Wat, this);
    }
  }
}
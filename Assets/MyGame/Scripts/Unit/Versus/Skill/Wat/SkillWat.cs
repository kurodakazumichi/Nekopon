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
    private ObjectPool<Props.GlowMover> drops = new ObjectPool<Props.GlowMover>();

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雲のスプライト
    /// </summary>
    private static Sprite CloudSprite = null;

    /// <summary>
    /// 雫のテクスチャ
    /// </summary>
    public static List<Sprite> DropSprites = new List<Sprite>();

    /// <summary>
    /// 加算合成用マテリアル
    /// </summary>
    public static Material Material;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Cloud.sprite", pre, done, (res) => { CloudSprite = res; });
      rs.Load<Sprite>("Skill.Wat.01.sprite", pre, done, (res) => { DropSprites.Add(res); });
      rs.Load<Sprite>("Skill.Wat.02.sprite", pre, done, (res) => { DropSprites.Add(res); });
      rs.Load<Material>("Common.Additive.material", pre, done, (res) => { Material = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Cloud.sprite");
      rs.Unload("Skill.Wat.01.sprite");
      rs.Unload("Skill.Wat.02.sprite");
      rs.Unload("Common.Additive.material");
      CloudSprite = null;
      DropSprites.Clear();
      Material = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // SpriteRenderer追加
      this.spriteRenderer = AddComponent<SpriteRenderer>();

      // 雫ユニットのオブジェクトプール初期設定
      this.drops.SetGenerator(() => { 
        return MyGameObject.Create<Props.GlowMover>("Drop", CacheTransform);
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
      this.spriteRenderer.sprite = CloudSprite;
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

      // セットアップ
      drop.Setup(
        Util.GetRandom(DropSprites), 
        Material,
        Define.Layer.Sorting.Effect
      );

      // 加算合成具合を設定
      drop.SetAdditive(Random.Range(0, 10f), 0.7f);

      // 移動後に呼ばれるコールバック設定
      drop.OnFinish = (unit) => {
        this.drops.Release(unit, CacheTransform);
      };

      // ToMoveに渡すパラメータを用意
      Vector3 start = this.target.Location.Top;
      start.x += Random.Range(-DROP_WIDTH, DROP_WIDTH);

      Vector3 end = start;
      end.y = -1;

      float time = Random.Range(MIN_RAIN_TIME, MAX_RAIN_TIME);

      // 発動
      drop.ToMove(start, end, time);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class SkillHol : SkillBase<SkillHol.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Move,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 食べ物の種類数
    /// </summary>
    private const int FOOD_COUNT = 5;

    /// <summary>
    /// 移動時間
    /// </summary>
    private const float MOVE_TIME = 1f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    /// <summary>
    /// 食べ物の種類
    /// </summary>
    private int foodIndex = -1;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// 食べ物の種類から回復率を取得する
    /// </summary>
    private float RecoveryRate 
    {
      get 
      {
        // 食べ物の種類が特定不可能であれば0
        if (this.foodIndex < 0 || FOOD_COUNT <= this.foodIndex) return 0;

        // 回復率
        float[] rates = new float[]{ 0.1f, 0.2f, 0.3f, 0.5f, 1f };
        return rates[this.foodIndex];
      }
    }

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 食べ物のスプライトリスト
    /// </summary>
    private static readonly Sprite[] Sprites = new Sprite[FOOD_COUNT];

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      
      for(int i = 0; i < FOOD_COUNT; ++i) 
      {
        // iをコールバック内で直接使うと値がループ終了後の5になってしまう。
        // C#だとクロージャー的な動作にならないっぽいのでローカルに入れ直し
        var index = i;

        rs.Load<Sprite>($"Skill.Hol.0{i+1}.sprite", pre, done, (res) => { 
          Sprites[index] = res; 
        });
      }
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;

      for(int i = 0; i < FOOD_COUNT; ++i) {
        rs.Unload($"Skill.Hol.0{i}.sprite");
      }
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // Component取得
      this.spriteRenderer = AddComponent<SpriteRenderer>();
      this.spriteRenderer.sortingLayerName = Define.Layer.Sorting.Effect;

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Move, OnMoveEnter, OnMoveUpdate, OnMoveExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // ISkillの実装

    public override void Setup()
    {
      this.foodIndex = CalcFoodIndex();
      this.spriteRenderer.sprite = Sprites[this.foodIndex];
    }

    public override void Fire(Player owner, Player target)
    {
      base.Fire(owner, target);
      this.state.SetState(State.Move);
    }

    //-------------------------------------------------------------------------
    // Move
    // 猫の頭上に食べ物が出てきて、落ちる。

    private void OnMoveEnter()
    {
      CacheTransform.position = this.owner.Location.AttackBase;
      this.basePosition   = CacheTransform.position;
      this.targetPosition = this.owner.Location.Cat;
      this.timer = 0;
    }

    private void OnMoveUpdate()
    {
      var rate = this.timer / MOVE_TIME;

      CacheTransform.position
        = Vector3.Lerp(this.basePosition, this.targetPosition, rate);

      UpdateTimer();

      if (MOVE_TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnMoveExit()
    {
      // プレイヤーの体力を回復
      this.owner.Recover(RecoveryRate);

      // エフェクト生成
      EffectManager.Instance.Create(EffectManager.Type.Twinkle)
        .Fire(this.owner.Location.Cat);

      // スキルは解放
      SkillManager.Instance.Release(this);
    }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 食べ物の種類を計算する
    /// </summary>
    private int CalcFoodIndex()
    {
      var rate = Random.Range(0, 100);

      // 確率
      int[] rates = new int[] {19, 69, 89, 97, 100};

      for (int i = 0; i < FOOD_COUNT; ++i) {
        if (rate <= rates[i]) return i;
      }

      return -1;
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// 通常攻撃ユニット
  /// </summary>
  public partial class Attack:Unit<Attack.State>, SkillManager.ISkill
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Usual,
      Attack,
      AttackEnd,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// InnerAttackの個数
    /// </summary>
    private const int INNER_COUNT = 10;

    /// <summary>
    /// 最小のスケール値
    /// </summary>
    private const float MIN_SCALE = 0.3f;

    /// <summary>
    /// 攻撃ユニットの最初の色
    /// </summary>
    private static readonly Color START_COLOR = new Color(0.9529412f, 0.2117647f, 0.5019608f, 0.5f);

    /// <summary>
    /// 攻撃ユニットの最終的な色
    /// </summary>
    private static readonly Color END_COLOR = new Color(0.2117647f, 0.9529412f, 0.89285f, 0.5f);

    /// <summary>
    /// 攻撃に要する時間
    /// </summary>
    private const float ATTACK_TIME = 0.5f;

    /// <summary>
    /// 振れ幅
    /// </summary>
    private static readonly Vector2 AMPLITUDE = new Vector2(0.05f, 0.02f);

    /// <summary>
    /// 周期
    /// </summary>
    private static readonly Vector2 CYCLES = new Vector2(2f, 5f);

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    /// <summary>
    /// 加算合成用のAttackオブジェクト
    /// </summary>
    private List<InnerAttack> children = new List<InnerAttack>();

    /// <summary>
    /// ベース位置
    /// </summary>
    private Vector3 basePosition = Vector3.zero;

    /// <summary>
    /// 攻撃する時に向かっていく座標
    /// </summary>
    private Vector3 targetPosition = Vector3.zero;

    /// <summary>
    /// 速度
    /// </summary>
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// 汎用タイマー
    /// </summary>
    private float timer = 0;

    /// <summary>
    /// 攻撃ヒット時のアクション
    /// </summary>
    private IAction action = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 攻撃用スプライト
    /// </summary>
    private static Sprite Sprite;

    /// <summary>
    /// 加算合成用マテリアル
    /// </summary>
    private static Material Material;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Attack.01.sprite", pre, done, (res) => { Sprite = res; });
      rs.Load<Material>("Common.Additive.material", pre, done, (res) => { Material = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Attack.01.sprite");
      rs.Unload("Common.Additive.material");
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      CreateMainAttack();
      CreateChildren();

      this.state.Add(State.Idle, OnIdleEnter);
      this.state.Add(State.Usual, OnUsualEnter, OnUsualUpdate);
      this.state.Add(State.Attack, OnAttackEnter, OnAttackUpdate);
      this.state.Add(State.AttackEnd, OnAttackEndEnter);
      this.state.SetState(State.Idle);
    }

    public void Setup()
    {
      // Mainのセットアップ
      this.spriteRenderer.sprite = Sprite;
      SetIntensity(0);

      // Innerのセットアップ
      this.children.ForEach((child) => {
        child.SetSprite(Sprite).SetMaterial(Material);
      });
    }

    //-------------------------------------------------------------------------
    // 生成系

    /// <summary>
    /// MainAttackを作成
    /// </summary>
    private void CreateMainAttack()
    {
      var go = new GameObject("AttackMain");
      go.transform.parent = CacheTransform;
      this.spriteRenderer = go.AddComponent<SpriteRenderer>();
      this.spriteRenderer.sortingLayerName = Define.Layer.Sorting.Effect;
    }

    /// <summary>
    /// InnerAttackを作成
    /// </summary>
    private void CreateChildren()
    {
      for(int i = 0; i < INNER_COUNT; ++i) {
        var child = new GameObject("Child").AddComponent<InnerAttack>();

        child
          .SetScale(i/(float)INNER_COUNT)
          .SetParent(CacheTransform);

        this.children.Add(child);
      }
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    /// <summary>
    /// Idle状態へ
    /// </summary>
    public void ToIdle()
    {
      this.state.SetState(State.Idle);
    }

    /// <summary>
    /// 通常状態へ
    /// </summary>
    public void ToUsual(Vector3 basePosition)
    {
      this.basePosition = basePosition;
      this.state.SetState(State.Usual);
    }

    /// <summary>
    /// 攻撃状態へ
    /// </summary>
    public void ToAttack(Vector3 targetPosition, IAction action)
    {
      this.action = action;
      this.targetPosition = targetPosition;
      this.state.SetState(State.Attack);
    }

    //-------------------------------------------------------------------------
    // Idle

    private void OnIdleEnter()
    {
      this.action = null;
    }

    //-------------------------------------------------------------------------
    // Usual

    private void OnUsualEnter()
    {
      this.timer = 0;
      this.velocity = Vector3.zero;
    }

    private void OnUsualUpdate()
    {
      // 座標計算
      this.velocity.x = Mathf.Cos(this.timer * CYCLES.x) * AMPLITUDE.x;
      this.velocity.y = Mathf.Sin(this.timer * CYCLES.y) * AMPLITUDE.y;
      CacheTransform.position = basePosition + this.velocity;

      // タイマーを更新
      this.timer += TimeSystem.Instance.DeltaTime;
    }

    //-------------------------------------------------------------------------
    // Attack

    private void OnAttackEnter()
    {
      this.timer = 0;
      this.basePosition = CacheTransform.position;
    }

    private void OnAttackUpdate()
    {
      // 座標更新
      var rate = Tween.EaseInOutSine(this.timer / ATTACK_TIME);
      CacheTransform.position
        = MyVector3.Lerp(basePosition, targetPosition, rate);

      // 時間経過
      this.timer += TimeSystem.Instance.DeltaTime;

      // 攻撃時間超えたらAttackEndへ
      if (ATTACK_TIME < this.timer) {
        this.state.SetState(State.AttackEnd);
      }
    }

    //-------------------------------------------------------------------------
    // AttackEnd

    private void OnAttackEndEnter()
    {
      this.action.Execute();
    }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 攻撃ユニットの強さを0~1で指定する、大きさと色が変化する
    /// </summary>
    public void SetIntensity(float rate)
    {
      // 最大は1f
      rate = Mathf.Min(1f, rate);

      // スケール調整
      CacheTransform.localScale = Vector3.one * Mathf.Min(1f, rate + MIN_SCALE);

      // 色調整
      this.spriteRenderer.color = Color.Lerp(START_COLOR, END_COLOR, rate);
    }
  }
}
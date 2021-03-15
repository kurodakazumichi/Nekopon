using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public partial class Attack:Unit<Attack.State>
  {
    public enum State
    {
      Idle,
      Usual,
      Attack,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// InnerAttackの個数
    /// </summary>
    private const int INNER_COUNT = 10;

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
    /// カラー
    /// </summary>
    private Color color = new Color(1, 1, 1, 0.5f);

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
      this.state.SetState(State.Idle);
    }

    public void Setup()
    {
      // Mainのセットアップ
      this.spriteRenderer.sprite = Sprite;
      this.spriteRenderer.color = this.color;

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

    public void ToIdle()
    {
      this.action = null;
      this.state.SetState(State.Idle);
    }

    public void ToUsual(Vector3 basePosition)
    {
      SetActive(true);
      this.basePosition = basePosition;
      this.state.SetState(State.Usual);
    }

    public void ToAttack(Vector3 targetPosition, IAction action)
    {
      this.action = action;
      this.targetPosition = targetPosition;
      this.state.SetState(State.Attack);
    }

    public void SetRate(float rate)
    {
      Debug.Logger.Log(rate);
      const float SCALE_BIAS = 0.3f;
      rate = Mathf.Min(1f, rate);
      
      CacheTransform.localScale = Vector3.one * Mathf.Min(1f, rate + SCALE_BIAS);
      this.color = Color.Lerp(Color.red, Color.blue, rate);
      this.color.a = 0.5f;
      this.spriteRenderer.color = this.color;
    }

    private void OnIdleEnter()
    {
      SetActive(false);
    }

    private void OnUsualEnter()
    {
      this.timer = 0;
      this.velocity = Vector3.zero;
    }

    private void OnUsualUpdate()
    {
      this.velocity.x = Mathf.Cos(this.timer * 2f) * 0.05f;
      this.velocity.y = Mathf.Sin(this.timer * 5f) * 0.02f;

      CacheTransform.position = basePosition + this.velocity;

      this.timer += TimeSystem.Instance.DeltaTime;
    }

    private void OnAttackEnter()
    {
      this.timer = 0;
      this.basePosition = CacheTransform.position;
    }

    private void OnAttackUpdate()
    {
      
      CacheTransform.position
        = MyVector3.Lerp(basePosition, targetPosition, Tween.EaseInOutSine(this.timer / 1f));

      this.timer += TimeSystem.Instance.DeltaTime;

      if (this.timer < 1f) return;

      // actionがある場合はactionにゆだねる
      if (action != null) {
        this.action.Execute();
      } else {
        this.state.SetState(State.Idle);
      }
    }
  }
}
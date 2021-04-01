using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// 属性スキル(火)
  /// 爆弾が出現し、相手にランダムなダメージを与える。
  /// </summary>
  public class SkillFir : SkillBase<SkillFir.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State { 
      Idle,
      Create,
      Attack,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 爆弾の誕生にかかる時間
    /// </summary>
    private const float CREATION_TIME = 0.7f;

    /// <summary>
    /// 爆弾が相手のパズルに向かうのに要する時間
    /// </summary>
    private const float ATTACK_TIME = 0.1f;

    /// <summary>
    /// 最小ダメージ量
    /// </summary>
    private const float MIN_DAMAGE = 200f;

    /// <summary>
    /// 最大ダメージ量
    /// </summary>
    private const float MAX_DAMAGE = 500f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// Spriteリソース
    /// </summary>
    public static Sprite Sprite;

    public static void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceSystem.Instance;
      rm.Load<Sprite>("Skill.Fir.01.sprite", pre, done, (res) => { Sprite = res; });
    }

    public static void Unload()
    {
      var rm = ResourceSystem.Instance;
      rm.Unload("Skill.Fir.01.sprite");
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // Component取得
      this.spriteRenderer = AddComponent<SpriteRenderer>();
      this.spriteRenderer.sortingLayerName = Define.Layer.Sorting.Effect;

      // 状態構築
      this.state.Add(State.Idle);
      this.state.Add(State.Create, OnCreateEnter, OnCreateUpdate);
      this.state.Add(State.Attack, OnAttackEnter, OnAttackUpdate, OnAttackExit);
      this.state.SetState(State.Idle);
    }

    protected override void OnMyEnable()
    {
      this.owner = this.target = null;
      this.basePosition = this.targetPosition = Vector3.zero;
    }

    //-------------------------------------------------------------------------
    // ISkill Interfaceの実装

    public override void Setup()
    {
      this.spriteRenderer.sprite = Sprite;
    }

    public override void Fire(Player owner, Player target)
    {
      base.Fire(owner, target);
      this.state.SetState(State.Create);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    //-------------------------------------------------------------------------
    // Creation
    // 猫の位置から猫の頭上あたりに爆弾が出現する

    private void OnCreateEnter()
    {
      CacheTransform.localScale = Vector3.zero;
      this.basePosition   = this.owner.Location.Cat;
      this.targetPosition = this.owner.Location.AttackBase;
      this.timer = 0;
    }

    private void OnCreateUpdate()
    {
      float rate = this.timer / CREATION_TIME;

      CacheTransform.position
        = Vector3.Lerp(basePosition, targetPosition, Tween.EaseOutElastic(rate));

      CacheTransform.localScale
        = MyVector3.Lerp(Vector3.zero, Vector3.one, Tween.EaseOutElastic(rate));

      UpdateTimer();

      if (CREATION_TIME < this.timer) {
        this.state.SetState(State.Attack);
      }
    }

    //-------------------------------------------------------------------------
    // Attack
    // 猫の頭上から相手のパズルに向かって爆弾が飛んでいく

    private void OnAttackEnter()
    {
      this.basePosition   = CacheTransform.position;
      this.targetPosition = this.target.Location.Center;
      this.timer = 0;
    }

    private void OnAttackUpdate()
    {
      float rate = this.timer / ATTACK_TIME;

      float x = MyMath.Lerp(basePosition.x, targetPosition.x, Tween.EaseOutSine(rate));
      float y = MyMath.Lerp(basePosition.y, targetPosition.y, Tween.EaseOutBack(rate));

      CacheTransform.position
        = new Vector3(x, y, 0);

      UpdateTimer();

      if (ATTACK_TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnAttackExit()
    {
      // 相手が無敵であればガードSEを鳴らす
      if (this.target.IsInvincible) {
        // TODO: ガードSEを再生
      }

      // 無敵じゃなければランダムダメージを与える
      else {
        // TODO: 爆発音SEを再生
        this.target.TakeDamage(Random.Range(MIN_DAMAGE, MAX_DAMAGE));
      }

      // エフェクト生成
      EffectManager.Instance.Create(EffectManager.Type.Spark)
        .Fire(CacheTransform.position);

      // スキルを返却
      SkillManager.Instance.Release(this);
    }
  }
}

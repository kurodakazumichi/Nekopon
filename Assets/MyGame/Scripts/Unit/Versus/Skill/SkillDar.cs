using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class SkillDar : SkillBase<SkillDar.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Haunted,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 幽霊エフェクト
    /// </summary>
    EffectManager.IEffect effect = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
    }

    public static void Unload()
    {
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Haunted, OnHauntedEnter, null, OnHauntedExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // ISkillの実装

    public override void Setup()
    {

    }

    public override void Fire(Player owner, Player target)
    {
      base.Fire(owner, target);
      this.state.SetState(State.Haunted);
    }

    //-------------------------------------------------------------------------
    // Haunted

    private void OnHauntedEnter()
    {
      // 幽霊エフェクトを生成
      this.effect = EffectManager.Instance.Create(EffectManager.Type.Ghost);

      // エフェクトのアクションに、幽霊スキル効果を発動する処理を設定
      this.effect.Action = () => { 
        this.target.Invisible();
        this.state.SetState(State.Idle);
      };

      // エフェクト発動
      this.effect.Fire(this.target.Location.Center);
    }

    private void OnHauntedExit()
    {
      this.effect.Action = null;
      this.effect = null;

      SkillManager.Instance.Release(this);
    }
  }
}

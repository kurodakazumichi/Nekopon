using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class SkillIce : SkillBase<SkillIce.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Circle,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 木の葉エフェクト
    /// </summary>
    IEffect effect = null;

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
      this.state.Add(State.Circle, OnCircleEnter, null, OnCircleExit);
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
      this.state.SetState(State.Circle);
    }

    //-------------------------------------------------------------------------
    // Spiral

    private void OnCircleEnter()
    {
      this.timer = 0;

      // 氷のエフェクトを生成
      this.effect = EffectManager.Instance.Create(EffectManager.Type.Ice);

      // エフェクトのアクションに、スキル効果を発動する処理を設定
      this.effect.Action = () =>
      {
        // パズルを凍結させる
        this.target.Freeze();
        this.state.SetState(State.Idle);
      };

      this.effect.Fire(this.target.Location.Center);
    }

    private void OnCircleExit()
    {
      this.effect.Action = null;
      this.effect = null;

      SkillManager.Instance.Release(this);
    }
  }

}

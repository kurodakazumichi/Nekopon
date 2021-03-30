using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class SkillTre : SkillBase<SkillTre.State>, SkillManager.ISkill
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Spiral,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 木の葉エフェクト
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
      this.state.Add(State.Spiral, OnSpiralEnter, null, OnSpiralExit);
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
      this.state.SetState(State.Spiral);
    }

    //-------------------------------------------------------------------------
    // Spiral

    private void OnSpiralEnter()
    {
      this.timer = 0;

      // 木の葉のエフェクトを生成
      this.effect = EffectManager.Instance.Create(EffectManager.Type.Leaves);

      // エフェクトのアクションに、スキル効果を発動する処理を設定
      this.effect.Action = () => 
      {
        // パズルをランダムに変更
        this.target.Randomize();
        this.state.SetState(State.Idle);
      };

      this.effect.Fire(this.target.Location.Center);
    }

    private void OnSpiralExit()
    {
      this.effect.Action = null;
      this.effect = null;

      SkillManager.Instance.Release(this);
    }
  }

}

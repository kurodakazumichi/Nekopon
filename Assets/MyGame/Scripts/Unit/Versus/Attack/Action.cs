using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public partial class Attack
  {
    /// <summary>
    /// 攻撃アクション、攻撃が当たった瞬間にExecuteが呼ばれる
    /// 相手プレイヤーに対してダメージを与える処理を行う
    /// </summary>
    public class Action : IAction
    {
      /// <summary>
      /// 攻撃ユニット
      /// </summary>
      private Attack attack;

      /// <summary>
      /// 攻撃する側(オーナー)
      /// </summary>
      private Player owner;

      /// <summary>
      /// 攻撃対象
      /// </summary>
      private Player target;

      /// <summary>
      /// コンストラクタ
      /// </summary>
      public Action(Attack attack, Player owner, Player target)
      {
        this.attack = attack;
        this.owner  = owner;
        this.target = target;
      }

      /// <summary>
      /// オーナーとターゲットを入れ替え
      /// </summary>
      private void Swap()
      {
        var self    = this.owner;
        this.owner  = this.target;
        this.target = self;
      }

      /// <summary>
      /// 攻撃を実行
      /// </summary>
      public void Execute()
      {
        // 相手が反射可能であれば、攻撃対象を入れ替え再攻撃
        if (target.CanReflect) {
          Swap();
          attack.ToAttack(this.target.Location.Top, this);
        } 
        
        // 相手が反射不可であれば、そのまま相手に攻撃を与える
        else {
          target.TakeAttack(owner);
          SkillManager.Instance.Release(attack);
        }
      }
    }

  }
}
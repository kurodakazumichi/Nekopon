using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public partial class Attack : Unit<Attack.State>
  {
    /// <summary>
    /// 攻撃アクション、攻撃が当たった瞬間にExecuteが呼ばれる
    /// 相手プレイヤーに対してダメージを与える処理を行う
    /// </summary>
    public class Action : IAction
    {
      private Attack attack;
      private Player self;
      private Player target;

      public Action(Attack attack, Player self, Player target)
      {
        this.attack = attack;
        this.self   = self;
        this.target = target;
      }

      private void Swap()
      {
        var self    = this.self;
        this.self   = this.target;
        this.target = self;
      }

      public void Execute()
      {
        if (target.CanReflect) {
          Swap();
          attack.ToAttack(this.self.Location.TargetBase, this);
        } else {
          target.TakeAttack(self);
          attack.ToIdle();
        }
      }
    }

  }
}
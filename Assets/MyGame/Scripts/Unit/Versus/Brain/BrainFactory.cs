using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;
namespace MyGame.Unit.Versus
{
  public static class BrainFactory
  {
    /// <summary>
    /// Brainを生成する
    /// </summary>
    public static IBrain Create(App.Brain brainType, Player owner, Player target)
    {
      // Brain種別によって生成するBrainを分岐
      switch(brainType) 
      {
        // AIの場合は猫タイプによって生成
        case App.Brain.AI: {
          return CreateAIBrain(owner, target);
        }
        
        // AI以外だったらプレイヤー操作用のBrainを生成
        default: {
          return CreatePlayerBrain(owner);
        }
      }
    }

    /// <summary>
    /// プレイヤー種別によって脳を生成
    /// </summary>
    private static IBrain CreatePlayerBrain(Player owner)
    {
      return new BrainPlayer(owner);
    }

    /// <summary>
    /// 猫の種類によって脳を生成
    /// </summary>
    private static IBrain CreateAIBrain(Player owner, Player target)
    {
      var props = new BrainAI.Props() { 
        owner  = owner,
        target = target,
      };

      return new BrainAI(props);
    }
  }

}
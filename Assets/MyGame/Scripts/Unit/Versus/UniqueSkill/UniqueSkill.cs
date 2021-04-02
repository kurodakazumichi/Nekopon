using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class UniqueSkill : Unit<UniqueSkill.State>, IUniqueSkill
  {
    public enum State
    {
      Idle,
      Cutin,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// Spriteリソース
    /// </summary>
    public static Dictionary<int, Sprite> Sprites = new Dictionary<int, Sprite>();

    public static void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceSystem.Instance;

      MyEnum.ForEach<Define.App.Cat>((type) => { 
        rm.Load<Sprite>($"CutIn.{type}.sprite", pre, done, (res) => { 
          Sprites.Add((int)type, res); 
        });
      });
    }

    public static void Unload()
    {
      var rm = ResourceSystem.Instance;

      MyEnum.ForEach<Define.App.Cat>((type) => { 
        rm.Unload($"CutIn.{type}.sprite");
      });
    }

    //-------------------------------------------------------------------------
    // IUniqueSkill Interfaceの実装

    public void Setup(Define.App.UniqueSkill skillType, Define.App.Cat catType)
    {

    }

    public void Fire(Player owner, Player target)
    {

    }
  }

}


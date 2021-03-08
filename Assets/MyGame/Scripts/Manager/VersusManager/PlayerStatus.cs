using MyGame.Define;
using MyGame.SaveManagement;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.VersusManagement
{
  /// <summary>
  /// Plyaer Statusクラス
  /// </summary>
  public class PlayerStatus
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// HP
    /// </summary>
    public LimitedFloat Hp { get; private set; } = new LimitedFloat();

    /// <summary>
    /// MP
    /// </summary>
    private Dictionary<App.Attribute, LimitedFloat> mp = new Dictionary<App.Attribute, LimitedFloat>();

    //-------------------------------------------------------------------------
    // publicメソッド

    /// <summary>
    /// MP
    /// </summary>
    public LimitedFloat Mp(App.Attribute attribute)
    {
      return this.mp[attribute];
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlayerStatus()
    {
      MyEnum.ForEach<App.Attribute>((attribute) => {
        this.mp.Add(attribute, new LimitedFloat());
      });
    }

    /// <summary>
    /// 設定を元にStatusを構築
    /// </summary>
    public PlayerStatus Setup(IPlayerConfig config)
    {
      // HP
      this.Hp.Setup(config.MaxHp, config.MaxHp);

      // MP
      MyEnum.ForEach<App.Attribute>((attribute) => 
      {  
        int max = config.GetMaxMp(attribute);
        Mp(attribute).Setup(max, max);
      });

      return this;
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    public void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) 
      {
        GUILayout.Label($"HP:{Hp.Now}");

        Util.ForEach(this.mp, (attr, mp) => {
          GUILayout.Label($"MP_{attr}:{mp.Now}");
        });
      }
    }
#endif
  }
}
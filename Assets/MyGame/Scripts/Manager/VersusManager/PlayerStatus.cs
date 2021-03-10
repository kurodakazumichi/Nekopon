﻿using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;

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

    /// <summary>
    /// AP
    /// </summary>
    public LimitedFloat Ap { get; private set; } = new LimitedFloat();

    /// <summary>
    /// ダメージ
    /// </summary>
    public LimitedFloat Dp { get; private set; } = new LimitedFloat();

    //-------------------------------------------------------------------------
    // publicメソッド

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

      // DP
      this.Dp.Setup(0, config.MaxHp);

      // AP
      this.Ap.Setup(0, config.MaxAp);

      return this;
    }

    /// <summary>
    /// MP
    /// </summary>
    public LimitedFloat Mp(App.Attribute attribute)
    {
      return this.mp[attribute];
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

        GUILayout.Label($"AP:{Ap.Now}");
      }
    }
#endif
  }
}
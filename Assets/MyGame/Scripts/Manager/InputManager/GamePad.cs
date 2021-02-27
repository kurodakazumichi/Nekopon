using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame.InputManagement
{

  public class GamePad
  {
    /// <summary>
    /// パッドに存在する軸のテーブル
    /// </summary>
    private Dictionary<AxisType, Axis> axes = new Dictionary<AxisType, Axis>();

    /// <summary>
    /// パッドに存在するボタンのテーブル
    /// </summary>
    private Dictionary<ButtonType, Button> buttons = new Dictionary<ButtonType, Button>();

    public bool IsConnectedPad { get; private set; } = false;



    public GamePad(int padNo)
    {
      // 接続されているパッドがあるかどうかのフラグ
      IsConnectedPad = padNo <= (Input.GetJoystickNames().Length - 1);

      // 軸のセットアップ
      this
        .SetupAxis(padNo, AxisType.LX  , 1 , false)
        .SetupAxis(padNo, AxisType.LY  , 2 , true)
        .SetupAxis(padNo, AxisType.RX  , 4 , false)
        .SetupAxis(padNo, AxisType.RY  , 5 , true)
        .SetupAxis(padNo, AxisType.DX  , 6 , false)
        .SetupAxis(padNo, AxisType.DY  , 7 , false)
        .SetupAxis(padNo, AxisType.LR, 3 , false)
        .SetupAxis(padNo, AxisType.LT      , 9 , false)
        .SetupAxis(padNo, AxisType.RT      , 10, false);

      // ボタンのセットアップ
      this
        .SetupButton(padNo, ButtonType.A, 0)
        .SetupButton(padNo, ButtonType.B, 1)
        .SetupButton(padNo, ButtonType.X, 2)
        .SetupButton(padNo, ButtonType.Y, 3)
        .SetupButton(padNo, ButtonType.L1, 4)
        .SetupButton(padNo, ButtonType.R1, 5)
        .SetupButton(padNo, ButtonType.LS, 8)
        .SetupButton(padNo, ButtonType.RS, 9)
        .SetupButton(padNo, ButtonType.Back, 6)
        .SetupButton(padNo, ButtonType.Start, 7);
    }

    public void Update()
    {
      foreach(KeyValuePair<AxisType, Axis> axis in this.axes) {
        axis.Value.Update();
      }
      Util.ForEach(this.axes, (type, axis) => { axis.Update(); });
      Util.ForEach(this.buttons, (type, button) => { button.Update(); });
    }

    private GamePad SetupAxis(int padNo, AxisType type, int no, bool invert)
    {
      this.axes[type] = new Axis(type, $"Joy{padNo + 1}_Axis{no}", invert);
      return this;
    }

    private GamePad SetupButton(int padNo, ButtonType type, int no)
    {
      this.buttons[type] = new Button(type, $"Joy{padNo + 1}_Button{no}");
      return this;
    }
    
#if _DEBUG
    public void OnGUIDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) {
        Util.ForEach<AxisType>((value) => { 
          this.axes[value].OnGUIDebug();
        });
      }

      using (new GUILayout.VerticalScope(GUI.skin.box)) {
        Util.ForEach<ButtonType>((value) => {
          this.buttons[value].OnGUIDebug();
        });
      }
    }
#endif
  }

}

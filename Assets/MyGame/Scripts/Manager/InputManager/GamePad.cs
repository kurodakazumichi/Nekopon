using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyGame.SaveManagement;

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

    /// <summary>
    /// 利用するキーボードのキーテーブル
    /// </summary>
    private Dictionary<KeyType, Key> keys = new Dictionary<KeyType, Key>();

    /// <summary>
    /// 接続されているパッドがあるかどうかのフラグ
    /// </summary>
    public bool IsConnectedPad { get; private set; } = false;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GamePad(int padNo)
    {
      // 接続されているパッドがあるかどうかのフラグ
      IsConnectedPad = padNo <= (Input.GetJoystickNames().Length - 1);

      // 軸のセットアップ
      this
        .SetupAxis(padNo, AxisType.LX, 1, false)
        .SetupAxis(padNo, AxisType.LY, 2, true)
        .SetupAxis(padNo, AxisType.RX, 4, false)
        .SetupAxis(padNo, AxisType.RY, 5, true)
        .SetupAxis(padNo, AxisType.DX, 6, false)
        .SetupAxis(padNo, AxisType.DY, 7, false)
        .SetupAxis(padNo, AxisType.LR, 3, false)
        .SetupAxis(padNo, AxisType.LT, 9, false)
        .SetupAxis(padNo, AxisType.RT, 10, false);

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

      // キーのセットアップ
      this
        .SetupKey(KeyType.A, KeyCode.Z)
        .SetupKey(KeyType.B, KeyCode.X)
        .SetupKey(KeyType.X, KeyCode.A)
        .SetupKey(KeyType.Y, KeyCode.S)
        .SetupKey(KeyType.L1, KeyCode.LeftShift)
        .SetupKey(KeyType.R1, KeyCode.LeftControl)
        .SetupKey(KeyType.U, KeyCode.UpArrow)
        .SetupKey(KeyType.D, KeyCode.DownArrow)
        .SetupKey(KeyType.L, KeyCode.LeftArrow)
        .SetupKey(KeyType.R, KeyCode.RightArrow);
    }

    /// <summary>
    /// セットアップ(KeyConfigを元に設定をする
    /// </summary>
    public void Setup(KeyConfig config)
    {
      config.Gets((type, code) => { 
        SetupKey(type, code);
      });
    }

    /// <summary>
    /// パッドの入力状態を更新
    /// </summary>
    public void Update()
    {
      // 接続されているパッドがある場合は、パッド入力を更新
      if (IsConnectedPad) {
        Util.ForEach(this.axes, (type, axis) => { axis.Update(); });
        Util.ForEach(this.buttons, (type, button) => { button.Update(); });
      }

      // キーボード入力の状態を更新
      Util.ForEach(this.keys, (type, key) => { key.Update(); });

      // キーボード入力をAxisにマージ
      this
        .MergeKeyToAxis(KeyType.L, AxisType.DX, -1.0f)
        .MergeKeyToAxis(KeyType.R, AxisType.DX, 1.0f)
        .MergeKeyToAxis(KeyType.U, AxisType.DY, 1.0f)
        .MergeKeyToAxis(KeyType.D, AxisType.DY, -1.0f);

      // キーボード入力をButtonにマージ
      this
        .MergeKeyToButton(KeyType.A, ButtonType.A)
        .MergeKeyToButton(KeyType.B, ButtonType.B)
        .MergeKeyToButton(KeyType.X, ButtonType.X)
        .MergeKeyToButton(KeyType.Y, ButtonType.Y)
        .MergeKeyToButton(KeyType.L1, ButtonType.L1)
        .MergeKeyToButton(KeyType.R1, ButtonType.R1);
    }

    /// <summary>
    /// 軸の入力値を取得
    /// </summary>
    public float GetAxis(AxisType type)
    {
      if (this.axes.TryGetValue(type, out Axis axis)) return axis.Value;
      return 0;
    }

    /// <summary>
    /// 軸が押されたかどうか
    /// </summary>
    public bool GetAxisDown(AxisType type)
    {
      if (this.axes.TryGetValue(type, out Axis axis)) return axis.IsDown;
      return false;
    }

    /// <summary>
    /// 軸が離されたかどうか
    /// </summary>
    public bool GetAxisUp(AxisType type)
    {
      if (this.axes.TryGetValue(type, out Axis axis)) return axis.IsUp; 
      return false;
    }

    /// <summary>
    /// 軸が押されているかどうか
    /// </summary>
    public bool GetAxisHold(AxisType type)
    {
      if (this.axes.TryGetValue(type, out Axis axis)) return axis.IsHold;
      return false;
    }

    /// <summary>
    /// 軸の押されている時間(秒)
    /// </summary>
    public float GetAxisTime(AxisType type)
    {
      if (this.axes.TryGetValue(type, out Axis axis)) return axis.Time;
      return 0;
    }

    /// <summary>
    /// ボタンが押されているかどうか
    /// </summary>
    public bool GetButton(ButtonType type)
    {
      if (this.buttons.TryGetValue(type, out Button button)) return button.IsHold;
      return false;
    }

    /// <summary>
    /// ボタンが押されたかどうか
    /// </summary>
    public bool GetButtonDown(ButtonType type)
    {
      if (this.buttons.TryGetValue(type, out Button button)) return button.IsDown;
      return false;
    }

    /// <summary>
    /// ボタンが離されたかどうか
    /// </summary>
    public bool GetButtonUp(ButtonType type)
    {
      if (this.buttons.TryGetValue(type, out Button button)) return button.IsUp;
      return false;
    }

    /// <summary>
    /// ボタンが押されているかどうか
    /// </summary>
    public bool GetButtonHold(ButtonType type)
    {
      if (this.buttons.TryGetValue(type, out Button button)) return button.IsHold;
      return false;
    }

    /// <summary>
    /// ボタンが押されている時間(秒)
    /// </summary>
    public float GetButtonTime(ButtonType type)
    {
      if (this.buttons.TryGetValue(type, out Button button)) return button.Time;
      return 0;
    }

    /// <summary>
    /// 何かしら押されたボタンを返す
    /// </summary>
    public Button GetPressedButton()
    {
      Button pressed = null;
      Util.ForEach(this.buttons, (type, button) => 
      {
        if (button.IsHold) { 
          pressed = button;
          return true; 
        } 
        return false;
      });

      return pressed;
    }

    /// <summary>
    /// キーボードの入力があった場合に、入力内容をボタン入力にマージする
    /// </summary>
    private GamePad MergeKeyToButton(KeyType keyType, ButtonType buttonType)
    {
      Key    key;
      Button button;

      if(!this.keys.TryGetValue(keyType, out key)) return this;
      if(!this.buttons.TryGetValue(buttonType, out button)) return this;

      if (key.HasInput) {
        button.IsDown = key.IsDown;
        button.IsUp   = key.IsUp;
        button.Time   = key.Time;
      }

      return this;
    }

    /// <summary>
    /// キーボードの入力があった場合に、入力内容を軸入力にマージする
    /// </summary>
    private GamePad MergeKeyToAxis(KeyType keyType, AxisType axisType, float value)
    {
      Key key;
      Axis axis;

      if (!this.keys.TryGetValue(keyType, out key)) return this;
      if (!this.axes.TryGetValue(axisType, out axis)) return this;

      if (key.HasInput) {
        axis.IsDown = key.IsDown;
        axis.IsUp   = key.IsUp;
        axis.Time   = key.Time;
        axis.Value  = value;
      }

      return this;
    }

    /// <summary>
    /// 軸インスタンスを生成し、軸の設定をする。
    /// </summary>
    private GamePad SetupAxis(int padNo, AxisType type, int no, bool invert)
    {
      this.axes[type] = new Axis(type, $"Joy{padNo + 1}_Axis{no}", invert);
      return this;
    }

    /// <summary>
    /// ボタンインスタンスを生成し、ボタンの設定をする。
    /// </summary>
    private GamePad SetupButton(int padNo, ButtonType type, int no)
    {
      this.buttons[type] = new Button(type, $"Joy{padNo + 1}_Button{no}");
      return this;
    }

    /// <summary>
    /// キーボードのキーインスタンスを生成し、ボタンの設定をする。
    /// </summary>
    private GamePad SetupKey(KeyType type, KeyCode code)
    {
      if (this.keys.TryGetValue(type, out Key key)) {
        key.Setup(type, code);
      } else {
        this.keys[type] = new Key(type, code);
      }
      
      return this;
    }
    
#if _DEBUG
    public void OnDebug()
    {
      using (new GUILayout.VerticalScope()) 
      {
        using (new GUILayout.VerticalScope(GUI.skin.box)) {
          GUILayout.Label("■ Axis");
          Util.ForEach(this.axes, (type, axis) => { axis.OnDebug(); });
        }

        using (new GUILayout.VerticalScope(GUI.skin.box)) {
          GUILayout.Label("■ Button");
          Util.ForEach(this.buttons, (type, button) => { button.OnDebug(); });
        }

        using (new GUILayout.VerticalScope(GUI.skin.box)) {
          GUILayout.Label("■ Key");
          Util.ForEach(this.keys, (type, key) => { key.OnDebug(); });
        }
      }
    }
#endif
  }

}

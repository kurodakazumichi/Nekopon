using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;

namespace MyGame.InputManagement
{

  public class GamePad
  {
    //-------------------------------------------------------------------------
    // メンバ変数

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

    //-------------------------------------------------------------------------
    // ライフサイクル

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GamePad()
    {
    }

    //-------------------------------------------------------------------------
    // コントローラーの設定に関する部分

    public void Init(JoyConfig config, int padNo)
    {
      // 接続されているパッドがあるかどうかのフラグ
      IsConnectedPad = padNo <= (Input.GetJoystickNames().Length - 1);

      // 対応するパッドがない,、configがnullなら何もしない
      if (!IsConnectedPad) return;
      if (!config) return;

      // 軸の設定を作成
      config.GetAxes((type, no, invert) => { 
        if (0 <= no) InitAxis(padNo, type, no, invert);
      });

      // ボタンの設定を作成
      config.GetButtons((type, no) => { 
        if (0 <= no) InitButton(padNo, type, no);
      });
    }

    /// <summary>
    /// セットアップ(KeyConfigを元に設定をする
    /// </summary>
    public void Init(KeyConfig config)
    {
      if (!config) return;

      // キーボード入力の設定
      config.Gets((type, code) => { 
        if (code != KeyCode.None) InitKey(type, code);
      });
    }

    /// <summary>
    /// 軸インスタンスを生成し、軸の設定をする。
    /// </summary>
    private GamePad InitAxis(int padNo, AxisType type, int no, bool invert)
    {
      var name = $"Joy{padNo + 1}_Axis{no}";

      if (this.axes.TryGetValue(type, out Axis axis)) {
        axis.Init(type, name, invert);
      } else {
        this.axes[type] = new Axis(type, name, invert);
      }
      
      return this;
    }

    /// <summary>
    /// ボタンインスタンスを生成し、ボタンの設定をする。
    /// </summary>
    private GamePad InitButton(int padNo, ButtonType type, int no)
    {
      var name = $"Joy{padNo + 1}_Button{no}";

      if (this.buttons.TryGetValue(type, out Button button)) {
        button.Init(type, name);
      } else {
        this.buttons[type] = new Button(type, name);
      }
      
      return this;
    }

    /// <summary>
    /// キーボードのキーインスタンスを生成し、ボタンの設定をする。
    /// </summary>
    private GamePad InitKey(KeyType type, KeyCode code)
    {
      if (this.keys.TryGetValue(type, out Key key)) {
        key.Init(type, code);
      } else {
        this.keys[type] = new Key(type, code);
      }

      return this;
    }

    //-------------------------------------------------------------------------
    // コントローラーの入力を受け取る部分

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
      
      // キーボード入力をJoy入力にマージ
      Merge();
    }

    /// <summary>
    /// キーボードの入力をAxisやButtonの入力としてマージする
    /// </summary>
    private void Merge()
    {
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
        .MergeKeyToButton(KeyType.R1, ButtonType.R1)
        .MergeKeyToButton(KeyType.Start, ButtonType.Start)
        .MergeKeyToButton(KeyType.Back, ButtonType.Back);
    }

    /// <summary>
    /// キーボードの入力があった場合に、入力内容をボタン入力にマージする
    /// </summary>
    private GamePad MergeKeyToButton(KeyType keyType, ButtonType buttonType)
    {
      Key    key;
      Button button;

      if (!this.keys.TryGetValue(keyType, out key)) return this;
      if (!this.buttons.TryGetValue(buttonType, out button)) return this;

      if (key.HasInput) {
        button.IsDown = key.IsDown;
        button.IsUp = key.IsUp;
        button.Time = key.Time;
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
        axis.IsUp = key.IsUp;
        axis.Time = key.Time;
        axis.Value = value;
      }

      return this;
    }

    //-------------------------------------------------------------------------
    // 入力取得関係

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

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

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

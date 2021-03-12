using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;
using MyGame.InputManagement;

namespace MyGame
{
  /// <summary>
  /// 入力管理
  /// </summary>
  public class InputSystem : SingletonMonoBehaviour<InputSystem>
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// ゲームパッド
    /// </summary>
    private List<GamePad> pads = new List<GamePad>();

    /// <summary>
    /// コマンド
    /// </summary>
    private Dictionary<Command, CommandBase> commands = new Dictionary<Command, CommandBase>();

    /// <summary>
    /// パッド数
    /// </summary>
    public int PadCount => this.pads.Count;

    /// <summary>
    /// 接続されているパッド数
    /// </summary>
    public int ConnectedPadCount { get; private set; }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyStart()
    {
      Debug.Manager.Instance.Regist(this);
      this.ConnectedPadCount = Input.GetJoystickNames().Length;

      this.pads.Add(new GamePad());
      this.pads.Add(new GamePad());
      this.commands[Command.Move] = new MoveCommand();
      this.commands[Command.Decide] = new DecideCommand();
      this.commands[Command.PressAnyButton] = new PressAnyButton();
    }

    protected override void MyUpdate()
    {
      this.pads.ForEach((pad) => { pad.Update(); });
    }

    protected override void OnMyDestory()
    {
      Debug.Manager.Instance.Discard(this);
    }

    //-------------------------------------------------------------------------
    // ゲームパッド関連

    /// <summary>
    /// ゲームパッドを取得する
    /// </summary>
    /// <param name="padNo"></param>
    /// <returns></returns>
    public GamePad GetPad(int padNo)
    {
      return Util.TryGet(this.pads, padNo);
    }

    /// <summary>
    /// Joystickの名前を取得する
    /// </summary>
    public string GetJoyName(int no)
    {
      return Util.TryGet(Input.GetJoystickNames(), no, "");
    }

    /// <summary>
    /// Joystickの名前からJoyPadの種類を取得する
    /// </summary>
    public App.JoyType GetJoyType(string name)
    {
      switch(name) {
        case "Wireless Controller": return App.JoyType.PS4;
        default  : return App.JoyType.X360;
      }
    }

    /// <summary>
    /// パッド番号からJoyTypeの種類を取得する
    /// </summary>
    public App.JoyType GetJoyType(int no)
    {
      return GetJoyType(GetJoyName(no));
    }

    //-------------------------------------------------------------------------
    // コマンド関連

    public ICommand GetCommand(Command type, int padNo)
    {
      padNo = (padNo < PadCount)? padNo : 0;
      this.commands[type].Execute(GetPad(padNo));
      return this.commands[type];
    }

#if _DEBUG

    //-------------------------------------------------------------------------
    // デバッグ

    private int __selectedPadIndex = 0;

    public override void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) 
      {
        using (new GUILayout.HorizontalScope()) {
          GUILayout.Label($"■ PadCount:{PadCount}");
          GUILayout.Label($"■ ConnectedPadCount:{ConnectedPadCount}");
        }

        string[] padNames = { "pad1", "pad2" };
        __selectedPadIndex = GUILayout.SelectionGrid(__selectedPadIndex, padNames, 2);
        this.pads[__selectedPadIndex].OnDebug();
      }
    }
#endif
  }

}

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
    private Dictionary<int, CommandBase> commands = new Dictionary<int, CommandBase>();

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
      DebugSystem.Instance.Regist(this);

      // 接続されているゲームパッドの数を保存
      this.ConnectedPadCount = Input.GetJoystickNames().Length;

      // ゲームパッドを生成
      this.pads.Add(new GamePad());
      this.pads.Add(new GamePad());

      // コマンド登録
      this.commands[(int)Command.Move]            = new MoveCommand();
      this.commands[(int)Command.Decide]          = new DecideCommand();
      this.commands[(int)Command.Cancel]          = new CancelCommand();
      this.commands[(int)Command.Chain]           = new ChainCommand();
      this.commands[(int)Command.ShowSkillGuide]  = new ShowSkillGuideCommand();
      this.commands[(int)Command.HideSkillGuide]  = new HideSkillGuideCommand();
      this.commands[(int)Command.FireSkillFir]    = new FireSkillFirCommand();
      this.commands[(int)Command.FireSkillWat]    = new FireSkillWatCommand();
      this.commands[(int)Command.FireSkillThu]    = new FireSkillThrCommand();
      this.commands[(int)Command.FireSkillIce]    = new FireSkillIceCommand();
      this.commands[(int)Command.FireSkillTre]    = new FireSkillTreCommand();
      this.commands[(int)Command.FireSkillHol]    = new FireSkillHolCommand();
      this.commands[(int)Command.FireSkillDar]    = new FireSkillDarCommand();
      this.commands[(int)Command.FireUniqueSkill] = new FireUniqueSkillCommand();
      this.commands[(int)Command.PressAnyButton]  = new PressAnyButton();
    }

    protected override void MyUpdate()
    {
      this.pads.ForEach((pad) => { pad.Update(); });
    }

    protected override void OnMyDestory()
    {
      DebugSystem.Instance.Discard(this);
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
      this.commands[(int)type].Execute(GetPad(padNo));
      return this.commands[(int)type];
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

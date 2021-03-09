using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;

namespace MyGame
{
  public class InputManager : SingletonMonoBehaviour<InputManager>
  {
    private List<GamePad> pads = new List<GamePad>();

    private Dictionary<Command, CommandBase> commands = new Dictionary<Command, CommandBase>();

    public int PadCount => this.pads.Count;

    protected override void MyStart()
    {
      Debug.Manager.Instance.Regist(this);

      this.pads.Add(new GamePad(0));
      this.commands[Command.Move] = new MoveCommand();
      this.commands[Command.Decide] = new DecideCommand();
      this.commands[Command.PressAnyButton] = new PressAnyButton();
    }

    public GamePad GetPad(int padNo)
    {
      padNo = Mathf.Max(0, padNo);
      padNo = (padNo < PadCount)? padNo : 0;
      return this.pads[padNo];
    }

    public ICommand GetCommand(Command type, int padNo)
    {
      padNo = (padNo < PadCount)? padNo : 0;
      this.commands[type].Execute(GetPad(padNo));
      return this.commands[type];
    }
    
    protected override void MyUpdate()
    {
      this.pads.ForEach((pad) => { pad.Update(); });
    }

    protected override void OnMyDestory()
    {
      Debug.Manager.Instance.Discard(this);
    }

#if _DEBUG
    public override void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) 
      {
        GUILayout.Label($"■PadCount:{PadCount}");

        this.pads[0].OnDebug();
      }
    }
#endif
  }

}

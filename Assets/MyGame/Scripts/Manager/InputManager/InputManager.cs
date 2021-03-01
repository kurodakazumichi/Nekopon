using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.InputManagement;

namespace MyGame
{
  public class InputManager : SingletonMonobehaviour<InputManager>
  {
    private List<GamePad> pads = new List<GamePad>();

    private Dictionary<Command, CommandBase> commands = new Dictionary<Command, CommandBase>();

    public int PadCount => this.pads.Count;

    protected override void MyStart()
    {
      this.pads.Add(new GamePad(0));
      this.commands[Command.Move] = new MoveCommand();
      this.commands[Command.Decide] = new DecideCommand();
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

#if _DEBUG
    private void OnGUI()
    {
      //this.pads[0].OnGUIDebug();
    }
#endif
  }

}

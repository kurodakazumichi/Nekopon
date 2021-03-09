using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;

namespace MyGame.Define
{
  /// <summary>
  /// ゲーム内で利用するコマンドの一覧を定義
  /// </summary>
  public enum Command
  {
    Move,
    Decide,
    PressAnyButton,
  }

  /// <summary>
  /// 
  /// </summary>
  public interface ICommand
  {
    Vector3 Axis {
      get;
    }

    bool IsFixed {
      get;
    }
  }

  public abstract class CommandBase: ICommand
  { 
    public virtual Vector3 Axis { get; protected set; } = Vector3.zero;

    public virtual bool IsFixed { get; protected set; } = false;
    public abstract void Execute(GamePad pad);
  }

  public class MoveCommand : CommandBase, ICommand
  {
    public override Vector3 Axis => this.value;
    public override bool IsFixed => (this.value.sqrMagnitude != 0); 

    private Vector3 value;

    public override void Execute(GamePad pad)
    {
      this.value = Vector3.zero;
      if (pad == null) return;

      this.value.x = pad.GetAxis(AxisType.LX);
      this.value.y = pad.GetAxis(AxisType.LY);

      float x = pad.GetAxis(AxisType.DX);
      float y = pad.GetAxis(AxisType.DY);

      if (Mathf.Abs(this.value.x) < Mathf.Abs(x)) this.value.x = x;
      if (Mathf.Abs(this.value.y) < Mathf.Abs(y)) this.value.y = y;
    }
  }

  public class DecideCommand: CommandBase, ICommand
  {
    
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonDown(ButtonType.A);
    }
  }

  public class PressAnyButton: CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      this.IsFixed = (pad.GetPressedButton() != null);
    }
  }
}
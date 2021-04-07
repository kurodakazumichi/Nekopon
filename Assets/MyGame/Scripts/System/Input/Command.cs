using UnityEngine;
using MyGame.Define;

namespace MyGame.InputManagement
{
  /// <summary>
  /// ゲーム内で利用するコマンドの一覧を定義
  /// </summary>
  public enum Command
  {
    Move,           // 方向入力
    Decide,         // 決定
    PressAnyButton, // 何かしらのキーを押した
  }

  /// <summary>
  /// コマンドのインターフェース
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

  /// <summary>
  /// コマンドベース
  /// </summary>
  public abstract class CommandBase: ICommand
  { 
    /// <summary>
    /// 軸
    /// </summary>
    public virtual Vector3 Axis { get; protected set; } = Vector3.zero;

    /// <summary>
    /// コマンドが確定しているかどうか
    /// </summary>
    public virtual bool IsFixed { get; protected set; } = false;

    /// <summary>
    /// コマンドが成立するかどうかを判定する
    /// </summary>
    public abstract void Execute(GamePad pad);
  }

  /// <summary>
  /// 移動コマンド
  /// 左スティック、十字キーの入力を統合
  /// </summary>
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

  /// <summary>
  /// 決定コマンド
  /// </summary>
  public class DecideCommand: CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonDown(ButtonType.A);
    }
  }

  /// <summary>
  /// キャンセルコマンド
  /// </summary>
  public class CancelCommand: CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonDown(ButtonType.B);
    }
  }

  /// <summary>
  /// 連鎖コマンド
  /// </summary>
  public class ChainCommand: CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonDown(ButtonType.Y);
    }
  }

  /// <summary>
  /// 何かしらキーが押された
  /// </summary>
  public class PressAnyButton: CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      this.IsFixed = (pad.GetPressedButton() != null);
    }
  }
}
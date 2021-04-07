using UnityEngine;
using MyGame.Define;

namespace MyGame.InputManagement
{
  /// <summary>
  /// ゲーム内で利用するコマンドの一覧を定義
  /// </summary>
  public enum Command
  {
    Move,            // 方向入力
    Decide,          // 決定
    Cancel,          // キャンセル
    Chain,           // 連鎖
    ShowSkillGuide,  // スキルガイドを表示
    FireSkillFir,    // 属性スキル(火)
    FireSkillWat,    // 属性スキル(水)
    FireSkillThu,    // 属性スキル(雷)
    FireSkillIce,    // 属性スキル(氷)
    FireSkillTre,    // 属性スキル(木)
    FireSkillHol,    // 属性スキル(聖)
    FireSkillDar,    // 属性スキル(闇)
    FireUniqueSkill, // 固有スキル
    PressAnyButton,  // 何かしらのキーを押した
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
  /// スキルガイド表示コマンド
  /// </summary>
  public class ShowSkillGuideCommand: CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonHold(ButtonType.L1);
    }
  }

  /// <summary>
  /// 属性スキル(火)
  /// </summary>
  public class FireSkillFirCommand: CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonHold(ButtonType.L1) && pad.GetButtonDown(ButtonType.Y);
    }
  }

  /// <summary>
  /// 属性スキル(水)
  /// </summary>
  public class FireSkillWatCommand : CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonHold(ButtonType.L1) && pad.GetButtonDown(ButtonType.B);
    }
  }

  /// <summary>
  /// 属性スキル(雷)
  /// </summary>
  public class FireSkillThrCommand : CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonHold(ButtonType.L1) && pad.GetButtonDown(ButtonType.A);
    }
  }

  /// <summary>
  /// 属性スキル(氷)
  /// </summary>
  public class FireSkillIceCommand : CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonHold(ButtonType.L1) && pad.GetButtonDown(ButtonType.X);
    }
  }

  /// <summary>
  /// 属性スキル(木)
  /// </summary>
  public class FireSkillTreCommand : CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonHold(ButtonType.L1) && pad.GetAxisDown(AxisType.DX);
    }
  }

  /// <summary>
  /// 属性スキル(聖)
  /// </summary>
  public class FireSkillHolCommand : CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      var L1 = pad.GetButtonHold(ButtonType.L1);
      var DU = pad.GetAxisDown(AxisType.DY) && 0 < pad.GetAxis(AxisType.DY);
      IsFixed = L1 && DU;
    }
  }

  /// <summary>
  /// 属性スキル(闇)
  /// </summary>
  public class FireSkillDarCommand : CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      var L1 = pad.GetButtonHold(ButtonType.L1);
      var DD = pad.GetAxisDown(AxisType.DY) && pad.GetAxis(AxisType.DY) < 0;
      IsFixed = L1 && DD;
    }
  }

  /// <summary>
  /// 固有スキル発動コマンド
  /// </summary>
  public class FireUniqueSkillCommand: CommandBase, ICommand
  {
    public override void Execute(GamePad pad)
    {
      IsFixed = pad.GetButtonDown(ButtonType.R1);
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
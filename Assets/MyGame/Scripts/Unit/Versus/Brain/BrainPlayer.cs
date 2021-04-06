using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class MoveCursorAction : IAction
  {
    /// <summary>
    /// プレイヤー
    /// </summary>
    private Player player = null;

    /// <summary>
    /// 方向
    /// </summary>
    private Vector3 direction = Vector3.zero;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MoveCursorAction(Player player, Vector3 direction)
    {
      this.player = player;
      this.direction = direction;
    }

    public void Execute()
    {
      if (this.direction.x <= -1) {
        this.player.MoveCursor(Define.App.Direction.L);
      }

      if (1f <= this.direction.x) {
        this.player.MoveCursor(Define.App.Direction.R);
      }

      if (this.direction.y <= -1) {
        this.player.MoveCursor(Define.App.Direction.D);
      }

      if (1f <= this.direction.y) {
        this.player.MoveCursor(Define.App.Direction.U);
      }
    }
  }

  public class BrainPlayer : IBrain
  {
    /// <summary>
    /// ゲームパッド番号
    /// </summary>
    private int padNo = 0;

    /// <summary>
    /// プレイヤー
    /// </summary>
    private Player owner = null;

    /// <summary>
    /// 決定された行動
    /// </summary>
    private IAction decidedAction = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public BrainPlayer(Player owner)
    {
      this.owner = owner;
      this.padNo = (owner.Type == Define.App.Player.P1)? 0 : 1;
    }

    /// <summary>
    /// 思考
    /// </summary>
    public IAction Think()
    {
      this.decidedAction = null;


      MonitorMoveCursor();

      
      return this.decidedAction;
    }

    private void MonitorMoveCursor()
    {
      if (this.decidedAction != null) return;

      var com = InputSystem.Instance.GetCommand(InputManagement.Command.Move, this.padNo);

      if (com.IsFixed) {
        this.decidedAction = new MoveCursorAction(this.owner, com.Axis);
      }
    }

    
  }
}

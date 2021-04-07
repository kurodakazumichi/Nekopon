using MyGame.Unit.Versus.BrainAction;

namespace MyGame.Unit.Versus
{
  public class BrainPlayer : IBrain
  {
    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// カーソル移動時、キーを押し続けた時にリピート移動する際の時間間隔
    /// </summary>
    private const float MOVE_REPEAT_TIMER = 0.12f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// ゲームパッド番号
    /// </summary>
    private int padNo = 0;

    /// <summary>
    /// 移動待機用のタイマー
    /// </summary>
    private float waitMoveTimer = 0;

    /// <summary>
    /// プレイヤー
    /// </summary>
    private Player owner = null;

    /// <summary>
    /// 決定された行動
    /// </summary>
    private IAction decidedAction = null;

    //-------------------------------------------------------------------------
    // メソッド

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

    /// <summary>
    /// カーソル移動の監視
    /// </summary>
    private void MonitorMoveCursor()
    {
      if (this.decidedAction != null) return;

      // 時間経過
      this.waitMoveTimer -= TimeSystem.Instance.DeltaTime;

      // 移動コマンドを取得
      var com = InputSystem.Instance.GetCommand(InputManagement.Command.Move, this.padNo);

      // コマンドが成立していなければ終了
      if (!com.IsFixed) {
        return;
      }
      
      // カーソル移動から一定時間経過していなければ終了
      if (0 < this.waitMoveTimer) {
        return;
      }

      // リピートタイマーをセットしつつ、カーソル移動コマンドを生成
      this.waitMoveTimer = MOVE_REPEAT_TIMER;
      this.decidedAction = new MoveCursorAction(this.owner, com.Axis);
    }
  }
}

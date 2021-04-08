using MyGame.Unit.Versus.BrainAction;
using MyGame.InputManagement;
using MyGame.Define;

namespace MyGame.Unit.Versus
{
  public class BrainPlayer : IBrain
  {
    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// カーソル移動時、キーを押し続けた時にリピート移動する際の時間間隔
    /// </summary>
    private const float MOVE_REPEAT_TIMER = 0.06f;

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
    /// 初回移動
    /// </summary>
    private bool isFirstMove = true;

    /// <summary>
    /// スキルガイド表示状態
    /// </summary>
    private bool isShowSkillGuide = false;

    /// <summary>
    /// プレイヤー
    /// </summary>
    private Player owner = null;

    /// <summary>
    /// 決定された行動
    /// </summary>
    private IAction decidedAction = null;

    /// <summary>
    /// 入力システム
    /// </summary>
    private InputSystem input = null;

    //-------------------------------------------------------------------------
    // メソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public BrainPlayer(Player owner)
    {
      // InputSystemをキャッシュ
      this.input = InputSystem.Instance;

      // Ownerとパッド番号を保持
      this.owner = owner;
      this.padNo = (owner.Type == Define.App.Player.P1)? 0 : 1;
    }

    /// <summary>
    /// 思考
    /// </summary>
    public IAction Think()
    {
      this.decidedAction = null;

      if (this.input.GetCommand(Command.ShowSkillGuide, this.padNo).IsFixed) {
        this.isShowSkillGuide = true;
        return new SkillGuideAction(this.owner, true);
      }

      if (this.input.GetCommand(Command.HideSkillGuide, this.padNo).IsFixed) {
        this.isShowSkillGuide = false;
        return new SkillGuideAction(this.owner, false);
      }

      if (this.isShowSkillGuide) {
        MonitorAttributeSkill();
        
      } else {
        MonitorMoveCursor();
        MonitorSelectPaw();
        MonitorReleasePaw();
        MonitorChain();
      }

      if (this.decidedAction == null) {
        if (this.input.GetCommand(Command.FireUniqueSkill, this.padNo).IsFixed) {
          return new FireUniqueSkillAction(this.owner);
        }
      }

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
      var com = this.input.GetCommand(Command.Move, this.padNo);

      // コマンドが成立していなければ終了
      if (!com.IsFixed) {
        this.isFirstMove = true;
        return;
      }
      
      // カーソル移動から一定時間経過していなければ終了
      if (0 < this.waitMoveTimer) {
        return;
      }

      // リピートタイマーをセットしつつ、カーソル移動コマンドを生成
      this.waitMoveTimer = MOVE_REPEAT_TIMER * ((this.isFirstMove)? 2f : 1f);
      this.decidedAction = new MoveCursorAction(this.owner, com.Axis);
      this.isFirstMove = false;
    }

    /// <summary>
    /// 属性スキル発動の監視
    /// </summary>
    private void MonitorAttributeSkill()
    {
      if (this.decidedAction != null) return;

      if (this.input.GetCommand(Command.ShowSkillGuide, this.padNo).IsFixed) return;

      this.decidedAction = GetFireAttributeAction(); 
    }

    /// <summary>
    /// 肉球選択のコマンドを監視
    /// </summary>
    private void MonitorSelectPaw()
    {
      if (this.decidedAction != null) {
        return;
      }

      if (!input.GetCommand(Command.Decide, this.padNo).IsFixed) {
        return; 
      }

      this.decidedAction = new SelectPawAction(this.owner);
    }

    /// <summary>
    /// 肉球解除のコマンドを監視
    /// </summary>
    private void MonitorReleasePaw()
    {
      if (this.decidedAction != null) {
        return;
      }

      if (!input.GetCommand(Command.Cancel, this.padNo).IsFixed) {
        return;
      }

      this.decidedAction = new ReleasePawAction(this.owner);
    }

    /// <summary>
    /// 連鎖コマンドの監視
    /// </summary>
    private void MonitorChain()
    {
      if (this.decidedAction != null) {
        return;
      }

      if (!input.GetCommand(Command.Chain, this.padNo).IsFixed) {
        return;
      }

      this.decidedAction = new ChainAction(this.owner);
    }

    /// <summary>
    /// 属性スキル発動アクションを取得
    /// </summary>
    private IAction GetFireAttributeAction()
    {
      if (this.input.GetCommand(Command.FireSkillFir, this.padNo).IsFixed) {
        return new FireAttributeAction(this.owner, App.Attribute.Fir);
      }

      if (this.input.GetCommand(Command.FireSkillWat, this.padNo).IsFixed) {
        return new FireAttributeAction(this.owner, App.Attribute.Wat);
      }

      if (this.input.GetCommand(Command.FireSkillThu, this.padNo).IsFixed) {
        return new FireAttributeAction(this.owner, App.Attribute.Thu);
      }

      if (this.input.GetCommand(Command.FireSkillIce, this.padNo).IsFixed) {
        return new FireAttributeAction(this.owner, App.Attribute.Ice);
      }

      if (this.input.GetCommand(Command.FireSkillTre, this.padNo).IsFixed) {
        return new FireAttributeAction(this.owner, App.Attribute.Tre);
      }

      if (this.input.GetCommand(Command.FireSkillHol, this.padNo).IsFixed) {
        return new FireAttributeAction(this.owner, App.Attribute.Hol);
      }

      if (this.input.GetCommand(Command.FireSkillDar, this.padNo).IsFixed) {
        return new FireAttributeAction(this.owner, App.Attribute.Dar);
      }

      return null;
    } 
  }
}

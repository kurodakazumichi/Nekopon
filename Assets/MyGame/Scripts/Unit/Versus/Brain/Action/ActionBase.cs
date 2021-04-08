namespace MyGame.Unit.Versus.BrainAction
{
  /// <summary>
  /// アクションの基底クラス
  /// </summary>
  public abstract class ActionBase : IAction
  {
    /// <summary>
    /// プレイヤー(Owner)
    /// </summary>
    protected Player owner = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ActionBase(Player owner)
    {
      this.owner = owner;
    }

    /// <summary>
    /// 実行
    /// </summary>
    public abstract void Execute();
  }

  /// <summary>
  /// 肉球を選択するアクション
  /// </summary>
  public class SelectPawAction : ActionBase
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="owner"></param>
    public SelectPawAction(Player owner) : base(owner)
    {
    }

    /// <summary>
    /// 肉球を選択する
    /// </summary>
    public override void Execute()
    {
      this.owner.TrySelectPaw();
    }
  }

  /// <summary>
  /// 肉球選択を解除するアクション
  /// </summary>
  public class ReleasePawAction : ActionBase
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ReleasePawAction(Player owner) : base(owner)
    {
    }

    /// <summary>
    /// 肉球選択を解除する
    /// </summary>
    public override void Execute()
    {
      this.owner.TryReleasePaw();
    }
  }

  /// <summary>
  /// 連鎖するアクション
  /// </summary>
  public class ChainAction : ActionBase
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ChainAction(Player owner) : base(owner)
    {
    }

    /// <summary>
    /// 連鎖する
    /// </summary>
    public override void Execute()
    {
      this.owner.TryChain();
    }
  }
}
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
}
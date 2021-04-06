namespace MyGame.Unit.Versus
{
  /// <summary>
  /// Brainのインターフェース
  /// </summary>
  public interface IBrain
  {
    /// <summary>
    /// 思考する
    /// </summary>
    IAction Think();
  }
}

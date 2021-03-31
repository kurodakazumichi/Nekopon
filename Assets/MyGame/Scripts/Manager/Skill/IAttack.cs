namespace MyGame
{
  /// <summary>
  /// 攻撃ユニットのInterface
  /// </summary>
  public interface IAttack : IPoolable
  {
    /// <summary>
    /// セットアップ可能
    /// </summary>
    void Setup();

    /// <summary>
    /// Idleに出来る
    /// </summary>
    void ToIdle();
  }
}
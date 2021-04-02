using MyGame.Unit.Versus;

namespace MyGame
{
  /// <summary>
  /// 固有スキルインターフェース
  /// </summary>
  public interface IUniqueSkill : IPoolable
  {
    /// <summary>
    /// セットアップ可能
    /// </summary>
    void Setup(Define.App.UniqueSkill skillType, Player owner, Player target);

    /// <summary>
    /// 発動可能
    /// </summary>
    void Fire();
  }
}
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
    void Setup(Define.App.UniqueSkill skillType, Define.App.Cat catType);

    /// <summary>
    /// 発動可能
    /// </summary>
    void Fire(Player owner, Player target);
  }
}
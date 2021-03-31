using MyGame.Unit.Versus;

namespace MyGame
{
  /// <summary>
  /// 属性スキルユニットのInterface
  /// </summary>
  public interface ISkill : IPoolable
  {
    /// <summary>
    /// 属性
    /// </summary>
    Define.App.Attribute Attribute { get; }

    /// <summary>
    /// 初期化
    /// </summary>
    ISkill Init(Define.App.Attribute attribute);

    /// <summary>
    /// セットアップ可能
    /// </summary>
    void Setup();

    /// <summary>
    /// 発動可能
    /// </summary>
    void Fire(Player owner, Player target);
  }
}
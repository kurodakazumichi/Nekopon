using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// エフェクトのインターフェース
  /// </summary>
  public interface IEffect : IPoolable
  {
    /// <summary>
    /// エフェクトの効果発動時に呼びたいアクションを設定可能
    /// </summary>
    System.Action Action { set; }

    /// <summary>
    /// セットアップ可能
    /// </summary>
    void Setup();

    /// <summary>
    /// 位置してして発動可能
    /// </summary>
    void Fire(Vector3 position);
  }
}
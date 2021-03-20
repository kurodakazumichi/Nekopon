using UnityEngine;

namespace MyGame.Unit.Mover
{
  /// <summary>
  /// MoverのInterface
  /// </summary>
  public interface IMover : IPoolable
  {
    /// <summary>
    /// Transformが取得できる
    /// </summary>
    Transform CacheTransform { get; }

    /// <summary>
    /// SpriteRendrerが取得できる
    /// </summary>
    SpriteRenderer Renderer { get; }

    /// <summary>
    /// アルファ値のアクセッサ
    /// </summary>
    float Alpha { get; set; }

    /// <summary>
    /// 輝度のアクセッサ
    /// </summary>
    float Brightness { get; set; }
  }
}
using UnityEngine;

namespace MyGame.Unit.Effect
{
  /// <summary>
  /// エフェクトの基底クラス
  /// </summary>
  public abstract class EffectBase<T> : Unit<T>, IEffect where T : System.Enum
  {
    /// <summary>
    /// エフェクトの効果発動時に呼びたいアクション
    /// </summary>
    public System.Action Action { protected get; set; } = null;

    /// <summary>
    /// セットアップ
    /// </summary>
    public virtual void Setup()
    {

    }

    /// <summary>
    /// エフェクトを放つ
    /// </summary>
    public virtual void Fire(Vector3 position)
    {
      CacheTransform.position = position;
    }
  }
}
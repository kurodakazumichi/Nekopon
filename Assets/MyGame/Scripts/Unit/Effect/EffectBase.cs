using UnityEngine;

namespace MyGame.Unit.Effect
{
  /// <summary>
  /// エフェクトの基底クラス
  /// </summary>
  public abstract class EffectBase<T> : Unit<T>, EffectManager.IEffect where T : System.Enum
  {
    /// <summary>
    /// エフェクトがIdle状態であることを表す
    /// </summary>
    public virtual bool IsIdle => true;

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
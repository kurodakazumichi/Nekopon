using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public static class Tween
  {
    /// <summary>
    /// Tweenのタイプ
    /// </summary>
    public enum Type
    {
      EaseInSine,
      EaseOutSine,
      EaseInOutSine,
      EaseOutBounce,
    }

    public static float easing(Type type, float t)
    {
      switch(type) {
        case Type.EaseInSine: return EaseInSine(t);
        case Type.EaseOutSine: return EaseOutSine(t);
        case Type.EaseInOutSine: return EaseInOutSine(t);
        case Type.EaseOutBounce: return EaseOutBounce(t);
        default: return EaseInSine(t);
      }
    }

    public static float EaseInSine(float t)
    {
      return 1 - Mathf.Cos((t * Mathf.PI) / 2);
    }

    public static float EaseOutSine(float t)
    {
      return Mathf.Sin((t * Mathf.PI) / 2);
    }

    public static float EaseInOutSine(float t)
    {
      return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
    }

    public static float EaseOutBounce(float t)
    {
      const float n1 = 7.5625f;
      const float d1 = 2.75f;

      if (t < 1 / d1) {
        return n1 * t * t;
      }
      else if (t < 2 / d1) {
        return n1 * (t -= 1.5f / d1) * t + 0.75f;
      }
      else if (t < 2.5 / d1) {
        return n1 * (t -= 2.25f / d1) * t + 0.9375f;
      }
      else {
        return n1 * (t -= 2.625f / d1) * t + 0.984375f;
      }
    }
  }
}

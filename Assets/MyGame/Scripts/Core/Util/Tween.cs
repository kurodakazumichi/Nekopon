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
      EaseUniform,
      EaseInSine,
      EaseOutSine,
      EaseInOutSine,
      EaseOutBounce,
      EaseInBack,
      EaseOutBack,
      EaseInOutBack,
      EaseInElastic,
      EaseOutElastic,
    }

    public static float easing(Type type, float t)
    {
      switch(type) {
        case Type.EaseUniform: return EaseUniform(t);
        case Type.EaseInSine: return EaseInSine(t);
        case Type.EaseOutSine: return EaseOutSine(t);
        case Type.EaseInOutSine: return EaseInOutSine(t);
        case Type.EaseOutBounce: return EaseOutBounce(t);
        case Type.EaseInBack: return EaseInBack(t);
        case Type.EaseOutBack: return EaseOutBack(t);
        case Type.EaseInOutBack: return EaseInOutBack(t);
        case Type.EaseInElastic: return EaseInElastic(t);
        case Type.EaseOutElastic: return EaseOutElastic(t);
        default: return EaseInSine(t);
      }
    }

    public static float EaseUniform(float t)
    {
      return t;
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

    public static float EaseInBack(float t)
    {
      const float c1 = 1.70158f;
      const float c3 = c1 + 1;

      return c3 * t * t * t - c1 * t * t;
    }

    public static float EaseOutBack(float t)
    {
      const float c1 = 1.70158f;
      const float c3 = c1 + 1;

      return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    public static float EaseInOutBack(float x)
    {
      const float c1 = 1.70158f;
      const float c2 = c1 * 1.525f;

      return x < 0.5
        ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
        : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }

    public static float EaseInElastic(float t)
    {
      const float c4 = (2 * Mathf.PI) / 3;

      return t == 0f
        ? 0
        : t == 1f
        ? 1
        : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);
    }

    public static float EaseOutElastic(float t)
    {
      const float c4 = (2 * Mathf.PI) / 3;

      return t == 0
        ? 0
        : t == 1
        ? 1
        : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    }
  }
}

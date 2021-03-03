using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public static class Tween
  {
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

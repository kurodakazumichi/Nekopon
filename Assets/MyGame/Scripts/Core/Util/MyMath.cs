using UnityEngine;

namespace MyGame
{

  public static class MyMath
  {
    /// <summary>
    /// 小数点の桁数を指定して丸める
    /// </summary>
    public static float Round(float num, int digit)
    {
      float pow = Mathf.Pow(10, digit);
      return Mathf.Floor(num * pow) / pow;
    }

    /// <summary>
    /// 補間
    /// </summary>
    public static float Lerp(float from, float to, float rate)
    {
      return from + (to - from) * rate;
    }
  }
}
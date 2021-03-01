using UnityEngine;

namespace MyGame
{

  public static class MyMath
  {
    public static float Round(float num, int digit)
    {
      float pow = Mathf.Pow(10, digit);
      return Mathf.Floor(num * pow) / pow;
    }
  }
}
using UnityEngine;

namespace MyGame
{
  public static class MyVector3
  {
    public static Vector3 Lerp(Vector3 v1, Vector3 v2, float rate)
    {
      return v1 + (v2 - v1) * rate;
    }
  }
}
using UnityEngine;

namespace MyGame
{
  public static class MyVector3
  {
    public static Vector3 Lerp(Vector3 v1, Vector3 v2, float rate)
    {
      return v1 + (v2 - v1) * rate;
    }

    public static Vector3 Random()
    {
      return new Vector3(
        UnityEngine.Random.Range(-1f, 1f),
        UnityEngine.Random.Range(-1f, 1f),
        0
      );
    }
  }
}
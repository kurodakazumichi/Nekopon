using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame
{
  public static class Util
  {
    /// <summary>
    /// 列挙型の要素の数だけループする
    /// </summary>
    /// <param name="action">コールバック</param>
    public static void ForEach<T>(Action<T> func) where T : Enum
    {
      foreach(T value in Enum.GetValues(typeof(T))) {
        func(value);
      }
    }

    public static void ForEach<T>(Action<T, string> func) where T : Enum
    {
      foreach(T value in Enum.GetValues(typeof(T))) {
        func(value, Enum.GetName(typeof(T), value));
      }
    }
  }

  public static class MyMath
  {
    public static float Round(float num, int digit)
    {
      float pow = Mathf.Pow(10, digit);
      return Mathf.Floor(num * pow) / pow;
    }
  }
}
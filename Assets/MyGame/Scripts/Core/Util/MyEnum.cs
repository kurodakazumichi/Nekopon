using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame
{
  public static class MyEnum
  {
    /// <summary>
    /// Enumの中からランダムな値を取得
    /// </summary>
    public static T Random<T>() where T : Enum
    {
      var count = Enum.GetNames(typeof(T)).Length;
      var index = UnityEngine.Random.Range(0, count);
      var value = Enum.GetValues(typeof(T)).GetValue(index);
      return (T)value;
    }
  }
}
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

    /// <summary>
    /// 列挙型の要素の数だけループする
    /// </summary>
    /// <param name="action">コールバック(value)</param>
    public static void ForEach<T>(Action<T> func) where T : Enum
    {
      foreach (T value in Enum.GetValues(typeof(T))) {
        func(value);
      }
    }

    /// <summary>
    /// 列挙型の要素の数だけループする
    /// </summary>
    /// <param name="action">コールバック(value, int)</param>
    public static void ForEach<T>(Action<T, int> func) where T : Enum
    {
      int index = 0;
      foreach(T value in Enum.GetValues(typeof(T))) {
        func(value, index++);
      }
    }

    /// <summary>
    /// 列挙型の要素の数だけループする
    /// </summary>
    /// <param name="func">func(value, key)</param>
    public static void ForEach<T>(Action<T, string> func) where T : Enum
    {
      foreach (T value in Enum.GetValues(typeof(T))) {
        func(value, Enum.GetName(typeof(T), value));
      }
    }

    /// <summary>
    /// Enum要素の文字列配列を取得
    /// </summary>
    public static string[] GetNames<T>() where T : Enum
    {
      return Enum.GetNames(typeof(T));
    }

    /// <summary>
    /// 文字列をEnumに強制変換する
    /// </summary>
    public static T Parse<T>(string name) where T : Enum
    {
      return (T)Enum.Parse(typeof(T), name);
    }
  }
}
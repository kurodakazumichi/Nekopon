using System.Collections.Generic;
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

    public static void ForEach<T1, T2>(Dictionary<T1, T2> dic, Action<T1, T2> func)
    {
      foreach(KeyValuePair<T1, T2> data in dic) {
        func(data.Key, data.Value);
      }
    }
  }
}
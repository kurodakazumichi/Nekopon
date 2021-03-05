using System.Collections.Generic;
using System;

namespace MyGame
{
  public static class Util
  {
    public static void ForEach<T>(T[] array, Action<T> func)
    {
      int count = array.Length;

      for (int i = 0; i < count; ++i) {
        func(array[i]);
      }
    }

    public static void ForEach<T>(T[] array, Func<T, bool> func)
    {
      int count = array.Length;
      for (int i = 0; i < count; ++i) {
        bool completed = func(array[i]);
        if (completed) break;
      }
    }

    public static void ForEach<T1, T2>(Dictionary<T1, T2> dic, Action<T1, T2> func)
    {
      foreach(KeyValuePair<T1, T2> data in dic) {
        func(data.Key, data.Value);
      }
    }

    public static void ForEach<T1, T2>(Dictionary<T1, T2> dic, Func<T1, T2, bool> func)
    {
      foreach(KeyValuePair<T1, T2> data in dic) {
        bool completed = func(data.Key, data.Value);
        if (completed) break;
      }
    }
  }
}
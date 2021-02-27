using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace MyGame.Debug
{
  /// <summary>
  /// Debug.Log系のラッパークラス
  /// マクロ定義によりメソッド呼び出しの有効無効が可能
  /// </summary>
  public static class Logger
  {
    [Conditional("_DEBUG")]    
    public static void Log(object message)
    {
      UnityEngine.Debug.Log(message);
    }

    [Conditional("_DEBUG")]
    public static void Error(object message)
    {
      UnityEngine.Debug.LogError(message);
    }

    [Conditional("_DEBUG")]
    public static void Warn(object message)
    {
      UnityEngine.Debug.LogWarning(message);
    }
  }
}
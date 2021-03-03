using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame.Define
{
  public static class App
  {
    public enum Attribute
    {
      Fir, // 火
      Wat, // 水
      Thu, // 雷
      Ice, // 氷
      Tre, // 木
      Hol, // 聖
      Dar, // 闇
    }

    /// <summary>
    /// 属性の個数
    /// </summary>
    public static int AttributeCount => Enum.GetNames(typeof(Attribute)).Length;

  }


}
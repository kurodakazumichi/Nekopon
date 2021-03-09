using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame.Define
{
  public static class App
  {
    /// <summary>
    /// 属性の種類、配列の添え字としても利用するため0からの連番で定義
    /// </summary>
    public enum Attribute
    {
      Fir = 0, // 火
      Wat = 1, // 水
      Thu = 2, // 雷
      Ice = 3, // 氷
      Tre = 4, // 木
      Hol = 5, // 聖
      Dar = 6, // 闇
    }

    /// <summary>
    /// プレイヤーの種類
    /// </summary>
    public enum Player
    {
      P1,
      P2,
    }

    /// <summary>
    /// 属性の個数
    /// </summary>
    public static int AttributeCount => Enum.GetNames(typeof(Attribute)).Length;

    /// <summary>
    /// 操作方法(キーボード操作の種類)
    /// </summary>
    public enum OperationMethod
    {
      Standard, // 1人プレイ時
      Player1,  // 対戦時：1P
      Player2,  // 対戦時：2P
    }

    /// <summary>
    /// 想定するゲームパッドの種類
    /// </summary>
    public enum JoyType
    {
      X360,
      PS4,
    }
  }
}
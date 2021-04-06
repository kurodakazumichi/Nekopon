using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame.Define
{
  public static class App
  {
    /// <summary>
    /// 画面下の座標
    /// </summary>
    public const float SCREEN_BOTTOM = -0.75f;

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
    /// 固有スキル
    /// </summary>
    public enum UniqueSkill
    {
      Invincible, // 無敵
      Reflection, // 反射
      Recovery,   // 回復
      Swap,       // 入替
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
    /// 脳のタイプ
    /// </summary>
    public enum Brain
    {
      Player,
      AI,
    }

    /// <summary>
    /// 方向
    /// </summary>
    public enum Direction
    {
      L, // 左
      R, // 右
      U, // 上
      D, // 下
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

    /// <summary>
    /// 猫
    /// </summary>
    public enum Cat
    {
      Minchi,
      Nick,
      Shiro,
      Tii,
    }
  }
}
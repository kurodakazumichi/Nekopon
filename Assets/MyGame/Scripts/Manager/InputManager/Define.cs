using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.InputManagement
{
  /// <summary>
  /// 軸の種類
  /// </summary>
  public enum AxisType
  {
    LX,
    LY,
    RX,
    RY,
    DX,
    DY,
    LR,
    LT,
    RT,
  }

  /// <summary>
  /// ボタンの種類
  /// </summary>
  public enum ButtonType
  {
    A,
    B,
    X,
    Y,
    L1,
    R1,
    LS,
    RS,
    Start,
    Back,
  }

  /// <summary>
  /// キーボード入力をマッピングするための定義
  /// キーボードのAが押されたときに、パッドのAが押された事にするなどマッピングする
  /// </summary>
  public enum KeyType
  {
    A,
    B,
    X,
    Y,
    L1,
    L2,
    R1,
    R2,
    LS,
    RS,
    Start,
    Back,
    Up,
    Down,
    Left,
    Right,
  }

}
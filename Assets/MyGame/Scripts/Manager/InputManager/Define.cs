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
    AxisLX,
    AxisLY,
    AxisRX,
    AxisRY,
    AxisDX,
    AxisDY,
    Triggers,
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

}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.VersusManagement
{
  /// <summary>
  /// 対戦画面内の各種ロケーション(座標)を格納したクラス
  /// </summary>
  public class Location
  {
    /// <summary>
    /// HPゲージの位置
    /// </summary>
    public Vector3 HpGuage { get; private set; } = Vector3.zero;

    /// <summary>
    /// APゲージの位置
    /// </summary>
    public Vector3 ApGuage { get; private set; } = Vector3.zero;

    /// <summary>
    /// 肉球の配置基準位置
    /// </summary>
    public Vector3 Paw { get; private set; } = Vector3.zero;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Location(string prefix, GameObject go)
    {
      Transform locations = go.transform;
      Paw = locations.Find($"{prefix}.Paw").position;
      HpGuage = locations.Find($"{prefix}.Gauge.Hp").position;
      ApGuage = locations.Find("${prefix}.Gauge.Ap").position;
    }
  }
}


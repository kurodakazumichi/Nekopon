using System.Collections;
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
    /// 自分のパズルの中心あたり
    /// </summary>
    public Vector3 MyCenter { get; private set; } = Vector3.zero;

    /// <summary>
    /// 相手のパズルの中心あたり
    /// </summary>
    public Vector3 TargetCenter { get; private set; } = Vector3.zero;

    /// <summary>
    /// 猫の位置
    /// </summary>
    public Vector3 Cat { get; private set; } = Vector3.zero;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Location(string myPrefix, string targetPrefix, GameObject go)
    {
      Transform locations = go.transform;
      Paw          = locations.Find($"{myPrefix}.Paw").position;
      HpGuage      = locations.Find($"{myPrefix}.Gauge.Hp").position;
      ApGuage      = locations.Find($"{myPrefix}.Gauge.Ap").position;
      MyCenter     = locations.Find($"{myPrefix}.Center").position;
      TargetCenter = locations.Find($"{targetPrefix}.Center").position;
      Cat          = locations.Find($"{myPrefix}.Cat").position;
    }
  }
}


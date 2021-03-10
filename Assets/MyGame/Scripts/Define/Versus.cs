using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Define
{
  public static class Versus
  {
    /// <summary>
    /// 連鎖モード
    /// </summary>
    public enum ChainMode
    {
      Single,
      Multi,
    }

    /// <summary>
    /// 肉球を配置する際のX方向の間隔
    /// </summary>
    public const float PAW_INTERVAL_X = 0.105f;

    /// <summary>
    /// 肉球を配置する際のY方向の間隔
    /// </summary>
    public const float PAW_INTERVAL_Y = 0.1035f;

    /// <summary>
    /// パズルの列数
    /// </summary>
    public const int PAW_COL = 6;

    /// <summary>
    /// パズルの行数
    /// </summary>
    public const int PAW_ROW = 12;

    /// <summary>
    /// 肉球の合計数
    /// </summary>
    public const int PAW_TOTAL = PAW_COL * PAW_ROW;

    /// <summary>
    /// 連鎖に必要な肉球の数
    /// </summary>
    public const int CHAIN_PAW_COUNT = 4;

    /// <summary>
    /// 対戦開始時に肉球が揃うのにかかる時間
    /// </summary>
    public const float PAW_SETUP_MIN_TIME = 1f;
    public const float PAW_SETUP_MAX_TIME = 2f;

    /// <summary>
    /// 1回あたりの補充にかける最小時間、及び最大時間
    /// 連鎖後、肉球を補充するために落下させるが、その落下にかかる時間
    /// </summary>
    public const float PAW_STAFF_MIN_TIME = 0.3f;
    public const float PAW_STAFF_MAX_TIME = 0.8f;

    /// <summary>
    /// ゲージの最大幅
    /// </summary>
    public const float GAUGE_MAX_WIDTH = 2.77f;

    /// <summary>
    /// ゲージの色設定
    /// </summary>
    public static readonly Color GAUGE_HP_COLOR = new Color(0, 1, 0.9500496f);
    public static readonly Color GAUGE_DP_COLOR = new Color(1, 0, 0, 0.5f);
    public static readonly Color GAUGE_AP_COLOR = new Color(1, 1, 1);
  }
}
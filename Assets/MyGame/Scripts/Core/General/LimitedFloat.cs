using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// 読み取り専用LimitedFloat
  /// </summary>
  public interface ILimitedFloat
  {
    float Now { get; }
    float Max { get; }
    float Rate { get; }
    bool IsFull { get; }
    bool IsEmpty { get; }
  }

  /// <summary>
  /// 制限付き浮動小数点
  /// </summary>
  public class LimitedFloat : ILimitedFloat
  {
    //-------------------------------------------------------------------------
    // Member

    /// <summary>
    /// 現在の値
    /// </summary>
    private float now;

    /// <summary>
    /// 最大値
    /// </summary>
    private float max;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// 現在の値
    /// </summary>
    public float Now {
      get { return this.now; }

      set {
        this.now = Mathf.Max(0, Mathf.Min(value, this.max));
      }
    }

    /// <summary>
    /// 最大値
    /// </summary>
    public float Max {
      get { return this.max; }

      set {
        this.max = Mathf.Max(1, value);
        this.now = Mathf.Min(this.now, this.max);
      }
    }

    /// <summary>
    /// 割合
    /// </summary>
    public float Rate => (this.now / this.max);

    /// <summary>
    /// 満タン
    /// </summary>
    public bool IsFull => (this.now == this.max);

    /// <summary>
    /// 空っぽ
    /// </summary>
    public bool IsEmpty => (this.now == 0);

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// セットアップ
    /// </summary>
    public void Setup(float now, float max)
    {
      Max = max;
      Now = now;
    }

    /// <summary>
    /// 1になる
    /// </summary>
    public void BeToOne()
    {
      Now = 1;
    }

    /// <summary>
    /// いっぱいになる
    /// </summary>
    public void BeToFull()
    {
      Now = Max;
    }

    /// <summary>
    /// 空になる
    /// </summary>
    public void BeToEmpty()
    {
      Now = 0;
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame
{
  /// <summary>
  /// ビットフラグ(32bit版)
  /// </summary>
  public class Flag32 : IFormattable
  {
    /// <summary>
    /// フラグ
    /// </summary>
    public uint Value { get; set; } = 0;

    /// <summary>
    /// フラグをクリア
    /// </summary>
    public void Clear()
    {
      this.Value = 0;
    }

    /// <summary>
    /// 指定されたフラグと一致するか
    /// </summary>
    public bool Is(uint flags)
    {
      return Value == flags;
    }

    /// <summary>
    /// 指定されたフラグがたっているか
    /// </summary>
    public bool Has(uint flags)
    {
      if (Value == 0) return false;
      return (this.Value & flags) == flags;
    }

    /// <summary>
    /// 指定されたフラグのいずれかがたっている
    /// </summary>
    public bool HasEither(uint flags)
    {
      if (Value == 0) return false;
      return (this.Value & flags) != 0;
    }

    //-------------------------------------------------------------------------
    // 演算子オーバーロード

    /// <summary>
    /// fragsを立てるのは += でやれるように operator +をオーバーロードしてみる
    /// </summary>
    public static Flag32 operator +(Flag32 self, uint flags)
    {
      self.Value |= flags;
      return self;
    }

    /// <summary>
    /// Fragを落とすのは -= でやれるように operator -をオーバーロードしてみる
    /// </summary>
    public static Flag32 operator -(Flag32 self, uint flags)
    {
      self.Value &= ~flags;
      return self;
    }

    //-------------------------------------------------------------------------
    // IFormattable

    // ToStringを実装し、文字列にすると2進数表記になるようにしておく
    public string ToString(string format, IFormatProvider formatProvider)
    {
      return Convert.ToString(this.Value, 2);
    }
  }

}
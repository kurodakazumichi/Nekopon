using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// カスタムコルーチン、カウンターが0になるまで継続する
  /// </summary>
  class WaitForCount : CustomYieldInstruction
  {
    private int count = 0;

    public void inc() { ++this.count; }
    public void dec() { --this.count; }
    public WaitForCount() { this.count = 0; }

    public override bool keepWaiting {
      get {
        return (0 < this.count);
      }
    }
  }
}
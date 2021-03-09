using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class Gauge : Unit<Gauge.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State { }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// ゲージのスプライトを表示するため
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// カーソル用のSprite
    /// </summary>
    private static Sprite GaugeSprite = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceManager.Instance;
      rm.Load<Sprite>("Common.White.sprite", pre, done, (res) => { GaugeSprite = res; });
    }

    public static void Unload()
    {
      var rm = ResourceManager.Instance;
      rm.Unload("Common.White.sprite");
      GaugeSprite = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
      this.spriteRenderer.sprite = GaugeSprite;
    }

  }
}

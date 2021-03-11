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
    // プロパティ

    public float Rate
    {
      set {
        var scale = CacheTransform.localScale;
        scale.x = Mathf.Lerp(0, Define.Versus.GAUGE_MAX_WIDTH, Mathf.Max(0, Mathf.Min(1, value)));
        CacheTransform.localScale = scale;
      }
    }

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
      this.spriteRenderer = CacheTransform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    //-------------------------------------------------------------------------
    // publicメソッド

    /// <summary>
    /// セットアップ
    /// </summary>
    public Gauge Setup()
    {
      this.spriteRenderer.sprite = GaugeSprite;
      return this;
    }

    /// <summary>
    /// HPカラーに変更
    /// </summary>
    public void ToHpColor()
    {
      this.spriteRenderer.color = Define.Versus.GAUGE_HP_COLOR;
    }

    /// <summary>
    /// ダメージカラーに変更
    /// </summary>
    public void ToDpColor()
    {
      this.spriteRenderer.color = Define.Versus.GAUGE_DP_COLOR;
    }

    /// <summary>
    /// APカラーに変更
    /// </summary>
    public void ToApColor()
    {
      this.spriteRenderer.color = Define.Versus.GAUGE_AP_COLOR;
    }
  }
}

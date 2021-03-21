using UnityEngine;

namespace MyGame.Unit.Mover
{
  /// <summary>
  /// 基本となるMover
  /// </summary>
  public class Basis : Unit<Basis.State>, IMover
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State { }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// Idleになった時に呼ばれるコールバック
    /// </summary>
    public System.Action<Basis> OnIdle { private get; set; } = null;

    /// <summary>
    /// 色
    /// </summary>
    private Color color = Color.white;

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer render = null;

    /// <summary>
    /// オペレーター
    /// </summary>
    private readonly MoverOperator op = new MoverOperator();

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// アイドルかどうか
    /// </summary>
    public bool IsIdle => this.op.IsIdle;

    //-------------------------------------------------------------------------
    // IMoverの実装

    /// <summary>
    /// SpriteRendererを返すプロパティ
    /// </summary>
    public SpriteRenderer Renderer => this.render;

    /// <summary>
    /// アルファ
    /// </summary>
    public float Alpha {
      get { return this.render.color.a; }
      set {
        this.color.a = value;
        this.render.color = this.color;
      }
    }

    /// <summary>
    /// 輝度
    /// </summary>
    public float Brightness {
      get { return Alpha; }
      set { Alpha = value; }
    }

    /// <summary>
    /// Tweenのタイプ
    /// </summary>
    public Tween.Type Tween {
      set { this.op.Tween = value; }
    }

    /// <summary>
    /// 移動
    /// </summary>
    public void ToMove(Vector3 start, Vector3 end, float time)
    {
      this.op.ToMove(start, end, time);
    }

    /// <summary>
    /// スケーリング
    /// </summary>
    public void ToScale(Vector3 start, Vector3 end, float time)
    {
      this.op.ToScale(start, end, time);
    }

    /// <summary>
    /// 通常
    /// </summary>
    public void ToUsual(Vector3 position, Vector3 scale)
    {
      this.op.ToUsual(position, scale);
    }

    /// <summary>
    /// Flash
    /// </summary>
    public void ToFlash(float time)
    {
      this.op.ToFlash(time);
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // SpriteRendererをひっつける
      this.render = AddComponent<SpriteRenderer>();

      // 自身を設定
      this.op.SetMover(this);
      this.op.OnIdle = () => { OnIdle?.Invoke(this); };
    }

    protected override void MyUpdate()
    {
      this.op.Update();
    }

    //-------------------------------------------------------------------------
    // セットアップ

    public void Setup(Sprite sprite, Material material, string layerName)
    {
      this.op.Setup(sprite, material, layerName);
    }

    /// <summary>
    /// Flashの設定
    /// </summary>
    public void SetFlash(float cycle, float minBrightness, float maxBrightness)
    {
      this.op.SetFlash(cycle, minBrightness, maxBrightness);
    }

    /// <summary>
    /// Flashの無効化
    /// </summary>
    public void DisableFlash()
    {
      this.op.DisableFlash();
    }
  }
}
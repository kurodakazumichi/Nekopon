using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Mover
{
  public class Glow : Unit<Glow.State>, IMover
  {
    public enum State
    { }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// Idleになった時に呼ばれるコールバック
    /// </summary>
    public System.Action<Glow> OnIdle { private get; set; } = null;

    /// <summary>
    /// 色
    /// </summary>
    private Color color = Color.white;

    /// <summary>
    /// Main Sprite Renderer
    /// </summary>
    private SpriteRenderer main = null;

    /// <summary>
    /// 加算合成 Sprite Renderer
    /// </summary>
    private SpriteRenderer glow = null;

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

    /// <summary>
    /// Tweenのタイプ
    /// </summary>
    public Tween.Type Tween {
      set { this.op.Tween = value; }
    }

    //-------------------------------------------------------------------------
    // Load, Unload
    
    /// <summary>
    /// 加算合成用マテリアル
    /// </summary>
    private static Material Material = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Material>("Common.Additive.material", pre, done, (res) => { Material = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Common.Additive.material");
      Material = null;
    }

    //-------------------------------------------------------------------------
    // IMoverの実装

    /// <summary>
    /// このクラスでは標準でglowのSpriteRendererを操作する
    /// </summary>
    public SpriteRenderer Renderer => this.glow;

    /// <summary>
    /// アルファ
    /// </summary>
    public float Alpha {
      get { return this.main.color.a; }
      set {
        this.color.a = value;
        this.main.color = this.color;
      }
    }

    /// <summary>
    /// 輝度
    /// </summary>
    public float Brightness {
      get { return this.glow.color.a; }
      set {
        this.color.a = value;
        this.glow.color = this.color;
      }
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // MainとGlowのSpriteオブジェクトを生成
      this.main = MyGameObject.Create<SpriteRenderer>("Main", CacheTransform);
      this.main.sortingOrder = Define.Layer.Order.Layer00;

      this.glow = MyGameObject.Create<SpriteRenderer>("Glow", CacheTransform);
      this.glow.sortingOrder = Define.Layer.Order.Layer00 + 1;

      this.op.SetMover(this);
      this.op.OnIdle = () => { OnIdle?.Invoke(this); };

      // デフォルトでFlashは無効
      this.op.DisableFlash();
    }

    protected override void MyUpdate()
    {
      this.op.Update();
    }

    //-------------------------------------------------------------------------
    // セットアップ

    public void Setup(Sprite sprite, string layerName)
    {
      this.main.sprite = sprite;
      
      this.op.Setup(this.main, sprite, null, layerName);
      this.op.Setup(this.glow, sprite, Material, layerName);
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

    //-------------------------------------------------------------------------
    // ステートマシン

    public void ToMove(Vector3 start, Vector3 end, float time)
    {
      this.op.ToMove(start, end, time);
    }

    public void ToScale(Vector3 start, Vector3 end, float time)
    {
      this.op.ToScale(start, end, time);
    }

    public void ToUsual(Vector3 position, Vector3 scale)
    {
      this.op.ToUsual(position, scale);
    }

    public void ToFlash(float time)
    {
      this.op.ToFlash(time);
    }
  }
}
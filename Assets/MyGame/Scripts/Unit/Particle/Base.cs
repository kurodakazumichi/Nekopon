using UnityEngine;

namespace MyGame.Unit.Particle
{
  /// <summary>
  /// パーティクルを生成するのに必要なパラメータ
  /// </summary>
  public class Props
  {
    //-------------------------------------------------------------------------
    // 以下はSpriteRendererに割り当てる必要があるパラメーター

    public Sprite Sprite = null;
    public Material MainMaterial = null;
    public Material GlowMaterial = null;
    public float Alpha = 1f;
    public float Brightness = 0f;
    public string LayerName = Define.Layer.Sorting.Effect;
    public bool IsOnlyGlow = false;

    //-------------------------------------------------------------------------
    // 各種パラメータ

    public Vector3 Velocity;
    public float AlphaAcceleration = 0f;
    public float LifeTime = 1f;
    public float RotationAcceleration = 0f;
    public float ScaleAcceleration = 0f;

    /// <summary>
    /// 自分自身で勝手に破棄するかどうか
    /// </summary>
    public bool IsSelfDestructive = true;
  };
  /// <summary>
  /// Particleのベース
  /// </summary>
  public abstract class Base<TState> : Unit<TState>, ParticleManager.IParticle where TState : System.Enum
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// Main Sprite Renderer
    /// </summary>
    protected SpriteRenderer main = null;

    /// <summary>
    /// 加算合成 Sprite Renderer
    /// </summary>
    protected SpriteRenderer glow = null;

    /// <summary>
    /// 色
    /// </summary>
    protected Color color = Color.white;

    /// <summary>
    /// 痕跡用のProps
    /// </summary>
    protected Props trace = null;

    /// <summary>
    /// トレース用タイマー
    /// </summary>
    protected float traceTimer = 0;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// Main用のマテリアル
    /// </summary>
    protected Material MainMaterial {
      set {
        this.main.material = value;
      }
    }

    /// <summary>
    /// Glow用のマテリアル
    /// </summary>
    protected Material GlowMaterial {
      set {
        this.glow.material = value;
      }
    }

    //-------------------------------------------------------------------------
    // メソッド

    /// <summary>
    /// 破壊
    /// </summary>
    public abstract void Destory();

    //-------------------------------------------------------------------------
    // IParticleの実装

    public abstract void Setup();

    /// <summary>
    /// セットアップ
    /// </summary>
    public virtual void Setup(Props props)
    {
      // SpriteRendererに関するパラメータ設定
      Sprite = props.Sprite;

      if (props.MainMaterial) {
        MainMaterial = props.MainMaterial;
      }
      if (props.GlowMaterial) {
        GlowMaterial = props.GlowMaterial;
      }

      Alpha      = props.Alpha;
      Brightness = props.Brightness;

      Velocity             = props.Velocity;
      RotationAcceleration = props.RotationAcceleration;
      AlphaAcceleration    = props.AlphaAcceleration;
      ScaleAcceleration    = props.ScaleAcceleration;
      LifeTime             = props.LifeTime;
      IsSelfDestructive    = props.IsSelfDestructive;

      // Glow Onlyだったらmainは非アクティブ
      this.main.gameObject.SetActive(!props.IsOnlyGlow);
    }

    /// <summary>
    /// 痕跡の設定
    /// </summary>
    public void SetTrace(Props props, float time)
    {
      this.trace = props;
      TraceTime = time;
    }

    /// <summary>
    /// 発動
    /// </summary>
    public virtual void Fire(Vector3 position, Vector3? scale, Quaternion? rotation)
    {
      CacheTransform.position = position;
      CacheTransform.localScale = scale ?? Vector3.one;
      CacheTransform.rotation = rotation ?? Quaternion.identity;
    }

    /// <summary>
    /// Spriteのsetter
    /// </summary>
    public Sprite Sprite {
      set {
        this.main.sprite = this.glow.sprite = value;
      }
    }

    /// <summary>
    /// アルファ
    /// </summary>
    public float Alpha {
      get { return this.main.color.a; }
      set {
        this.color.a = value;
        this.main.color = this.color;
        Brightness = Mathf.Min(Brightness, value);
      }
    }

    /// <summary>
    /// 輝度
    /// </summary>
    public float Brightness {
      get { return this.glow.color.a; }
      set {
        this.color.a = Mathf.Min(value, Alpha);
        this.glow.color = this.color;
      }
    }

    /// <summary>
    /// レイヤー名
    /// </summary>
    public string LayerName {
      set {
        this.main.sortingLayerName = value;
        this.glow.sortingLayerName = value;
      }
    }

    /// <summary>
    /// アルファ加速度
    /// </summary>
    public float AlphaAcceleration { get; set; }

    /// <summary>
    /// 寿命
    /// </summary>
    public float LifeTime { get; set; }

    /// <summary>
    /// 速度
    /// </summary>
    public Vector3 Velocity { get; set; }

    /// <summary>
    /// 回転加速度
    /// </summary>
    public float RotationAcceleration { get; set; }

    /// <summary>
    /// スケール加速度
    /// </summary>
    public float ScaleAcceleration { get; set; }

    /// <summary>
    /// 痕跡を残す時間
    /// </summary>
    public float TraceTime { get; set; }

    /// <summary>
    /// 自分自身で破壊するかどうか
    /// </summary>
    public bool IsSelfDestructive { get; set; }
  }
}
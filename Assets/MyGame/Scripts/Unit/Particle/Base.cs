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

    //-------------------------------------------------------------------------
    // 以下は処理中に直接使用するパラメータ

    public Vector3 Velocity;
    public float AlphaAcceleration = 0f;
    public float LifeTime = 1f;
    
  };
  /// <summary>
  /// Particleのベース
  /// </summary>
  /// <typeparam name="TState"></typeparam>
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
    /// パーティクルに関するパラメータが詰まったもの
    /// </summary>
    protected Props props = null;

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
    // IParticleの実装

    public abstract void Setup();

    /// <summary>
    /// セットアップ
    /// </summary>
    public virtual void Setup(Props props)
    {
      // propsを保持
      this.props = props;

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
    }

    /// <summary>
    /// 発動
    /// </summary>
    public abstract void Fire(Vector3 position);

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
      }
    }

    /// <summary>
    /// 輝度
    /// </summary>
    public float Brightness {
      get { return this.glow.color.a; }
      set {
        this.color.a = value * this.main.color.a; // mainのアルファが下がっていれば輝度も下がる
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
    public float AlphaAcceleration {
      get { return this.props.AlphaAcceleration; }
      set { this.props.AlphaAcceleration = value; }
    }

    /// <summary>
    /// 寿命
    /// </summary>
    public float LifeTime {
      get { return this.props.LifeTime; }
      set { this.props.LifeTime = value; }
    }

    /// <summary>
    /// 速度
    /// </summary>
    public Vector3 Velocity {
      get { return this.props.Velocity; }
      set { this.props.Velocity = value; }
    }
  }
}
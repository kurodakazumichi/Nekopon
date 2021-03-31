using UnityEngine;

namespace MyGame.Unit.Particle
{
  /// <summary>
  /// Particleのベース
  /// </summary>
  public abstract class Base<TState> : Unit<TState>, IParticle where TState : System.Enum
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
    protected Props trace = new Props();

    /// <summary>
    /// トレース用タイマー
    /// </summary>
    protected float traceTimer = 0;

    /// <summary>
    /// 軌跡が有効かどうか
    /// </summary>
    protected bool isTraceEnabled = false;

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
      // SpriteRendererに関するパラメータ設定
      Sprite = props.Sprite;
      LayerName = props.LayerName;

      if (props.MainMaterial) {
        MainMaterial = props.MainMaterial;
      }
      if (props.GlowMaterial) {
        GlowMaterial = props.GlowMaterial;
      }

      Alpha      = props.Alpha;
      Brightness = props.Brightness;

      Velocity             = props.Velocity;
      Gravity              = props.Gravity;
      RotationAcceleration = props.RotationAcceleration;
      AlphaAcceleration    = props.AlphaAcceleration;
      ScaleAcceleration    = props.ScaleAcceleration;
      LifeTime             = props.LifeTime;
      IsSelfDestructive    = props.IsSelfDestructive;

      this.main.gameObject.SetActive(props.MainIsEnabled);
      this.glow.gameObject.SetActive(props.GlowIsEnabled);
    }

    /// <summary>
    /// 痕跡の設定
    /// </summary>
    public void SetTrace(Props props, float time = 0)
    {
      this.isTraceEnabled = (props != null);

      if (props != null) {
        this.trace.Copy(props);
      }
      
      TraceTime = time;
      this.traceTimer = 0;
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
    /// MainSprite
    /// </summary>
    public Sprite MainSprite {
      get { return this.main.sprite; }
      set { this.main.sprite = value; }
    }

    /// <summary>
    /// GlowSprite
    /// </summary>
    public Sprite GlowSprite {
      get { return this.glow.sprite; }
      set { this.glow.sprite = value; }
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
    /// 速度
    /// </summary>
    public Vector3 Velocity { get; set; } = Vector3.zero;

    /// <summary>
    /// 重力
    /// </summary>
    public float Gravity { get; set; } = 0f;

    /// <summary>
    /// スケール加速度
    /// </summary>
    public float ScaleAcceleration { get; set; } = 0f;

    /// <summary>
    /// 回転加速度
    /// </summary>
    public float RotationAcceleration { get; set; } = 0f;

    /// <summary>
    /// アルファ加速度
    /// </summary>
    public float AlphaAcceleration { get; set; } = 0f;

    /// <summary>
    /// 寿命
    /// </summary>
    public float LifeTime { get; set; } = 1f;

    /// <summary>
    /// 痕跡を残す時間
    /// </summary>
    public float TraceTime { get; set; } = 0f;

    /// <summary>
    /// 自分自身で破壊するかどうか
    /// </summary>
    public bool IsSelfDestructive { get; set; } = true;

    //-------------------------------------------------------------------------
    // メソッド

    /// <summary>
    /// 座標の操作
    /// </summary>
    protected virtual void OperatePosition(float deltaTime)
    {
      CacheTransform.position += Velocity * deltaTime;
    }

    /// <summary>
    /// 重力を操作
    /// </summary>
    /// <param name="deltaTime"></param>
    protected virtual void OperateGravity(float deltaTime)
    {
      if (Gravity == 0) return;
      var v = Velocity;
      v.y -= Gravity * deltaTime;
      Velocity = v;
    }

    /// <summary>
    /// スケールの操作
    /// </summary>
    protected virtual void OperateScale(float deltaTime)
    {
      if (ScaleAcceleration == 0) return;

      CacheTransform.localScale += Vector3.one * ScaleAcceleration * deltaTime;

      if (
        CacheTransform.localScale.x < 0 ||
        CacheTransform.localScale.y < 0 ||
        CacheTransform.localScale.z < 0
      ) {
        CacheTransform.localScale = Vector3.zero;
      }
    }

    /// <summary>
    /// 回転の操作
    /// </summary>
    /// <param name="deltaTime"></param>
    protected virtual void OperateRotation(float deltaTime)
    {
      if (RotationAcceleration == 0) return;
      CacheTransform.Rotate(Vector3.forward, RotationAcceleration * deltaTime);
    }

    /// <summary>
    /// アルファの操作
    /// </summary>
    protected virtual void OperateAlpha(float deltaTime)
    {
      if (AlphaAcceleration == 0) return;
      Alpha += AlphaAcceleration * deltaTime;
    }

    /// <summary>
    /// 軌跡の操作
    /// </summary>
    protected virtual void OperateTrace(float deltaTime)
    {
      if (!this.isTraceEnabled) return;

      if (this.traceTimer < 0) {
        var p = ParticleManager.Instance.Create(ParticleManager.Type.Standard);
        p.Setup();
        p.Setup(this.trace);
        p.Fire(CacheTransform.position, CacheTransform.localScale, CacheTransform.rotation);
        this.traceTimer = TraceTime;
      }

      this.traceTimer -= deltaTime;
    }

    /// <summary>
    /// 寿命の操作
    /// </summary>
    protected virtual void OperateLifeTime(float deltaTime)
    {
      if (!IsSelfDestructive) return;
      this.LifeTime -= deltaTime;
    }

    /// <summary>
    /// Idleになる必要があるか
    /// </summary>
    protected virtual bool NeedToIdle {
      get {
        // 自己破壊しない設定の場合は外部から破壊しない限りIdleにならない
        if (!this.IsSelfDestructive) return false;
        if (this.Alpha < 0) return true;
        if (this.LifeTime < this.timer) return true;
        return false;
      }
    }

    /// <summary>
    /// 破壊
    /// </summary>
    public abstract void Destory();
  }
}
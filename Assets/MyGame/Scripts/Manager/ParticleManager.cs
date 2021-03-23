using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Particle;

namespace MyGame
{
  /// <summary>
  /// パーティクルのインターフェース
  /// </summary>
  public interface IParticle : IPoolable
  {
    //-------------------------------------------------------------------------
    // メソッド

    /// <summary>
    /// 標準セットアップ
    /// </summary>
    void Setup();

    /// <summary>
    /// Propsを要するセットアップ
    /// </summary>
    void Setup(Props props);

    /// <summary>
    /// 軌跡の設定
    /// </summary>
    void SetTrace(Props props, float time = 0);

    /// <summary>
    /// 発動
    /// </summary>
    void Fire(Vector3 position, Vector3? scale = null, Quaternion? rotation = null);

    //-------------------------------------------------------------------------
    // プロパティ系

    /// <summary>
    /// Transform
    /// </summary>
    Transform CacheTransform { get; }

    /// <summary>
    /// スプライト
    /// </summary>
    Sprite Sprite { get; set; }

    /// <summary>
    /// 描画レイヤー名
    /// </summary>
    string LayerName { set; }

    /// <summary>
    /// アルファ
    /// </summary>
    float Alpha { set; }

    /// <summary>
    /// 輝度
    /// </summary>
    float Brightness { set; }

    /// <summary>
    /// 速度
    /// </summary>
    Vector3 Velocity { set; }

    /// <summary>
    /// スケーリング速度
    /// </summary>
    float ScaleAcceleration { set; }

    /// <summary>
    /// 回転速度
    /// </summary>
    float RotationAcceleration { set; }

    /// <summary>
    /// 透過速度
    /// </summary>
    float AlphaAcceleration { set; }

    /// <summary>
    /// 寿命
    /// </summary>
    float LifeTime { set; }

    /// <summary>
    /// 自滅するかどうかのフラグ
    /// </summary>
    bool IsSelfDestructive { set; }

    /// <summary>
    /// 軌跡生成時間
    /// </summary>
    float TraceTime { set; }
  }

  public class ParticleManager : SingletonMonoBehaviour<ParticleManager>
  {
    /// <summary>
    /// パーティクルの種類
    /// </summary>
    public enum Type
    {
      Standard,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// オブジェクトプール
    /// </summary>
    private Dictionary<int, ObjectPool<IParticle>> pools = new Dictionary<int, ObjectPool<IParticle>>();

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
#if DEBUG
      ResourceSystem.Instance.Load<Sprite>("Paw.Fir.sprite", null, null, (res) => {
        __Sprite = res;
      });
#endif
      Standard.Load(pre, done);
    }

    public static void Unload()
    {
      Standard.Unload();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // デバッグ登録
      DebugSystem.Instance.Regist(this);

      // ObjectPoolの初期設定
      InitPool<Standard>(Type.Standard);
    }

    protected override void OnMyDestory()
    {
      // デバッグ解除
      DebugSystem.Instance.Discard(this);
    }

    //-------------------------------------------------------------------------
    // 初期化関連

    private void InitPool<T>(Type type) where T : Component, IParticle
    {
      // プール生成
      var pool = new ObjectPool<IParticle>();

      // Generator設定
      pool.SetGenerator(() => {
        return MyGameObject.Create<T>($"{type}", CacheTransform);
      });

      // 登録
      this.pools.Add((int)type, pool);
    }

    //-------------------------------------------------------------------------
    // 生成と解除

    /// <summary>
    /// エフェクトを生成
    /// </summary>
    public IParticle Create(Type type)
    {
      var effect = this.pools[(int)type].Create();
      effect.Setup();
      return effect;
    }

    /// <summary>
    /// エフェクトを解除、使い終わったらエフェクトはReleaseしてプールに戻す
    /// </summary>
    public void Release(Type type, IParticle effect)
    {
      this.pools[(int)type].Release(effect, CacheTransform);
    }

#if _DEBUG

    public static Sprite __Sprite;
    private bool __enableTrace = false;
    private float __traceTime = 0.1f;
    private Props __props = new Props();
    private Props __trace = new Props();

    //-------------------------------------------------------------------------
    // デバッグ
    public override void OnDebug()
    {
      using (new GUILayout.VerticalScope()) {

        // Traceを有効にする
        __enableTrace = GUILayout.Toggle(__enableTrace, "Trace");

        if (!__enableTrace) {
          OnDebugProps(__props);
        } else {
          using (new GUILayout.HorizontalScope()) {
            __traceTime = GUILayout.HorizontalSlider(__traceTime, 0.01f, 1f);
          }
          OnDebugProps(__trace);
        }

        MyEnum.ForEach<Type>((type) => {
          if (GUILayout.Button($"{type}")) {
            var p = Create(type);
            __props.Sprite = __Sprite;
            p.Setup(__props);

            if (__enableTrace) {
              __trace.Sprite = __Sprite;
              p.SetTrace(__trace, __traceTime);
            }

            p.Fire(Vector3.zero);
          }
        });
      }
    }

    private void OnDebugProps(Props props)
    {
      using (new GUILayout.HorizontalScope(GUI.skin.box)) {

        GUIVerticalScope(() => {
          GUILayout.Label("Alpha");
          props.Alpha = GUILayout.HorizontalSlider(props.Alpha, 0, 1f);
        });
        GUIVerticalScope(() => {
          GUILayout.Label("Brightness");
          props.Brightness = GUILayout.HorizontalSlider(props.Brightness, 0, 1f);
        });


      }

      using (new GUILayout.HorizontalScope(GUI.skin.box)) 
      {
        GUIVerticalScope(() => {
          GUILayout.Label("AlphaAcceleration");
          props.AlphaAcceleration
            = GUILayout.HorizontalSlider(props.AlphaAcceleration, -1f, 1f);
        });
        GUIVerticalScope(() => {
          GUILayout.Label("ScaleAcceleration");
          props.ScaleAcceleration
            = GUILayout.HorizontalSlider(props.ScaleAcceleration, -1f, 1f);
        });
        GUIVerticalScope(() => {
          GUILayout.Label("RotationAcceleration");
          props.RotationAcceleration
            = GUILayout.HorizontalSlider(props.RotationAcceleration, -360f, 360f);
        });
      }

      using (new GUILayout.HorizontalScope(GUI.skin.box)) 
      {
        GUIVerticalScope(() => {
          GUILayout.Label("LifeTime");
          props.LifeTime = GUILayout.HorizontalSlider(props.LifeTime, 0f, 10f);
        });
        GUIVerticalScope(() => {
          GUILayout.Label("Velocity");
          props.Velocity.x = GUILayout.HorizontalSlider(props.Velocity.x, -1f, 1f);
        });
        GUIVerticalScope(() => { 
          GUILayout.Label("Gravity");
          props.Gravity = GUILayout.HorizontalSlider(props.Gravity, -1f, 1f);
        });
      }

      using (new GUILayout.HorizontalScope(GUI.skin.box)) {
        props.MainIsEnabled
          = GUILayout.Toggle(props.MainIsEnabled, "MainIsEnabled");

        props.GlowIsEnabled 
          = GUILayout.Toggle(props.GlowIsEnabled, "GlowIsEnabled");

        props.IsSelfDestructive
          = GUILayout.Toggle(props.IsSelfDestructive, "IsSelfDestructive");
      }
    }

    private void GUIVerticalScope(System.Action action)
    {
      using (new GUILayout.VerticalScope()) { action(); }
    }
#endif
  }
}
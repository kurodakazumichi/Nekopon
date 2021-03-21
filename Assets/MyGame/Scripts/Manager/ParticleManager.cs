using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Particle;

namespace MyGame
{
  public class ParticleManager : SingletonMonoBehaviour<ParticleManager>
  {
    /// <summary>
    /// パーティクルのインターフェース
    /// </summary>
    public interface IParticle : IPoolable
    {
      void Setup();
      void Setup(Props props);
      void Fire(Vector3 position);

      Sprite Sprite { set; }
      string LayerName { set; }
      float Alpha { set; }
      float Brightness { set; }


      float AlphaAcceleration { set; }
      Vector3 Velocity { set; }
      float LifeTime { set; }
    }

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
    private float __alphaAcceleration = 0f;
    private float __speed = 0f;
    //-------------------------------------------------------------------------
    // デバッグ
    public override void OnDebug()
    {
      using (new GUILayout.VerticalScope()) {
        GUILayout.Label("AlphaAcceleration");
        __alphaAcceleration = GUILayout.HorizontalSlider(__alphaAcceleration, -1f, 1f);
        GUILayout.Label("Speed");
        __speed = GUILayout.HorizontalSlider(__speed, -1f, 1f);
        MyEnum.ForEach<Type>((type) => {
          if (GUILayout.Button($"{type}")) {
            var p = Create(type);

            var props = new Props(){
              Sprite = __Sprite,
              Velocity = Vector3.right * __speed,
              Brightness = 0.1f,
              AlphaAcceleration = __alphaAcceleration,
            };

            p.Setup(props);
            p.Fire(Vector3.zero);
          }
        });
      }
    }
#endif
  }
}
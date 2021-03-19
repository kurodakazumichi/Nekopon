using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Effect;

namespace MyGame
{
  public class EffectManager : SingletonMonoBehaviour<EffectManager>
  {
    public interface IEffect : IPoolable
    {
      bool IsIdle { get; }

      void Setup();

      void Fire(Vector3 position);
    }

    public enum Type
    {
      Rain,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    private Dictionary<int, ObjectPool<IEffect>> pools = new Dictionary<int, ObjectPool<IEffect>>();

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Rain.Load(pre, done);
    }

    public static void Unload()
    {
      Rain.Unload();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // デバッグ登録
      DebugSystem.Instance.Regist(this);

      // ObjectPoolの初期設定
      InitPool<Rain>(Type.Rain);
      
    }

    protected override void OnMyDestory()
    {
      // デバッグ解除
      DebugSystem.Instance.Discard(this);
    }

    //-------------------------------------------------------------------------
    // 初期化関連

    private void InitPool<T>(Type type) where T : Component, IEffect
    {
      // プール生成
      var pool = new ObjectPool<IEffect>();

      // Generator設定
      pool.SetGenerator(() => {
        return MyGameObject.Create<T>("Effect", CacheTransform);
      });

      // 登録
      this.pools.Add((int)type, pool);
    }

    //-------------------------------------------------------------------------
    // 生成と解除

    /// <summary>
    /// エフェクトを生成
    /// </summary>
    public IEffect Create(Type type)
    {
      var effect = this.pools[(int)type].Create();
      effect.Setup();
      return effect;
    }

    /// <summary>
    /// エフェクトを解除、使い終わったらエフェクトはReleaseしてプールに戻す
    /// </summary>
    public void Release(Type type, IEffect effect)
    {
      this.pools[(int)type].Release(effect, CacheTransform);
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ
    public override void OnDebug()
    {
      using (new GUILayout.HorizontalScope(GUI.skin.box)) {
        if (GUILayout.Button("Rain")) {
          Create(Type.Rain).Fire(Vector3.zero);
        }
      }
    }
#endif
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Effect;

namespace MyGame
{
  public class EffectManager : SingletonMonoBehaviour<EffectManager>
  {
    /// <summary>
    /// エフェクトのインターフェース
    /// </summary>
    public interface IEffect : IPoolable
    {
      /// <summary>
      /// エフェクトの効果発動時に呼びたいアクションを設定可能
      /// </summary>
      System.Action Action { set; }

      /// <summary>
      /// セットアップ可能
      /// </summary>
      void Setup();

      /// <summary>
      /// 位置してして発動可能
      /// </summary>
      void Fire(Vector3 position);
    }

    /// <summary>
    /// エフェクトの種類
    /// </summary>
    public enum Type
    {
      Rain,
      Thunder,
      Ice,
      Leaves,
      Ghost,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// オブジェクトプール
    /// </summary>
    private Dictionary<int, ObjectPool<IEffect>> pools = new Dictionary<int, ObjectPool<IEffect>>();

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Rain.Load(pre, done);
      Thunder.Load(pre, done);
      Ice.Load(pre, done);
      Leaves.Load(pre, done);
      Ghost.Load(pre, done);
    }

    public static void Unload()
    {
      Rain.Unload();
      Thunder.Unload();
      Ice.Unload();
      Leaves.Unload();
      Ghost.Unload();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // デバッグ登録
      DebugSystem.Instance.Regist(this);

      // ObjectPoolの初期設定
      InitPool<Rain>(Type.Rain);
      InitPool<Thunder>(Type.Thunder);
      InitPool<Ice>(Type.Ice);
      InitPool<Leaves>(Type.Leaves);
      InitPool<Ghost>(Type.Ghost);
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
      using (new GUILayout.VerticalScope()) {
        MyEnum.ForEach<Type>((type) => { 
          if (GUILayout.Button($"{type}")) {
            Create(type).Fire(Vector3.zero);
          }
        });
      }
    }
#endif
  }
}
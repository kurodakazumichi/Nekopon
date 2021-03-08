using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace MyGame
{
  public class ResourceManager : SingletonMonoBehaviour<ResourceManager>
  {
    /// <summary>
    /// キャッシュリソース、参照カウンタとリソースの参照を保持するのみ
    /// </summary>
    private class CachedResource
    {
      /// <summary>
      /// 参照カウンタ
      /// </summary>
      public uint Count = 0;

      /// <summary>
      /// リソース
      /// </summary>
      public UnityEngine.Object Resource = null;

      /// <summary>
      /// コンストラクタで参照カウンタを1に設定
      /// </summary>
      /// <param name="resource"></param>
      public CachedResource(UnityEngine.Object resource)
      {
        this.Count    = 1;
        this.Resource = resource;
      }
    }

    /// <summary>
    /// キャッシュ済リソース、keyはリソースのアドレス
    /// </summary>
    private Dictionary<string, CachedResource> cache = new Dictionary<string, CachedResource>();

    protected override void MyStart()
    {
      Debug.Manager.Instance.Regist(this);
    }

    protected override void OnMyDestory()
    {
      Debug.Manager.Instance.Discard(this);
    }

    /// <summary>
    /// リソースの非同期ロードを行う
    /// </summary>
    /// <typeparam name="T">ロードするリソースの種類</typeparam>
    /// <param name="address">Addressable Asssetsで登録したAddress</param>
    /// <param name="pre">リソースロード前に呼ばれるコールバック関数</param>
    /// <param name="post">ロード完了したリソースを受け取る関数</param>
    /// <param name="done">ロード完了時に呼ばれる関数</param>
    public void Load<T>(string address, Action pre, Action done, Action<T> post = null) where T : UnityEngine.Object
    {
      pre();
      Addressables.LoadAssetAsync<T>(address).Completed += op => 
      {
        // ロード完了時コールバックを実行
        post?.Invoke(op.Result);
        done();

        // 未キャッシュであればキャッシュ、キャッシュ済であれば参照カウンタを更新
        if (!this.cache.ContainsKey(address)) {
          this.cache[address] = new CachedResource(op.Result);
        } else {
          this.cache[address].Count++;
        }
      };
    }

    /// <summary>
    /// キャッシュ済のリソースを取得、参照カウンタは変化しない
    /// </summary>
    public T GetCache<T>(string address) where T : UnityEngine.Object
    {
      if (!this.cache.ContainsKey(address)) {
        Debug.Logger.Log($"ResourceManager.LoadSync: {address} is not found.");
        return null;
      }
      else {
        return this.cache[address].Resource as T;
      }
    }

    /// <summary>
    /// リソースの破棄、参照カウンタが0になるまでは実際の破棄は行われない
    /// </summary>
    /// <param name="address">破棄するリソースのアドレス</param>
    public void Unload(string address)
    {
      // リソースが存在しなければ何もしない
      if (!this.cache.ContainsKey(address)) { 
        Debug.Logger.Warn($"ResourceManager.Unload:{address}は読み込まれていないか、すでに破棄されています。");
        return;
      }

      // キャッシュリソースを取得し、参照カウントを下げる
      var cache = this.cache[address];
      cache.Count--;

      // 参照カウントが0であればリソースを解放する
      if (cache.Count == 0) {
        Addressables.Release(cache.Resource);
        this.cache.Remove(address);
      }
    }

#if _DEBUG
    public override void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) {
        Util.ForEach(this.cache, (key, value) => {
          GUILayout.Label($"{key}:{value.Count}");
        });
      }
    }
#endif
  }
}
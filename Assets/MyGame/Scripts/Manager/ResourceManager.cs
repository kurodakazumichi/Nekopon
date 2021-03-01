using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace MyGame
{
  public class ResourceManager : SingletonMonobehaviour<ResourceManager>
  {
    private Dictionary<string, UnityEngine.Object> cache = new Dictionary<string, UnityEngine.Object>();

    /// <summary>
    /// リソースの非同期ロードを行う
    /// </summary>
    /// <typeparam name="T">ロードするリソースの種類</typeparam>
    /// <param name="address">Addressable Asssetsで登録したAddress</param>
    /// <param name="pre">リソースロード前に呼ばれるコールバック関数</param>
    /// <param name="post">ロード完了したリソースを受け取る関数</param>
    /// <param name="done">ロード完了時に呼ばれる関数</param>
    public void Load<T>(string address, Action<T> post, Action pre, Action done)
    {
      // 既に読み込まれていたら何もしない
      if (this.cache.ContainsKey(address)) return;

      pre();
      Addressables.LoadAssetAsync<T>(address).Completed += op => {
        post?.Invoke(op.Result);
        done();
        this.cache[address] = op.Result as UnityEngine.Object;
      };
    }

    public void Unload<T>(T obj)
    {
      Addressables.Release(obj);
    }

    public T LoadSync<T>(string address) where T : UnityEngine.Object
    {
#if _DEBUG
      if (!this.cache.ContainsKey(address)) {
        Debug.Logger.Log($"ResourceManager.LoadSync: {address} is not found.");
      }
#endif
      return this.cache[address] as T;
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace MyGame
{
  public class ResourceManager : SingletonMonobehaviour<ResourceManager>
  {
    /// <summary>
    /// リソースの非同期ロードを行う
    /// </summary>
    /// <typeparam name="T">ロードするリソースの種類</typeparam>
    /// <param name="address">Addressable Asssetsで登録したAddress</param>
    /// <param name="pre">リソースロード前に呼ばれるコールバック関数</param>
    /// <param name="post">ロード完了したリソースを受け取る関数</param>
    /// <param name="done">ロード完了時に呼ばれる関数</param>
    public void Load<T>(string address, Action pre, Action<T> post, Action done)
    {
      pre();
      Addressables.LoadAssetAsync<T>(address).Completed += op => {
        post(op.Result);
        done();
      };
    }
  }
}
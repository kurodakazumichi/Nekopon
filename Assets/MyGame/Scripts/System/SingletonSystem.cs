using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// シングルトンを管理する
  /// </summary>
  public class SingletonSystem : SingletonMonoBehaviour<SingletonSystem>
  {
    public SingletonSystem Regist<T> (GameObject parent) where T : SingletonMonoBehaviour<T>
    {
      var type = typeof(T);
      var go = new GameObject(type.Name);
      go.AddComponent<T>();
      go.transform.parent = parent.transform;
      return this;
    }
  }

}
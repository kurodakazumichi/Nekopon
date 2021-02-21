using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// シングルトンを管理する
  /// </summary>
  public class SingletonManager : SingletonMonobehaviour<SingletonManager>
  {
    override protected void Awake()
    {
      base.Awake();
      DontDestroyOnLoad(this.gameObject);
    }

    public SingletonManager Setup<T> (GameObject parent) where T : SingletonMonobehaviour<T>
    {
      var type = typeof(T);
      var go = new GameObject(type.Name);
      go.AddComponent<T>();
      go.transform.parent = parent.transform;
      return this;
    }
  }

}
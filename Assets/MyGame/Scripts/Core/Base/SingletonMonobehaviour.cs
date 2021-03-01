using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class SingletonMonobehaviour<T> : MyMonoBehaviour where T : MyMonoBehaviour
  {
    private static T instance;

    public static T Instance {
      get {
        if (instance == null) {
          instance = (T)FindObjectOfType(typeof(T));
          if (instance == null) {
            Debug.Logger.Error(typeof(T) + "がシーンに存在しません。");
          }
        }
        return instance;
      }
    }

    public static bool HasInstance => (instance != null);

    protected override void MyAwake()
    {
      if (this != Instance) {
        Debug.Logger.Warn($"{typeof(T).Name} が1回以上生成されるフローが存在します。");
        Destroy(this);
        return;
      }
    }
  }
}
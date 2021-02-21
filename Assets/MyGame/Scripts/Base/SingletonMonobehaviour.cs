using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
  {
    private static T instance;

    public static T Instance {
      get {
        if (instance == null) {
          instance = (T)FindObjectOfType(typeof(T));
          if (instance == null) {
            Debug.LogError(typeof(T) + "がシーンに存在しません。");
          }
        }
        return instance;
      }
    }

    public static bool HasInstance => (instance != null);

    virtual protected void Awake()
    {
      if (this != Instance) {
#if _DEBUG
        Debug.LogWarning($"{typeof(T).Name} が1回以上生成されるフローが存在します。");
#endif
        Destroy(this);
        return;
      }
    }
  }
}
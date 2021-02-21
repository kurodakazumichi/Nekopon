using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class MyMonoBehaviourBase:MonoBehaviour
  {
    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
  }

  public class MyMonoBehaviour : MyMonoBehaviourBase
  {
    /// <summary>
    /// Transformコンポーネントのキャッシュ
    /// </summary>
    [HideInInspector]
    public Transform cacheTransform;

    protected sealed override void Awake()
    {
      MyAwake();
    }

    protected sealed override void Start()
    {
      if (cacheTransform == null) {
        cacheTransform = this.transform;
      }

      MyStart();
    }

    protected sealed override void Update()
    {
      MyUpdate();
    }

    protected virtual void MyAwake() { }
    protected virtual void MyStart() { }
    protected virtual void MyUpdate() { }
  }
}
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

    protected override void Awake()
    {
      if (cacheTransform == null) {
        cacheTransform = this.transform;
      }

      MyAwake();
    }

    protected override void Start()
    {
      MyStart();
    }

    protected override void Update()
    {
      MyUpdate();
    }

    protected virtual void MyAwake() { }
    protected virtual void MyStart() { }
    protected virtual void MyUpdate() { }

    public MyMonoBehaviour SetParent(Transform parent)
    {
      this.cacheTransform.parent = parent;
      return this;
    }

    public MyMonoBehaviour SetActive(bool active)
    {
      this.gameObject.SetActive(active);
      return this;
    }
  }
}
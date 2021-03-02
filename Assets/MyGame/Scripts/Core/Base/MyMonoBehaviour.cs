using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class MyMonoBehaviour : MonoBehaviour
  {
    /// <summary>
    /// Transformコンポーネントのキャッシュ
    /// </summary>
    public Transform cacheTransform { get; private set; }

    protected virtual void Awake()
    {
      if (cacheTransform == null) {
        cacheTransform = this.transform;
      }

      MyAwake();
    }

    protected virtual void Start()
    {
      MyStart();
    }

    protected virtual void Update()
    {
      MyUpdate();
    }

    protected virtual void OnDestroy()
    {
      this.OnMyDestory();
    }

    protected virtual void MyAwake() { }
    protected virtual void MyStart() { }
    protected virtual void MyUpdate() { }

    protected virtual void OnMyDestory() { }

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

#if _DEBUG
    public virtual void OnDebug()
    {

    }
#endif
  }
}
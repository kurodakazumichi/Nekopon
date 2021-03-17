using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// UnityのMonoBehaviourのラッパー
  /// </summary>
  public class MyMonoBehaviour : MonoBehaviour
  {
    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// Transformコンポーネントのキャッシュ
    /// </summary>
    public Transform CacheTransform { get; private set; }

    /// <summary>
    /// 自信がアクティブかどうか
    /// </summary>
    public bool IsActiveSelf => this.gameObject.activeSelf;

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected virtual void Awake()
    {
      if (CacheTransform == null) {
        CacheTransform = this.transform;
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

    protected virtual void OnEnable()
    {
      this.OnMyEnable();
    }

    protected virtual void OnDisable()
    {
      this.OnMyDisable();
    }

    protected virtual void MyAwake() { }
    protected virtual void MyStart() { }
    protected virtual void MyUpdate() { }
    protected virtual void OnMyDestory() { }
    protected virtual void OnMyEnable() { }
    protected virtual void OnMyDisable() { }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 親を設定する
    /// </summary>
    public void SetParent(Transform parent, bool worldPositionStays = true)
    {
      this.CacheTransform.SetParent(parent, worldPositionStays);
    }

    /// <summary>
    /// アクティブを設定する
    /// </summary>
    public void SetActive(bool isActive)
    {
      this.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// コンポーネントを追加する
    /// </summary>
    public T AddComponent<T>() where T : Component
    {
      return this.gameObject.AddComponent<T>();
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    /// <summary>
    /// デバッグ用の基底メソッド
    /// </summary>
    public virtual void OnDebug()
    {

    }
#endif
  }
}
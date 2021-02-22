using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit
{
  public class Unit : MyMonoBehaviour
  {
    /// <summary>
    /// ロード完了フラグ
    /// </summary>
    public bool IsLoaded {
      get; protected set;
    }

    protected override void Start()
    {
      MyStart();
      this.IsLoaded = false;
      StartCoroutine(Load());
    }

    protected virtual IEnumerator Load()
    {
      IsLoaded = true;
      yield break;
    }

    protected override void Update()
    {
      if (!this.IsLoaded) return;

      MyUpdate();
    }
  }
}

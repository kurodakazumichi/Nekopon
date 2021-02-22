﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Scene
{
  public abstract class SceneBase : MyMonoBehaviour
  {
    /// <summary>
    /// ロード完了フラグ
    /// </summary>
    protected bool isLoaded = false;

    protected override void Start()
    {
      MyStart();
      this.isLoaded = false;
      StartCoroutine(Load());
    }

    protected virtual IEnumerator Load()
    {
      isLoaded = true;
      yield break;
    }

    protected override void Update()
    {
      if (!this.isLoaded) return;

      MyUpdate();
    }
  }
}

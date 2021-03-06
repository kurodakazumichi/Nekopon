﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Scene
{
  /// <summary>
  /// シーンの基底クラス
  /// Start→Load→Updateの順で動作する(Updateはロードが完了するまで動かない)
  /// </summary>
  public abstract class SceneBase<TState> : MyMonoBehaviour where TState: System.Enum
  {
    /// <summary>
    /// ロード完了フラグ
    /// </summary>
    protected bool isLoaded = false;

    protected StateMachine<TState> state = new StateMachine<TState>();

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

    protected override void MyUpdate()
    {
      this.state.Update();
    }
  }
}

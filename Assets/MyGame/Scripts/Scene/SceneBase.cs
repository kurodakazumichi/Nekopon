using System.Collections;
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

    protected virtual IEnumerator Load()
    {
      isLoaded = true;
      return null;
    }
  }
}

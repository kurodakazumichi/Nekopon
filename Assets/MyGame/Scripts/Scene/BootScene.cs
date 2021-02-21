﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class BootScene : SceneBase
  {
#if _DEBUG
    // シーン遷移しないフラグ(デバッグ用)
    public bool _NoSceneTransition = false;
#endif

    // Start is called before the first frame update
    protected override void MyStart()
    {
      var system = new GameObject("GlobalSystem");
      // 親要素をDontDestoryしておかないと、DontDestoryされた子であっても消される
      // このDontDestoryOnLoadは大事だよ。
      DontDestroyOnLoad(system);

      // シングルトンを管理するシングルトン
      var singleton = new GameObject("SingletonManager", typeof(SingletonManager));
      singleton.transform.parent = system.transform;

      // 常駐させたいシステムがあればここで生成
      SingletonManager.Instance
        .Setup<TimeManager>(system)
        .Setup<SceneManager>(system);

#if _DEBUG
      if (_NoSceneTransition) {
        return;
      }
#endif
      SceneManager.Instance.LoadSceneAdditive(SceneManager.SceneType.Title);
    }
  }
}


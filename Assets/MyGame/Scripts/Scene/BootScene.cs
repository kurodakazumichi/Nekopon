using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Scene
{
  public class BootScene : SceneBase<BootScene.State>
  {
    public enum State { }
#if _DEBUG
    // シーン遷移しないフラグ(デバッグ用)
    public SceneManager.SceneType _FirstTransitionScene = SceneManager.SceneType.None;
    public int _TargetFrameRate = 60;
#endif

    protected override void MyAwake()
    {
#if _DEBUG
      Application.targetFrameRate = _TargetFrameRate;
#endif
    }

    // Start is called before the first frame update
    protected override void MyStart()
    {
      var system = new GameObject("GlobalSystem");
      // ゲーム終了時に先にManagerが消えてしまうとエラーがでるため、その対策
      DontDestroyOnLoad(system);

      // シングルトンを管理するシングルトン
      var singleton = new GameObject("SingletonManager", typeof(SingletonManager));
      singleton.transform.parent = system.transform;

      // 常駐させたいシステムがあればここで生成
      SingletonManager.Instance
        .Setup<Debug.Manager>(system)
        .Setup<TimeManager>(system)
        .Setup<SceneManager>(system)
        .Setup<ResourceManager>(system)
        .Setup<SoundManager>(system)
        .Setup<InputManager>(system);

#if _DEBUG
      if (_FirstTransitionScene != SceneManager.SceneType.None) {
        SceneManager.Instance.LoadSceneAdditive(_FirstTransitionScene);
        return;
      }
#endif
      SceneManager.Instance.LoadSceneAdditive(SceneManager.SceneType.Title);
    }
  }
}


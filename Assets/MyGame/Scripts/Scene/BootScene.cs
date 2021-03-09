using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Scene
{
  public class BootScene : SceneBase<BootScene.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State { 
      Idle,
      Usual,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

#if _DEBUG
    // シーン遷移しないフラグ(デバッグ用)
    public SceneManager.SceneType _FirstTransitionScene = SceneManager.SceneType.None;
    public int _TargetFrameRate = 60;
#endif

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
#if _DEBUG
      Application.targetFrameRate = _TargetFrameRate;
#endif
      // Stateのセットアップ
      this.state.Add(State.Idle);
      this.state.Add(State.Usual, OnUsualEnter);
      this.state.SetState(State.Idle);

      var system = new GameObject("GlobalSystem");
      // ゲーム終了時に先にManagerが消えてしまうとエラーがでるため、その対策
      DontDestroyOnLoad(system);

      // シングルトンを管理するシングルトン
      var singleton = new GameObject("SingletonManager", typeof(SingletonManager));
      singleton.transform.parent = system.transform;

      // 常駐させたいシステムがあればここで生成
      SingletonManager.Instance
        .Setup<Debug.Manager>(system)
        .Setup<SaveManager>(system)
        .Setup<TimeManager>(system)
        .Setup<SceneManager>(system)
        .Setup<ResourceManager>(system)
        .Setup<SoundManager>(system)
        .Setup<InputManager>(system);
    }

    protected override IEnumerator Load()
    {
      var waitForCount = new WaitForCount();
      System.Action pre  = waitForCount.inc;
      System.Action done = waitForCount.dec;

      SaveManager.Instance.Load(pre, done);

      yield return waitForCount;

      this.isLoaded = true;
      this.state.SetState(State.Usual);
    }

    protected override void MyUpdate()
    {
      base.MyUpdate();
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    private void OnUsualEnter()
    {
      SaveManager.Instance.Init();

      SetupGamePad();

#if _DEBUG
      if (_FirstTransitionScene != SceneManager.SceneType.None) {
        SceneManager.Instance.LoadSceneAdditive(_FirstTransitionScene);
        return;
      }
#endif
      SceneManager.Instance.LoadSceneAdditive(SceneManager.SceneType.Title);
    }

    /// <summary>
    /// ゲームパッドの設定
    /// </summary>
    private void SetupGamePad()
    {
      KeyConfig standard = SaveManager.Instance.GetKeyConfig(Define.App.OperationMethod.Standard);
      JoyConfig x360 = SaveManager.Instance.GetJoyConfig(Define.App.JoyType.X360);
      var im = InputManager.Instance;

      im.GetPad(0).Setup(standard);
      im.GetPad(0).Setup(x360, 0);
    }
  }
}


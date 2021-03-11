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
        .Regist<Debug.Manager>(system)
        .Regist<SaveManager>(system)
        .Regist<TimeManager>(system)
        .Regist<SceneManager>(system)
        .Regist<ResourceManager>(system)
        .Regist<SoundManager>(system)
        .Regist<InputManager>(system);
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
      SaveManager.Instance.Setup();

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
      // Singleton取得
      var sm = SaveManager.Instance;
      var im = InputManager.Instance;

      // GamePadの設定
      InputManagement.GamePad pad;
      
      // 1P GamePadの設定
      pad = im.GetPad(0);
      pad.Init(sm.GetJoyConfig(im.GetJoyType(0)), 0);
      pad.Init(sm.GetKeyConfig(Define.App.OperationMethod.Standard));

      // 2P GamePadの設定
      pad = im.GetPad(1);
      pad.Init(sm.GetJoyConfig(im.GetJoyType(1)), 1);
    }
  }
}


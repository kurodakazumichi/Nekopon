using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;

namespace MyGame.Scene
{

  public class VersusScene : SceneBase<VersusScene.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Setup,
      Ready,
      Go,
      Usual,
      Result,
      Continue,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    // Resource
    private GameObject backGroundPrefab = null;

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // VersusScene用のManagerを入れるオブジェクト
      var system = new GameObject("system");
      system.transform.parent = CacheTransform;

      // 必要なシングルトンがあればココで定義
      SingletonManager.Instance.Setup<VersusManager>(system);

      // 状態のセットアップ
      this.state.Add(State.Idle);
      this.state.Add(State.Setup, OnSetupEnter);
      this.state.Add(State.Usual, null, OnUsualUpdate);
      this.state.SetState(State.Idle);
    }

    protected override void MyStart()
    {

    }

    protected override IEnumerator Load()
    {
      // ロード待機用
      var waitForCount = new WaitForCount();
      System.Action pre  = waitForCount.inc;
      System.Action done = waitForCount.dec;

      // ロード
      var rm = ResourceManager.Instance;
      rm.Load<GameObject>("VS.BackGround.prefab", pre, done, (res) => { this.backGroundPrefab = res; });
      VersusManager.Load(pre, done);

      // ロード待機
      yield return waitForCount;

      // ロード完了
      this.isLoaded = true;
      this.state.SetState(State.Setup);
    }

    protected override void OnMyDestory()
    {
      var rm = ResourceManager.Instance;
      rm.Unload("VS.BackGround.prefab");
      VersusManager.Unload();
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    private void OnSetupEnter()
    {
      var backGround = Instantiate(this.backGroundPrefab);
      backGround.transform.parent = CacheTransform;

      var vs = VersusManager.Instance;
      vs.Setup(backGround);

      this.state.SetState(State.Usual);
    }

    private void OnUsualUpdate()
    {
      VersusManager.Instance.UpdatePlayer();
    }


  }
}

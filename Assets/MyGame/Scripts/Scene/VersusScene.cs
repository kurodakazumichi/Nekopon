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
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    // Resource
    private GameObject backGroundPrefab = null;

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      Debug.Logger.Log("VersusScene.MyAwake");
      // VersusScene用のManagerを入れるオブジェクト
      var system = new GameObject("system");
      system.transform.parent = CacheTransform;

      // 必要なシングルトンがあればココで定義
      SingletonManager.Instance.Setup<VersusManager>(system);
    }

    protected override void MyStart()
    {
      this.state.Add(State.Idle);
      this.state.Add(State.Setup, EnterSetup);
    }

    protected override IEnumerator Load()
    {
      var waitForCount = new WaitForCount();
      System.Action pre  = waitForCount.inc;
      System.Action done = waitForCount.dec;

      var rm = ResourceManager.Instance;
      rm.Load<GameObject>("VS.BackGround.prefab", pre, done, (res) => { this.backGroundPrefab = res; });
      VersusManager.Load(pre, done);

      yield return waitForCount;

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

    private void EnterSetup()
    {
      var backGround = Instantiate(this.backGroundPrefab);
      backGround.transform.parent = CacheTransform;

      var vs = VersusManager.Instance;
      vs.Setup(backGround);
    }

  }
}

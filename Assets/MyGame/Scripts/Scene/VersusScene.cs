using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void MyStart()
    {
      this.state.Add(State.Idle);
      this.state.Add(State.Setup, SetupEnter);
    }

    protected override IEnumerator Load()
    {
      var waitForCount = new WaitForCount();
      System.Action pre  = waitForCount.inc;
      System.Action done = waitForCount.dec;

      var rm = ResourceManager.Instance;
      rm.Load<GameObject>("VS.BackGround.prefab", pre, done, (res) => { this.backGroundPrefab = res; });

      yield return waitForCount;

      this.isLoaded = true;
      this.state.SetState(State.Setup);
    }

    protected override void OnMyDestory()
    {
      var rm = ResourceManager.Instance;
      rm.Unload("VS.BackGround.prefab");
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    private void SetupEnter()
    {
      Instantiate(this.backGroundPrefab).transform.parent = this.transform;
    }

  }
}

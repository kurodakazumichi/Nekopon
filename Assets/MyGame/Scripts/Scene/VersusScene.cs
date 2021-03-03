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
    private GameObject pawPrefab = null;

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
      rm.Load<GameObject>("VS.Paw.prefab", pre, done, (res) => { this.pawPrefab = res; });

      Unit.Versus.Paw.Load(pre, done);
      yield return waitForCount;

      this.isLoaded = true;
      this.state.SetState(State.Setup);
    }

    protected override void OnMyDestory()
    {
      var rm = ResourceManager.Instance;
      rm.Unload("VS.BackGround.prefab");
      rm.Unload("VS.Paw.prefab");
      Unit.Versus.Paw.Unload();
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    private void SetupEnter()
    {
      Transform trans = Instantiate(this.backGroundPrefab).transform;
      trans.parent = CacheTransform;

      Vector3 locP1Paw = trans.Find("P1.Paw").transform.position;
      Vector3 locP2Paw = trans.Find("P2.Paw").transform.position;

      for(int i = 0; i < 12 * 6; ++i) {

        int x = i % 6;
        int y = i / 6;

        var paw = Instantiate(this.pawPrefab).GetComponent<Unit.Versus.Paw>();
        paw.RandomAttribute();
        paw.CacheTransform.position = locP1Paw + new Vector3(Define.Versus.PAW_INTERVAL_X * x, Define.Versus.PAW_INTERVAL_Y * y, 0);
      }

      for (int i = 0; i < 12 * 6; ++i) {

        int x = i % 6;
        int y = i / 6;

        var paw = Instantiate(this.pawPrefab).GetComponent<Unit.Versus.Paw>();
        paw.RandomAttribute();
        paw.CacheTransform.position = locP2Paw + new Vector3(Define.Versus.PAW_INTERVAL_X * x, Define.Versus.PAW_INTERVAL_Y * y, 0);
      }
    }

  }
}

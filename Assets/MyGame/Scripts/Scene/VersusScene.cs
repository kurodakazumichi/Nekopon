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
      Usual,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 背景Prefab
    /// </summary>
    private GameObject backGroundPrefab = null;

    /// <summary>
    /// SceneManager
    /// </summary>
    private SceneSystem scene = null;

    /// <summary>
    /// VersusManager
    /// </summary>
    private VersusManager vs = null;

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // VersusScene用のManagerを入れるオブジェクト
      var system = new GameObject("system");
      system.transform.parent = CacheTransform;

      // 必要なシングルトンがあればココで定義
      SingletonSystem.Instance
        .Regist<PawEffectManager>(system)
        .Regist<VersusManager>(system);
      

      // 状態のセットアップ
      this.state.Add(State.Idle);
      this.state.Add(State.Setup, OnSetupEnter);
      this.state.Add(State.Usual, null, OnUsualUpdate);
      this.state.SetState(State.Idle);
    }

    protected override void MyStart()
    {
      this.scene = SceneSystem.Instance;
      this.vs    = VersusManager.Instance;
    }

    protected override IEnumerator Load()
    {
      // ロード待機用
      var waitForCount = new WaitForCount();
      System.Action pre  = waitForCount.inc;
      System.Action done = waitForCount.dec;

      // ロード
      var rm = ResourceSystem.Instance;
      rm.Load<GameObject>("VS.BackGround.prefab", pre, done, (res) => { this.backGroundPrefab = res; });
      PawEffectManager.Load(pre, done);
      VersusManager.Load(pre, done);

      // ロード待機
      yield return waitForCount;

      // ロード完了
      this.isLoaded = true;
      this.state.SetState(State.Setup);
    }

    protected override void OnMyDestory()
    {
      var rm = ResourceSystem.Instance;
      rm.Unload("VS.BackGround.prefab");
      PawEffectManager.Unload();
      VersusManager.Unload();

    }

    //-------------------------------------------------------------------------
    // ステートマシン

    private void OnSetupEnter()
    {
      // 背景を生成
      var backGround = Instantiate(this.backGroundPrefab);
      backGround.transform.parent = CacheTransform;

      // VersusManagerをセットアップしてあとはお任せ
      this.vs.Setup(backGround);
      this.state.SetState(State.Usual);
    }

    private void OnUsualUpdate()
    {
      // 対戦が終わるまでループ
      if (!this.vs.Move()) {
        this.scene.UnloadSceneAsync(SceneSystem.SceneType.Versus, () => {
          this.scene.LoadSceneAdditive(SceneSystem.SceneType.Title);
        });
      }
    }

  }
}

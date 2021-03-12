using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Title;
using MyGame.Unit.Cursor;
using MyGame.InputManagement;

namespace MyGame.Scene
{
  /// <summary>
  /// タイトルシーン、タイトルシーンに必要なリソース、オブジェクトの管理と制御
  /// </summary>
  public class TitleScene : SceneBase<TitleScene.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Setup,
      Intro,
      MenuSelection,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    // Resource
    private GameObject logoPrefab = null;
    private List<GameObject> menuPrefabs = new List<GameObject>();
    private GameObject cursorPrefab = null;
    private GameObject effectPrefab = null;
    
    // GameObject
    private TitleLogo logo = null;
    private List<Menu> menus = new List<Menu>();
    private Paw cursor = null;
    private EffectGenerator effect = null;

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyStart()
    {
      this.state.Add(State.Idle);
      this.state.Add(State.Setup, OnSetupEnter, null, null);
      this.state.Add(State.Intro, OnIntroEnter, OnIntroUpdate, null);
      this.state.Add(State.MenuSelection, OnMenuSelectionEnter, OnMenuSelectionUpdate, null);
    }

    protected override IEnumerator Load()
    {
      var waitForCount = new WaitForCount();
      System.Action pre  = waitForCount.inc;
      System.Action done = waitForCount.dec;

      var rm = ResourceSystem.Instance;
      rm.Load<GameObject>("Title.Logo.prefab", pre, done, (res) => { this.logoPrefab = res; });
      rm.Load<GameObject>("Title.MenuCpu.prefab", pre, done, (res) => { this.menuPrefabs.Add(res);  });
      rm.Load<GameObject>("Title.MenuVs.prefab", pre, done, (res) => { this.menuPrefabs.Add(res);  });
      rm.Load<GameObject>("Title.MenuDemo.prefab", pre, done, (res) => { this.menuPrefabs.Add(res);  });
      rm.Load<GameObject>("Title.MenuOption.prefab", pre, done, (res) => { this.menuPrefabs.Add(res);  });
      rm.Load<GameObject>("Title.EffectGenerator.prefab", pre, done, (res) => { this.effectPrefab = res; });
      rm.Load<GameObject>("Cursor.CatPaw.prefab", pre, done, (res) => { this.cursorPrefab = res; });
      rm.Load<AudioClip>("BGM.001", pre, done);
      rm.Load<AudioClip>("SE.Select01", pre, done);
      TitleLogo.Load(pre, done);
      Menu.Load(pre, done);
      Paw.Load(pre, done);
      EffectGenerator.Load(pre, done);

      yield return waitForCount;

      this.isLoaded = true;
      this.state.SetState(State.Setup);
    }

    protected override void OnMyDestory()
    {
      SoundSystem.Instance.StopBGM();
      var rm = ResourceSystem.Instance;
      rm.Unload("Title.Logo.prefab");
      rm.Unload("Title.MenuCpu.prefab");
      rm.Unload("Title.MenuVs.prefab");
      rm.Unload("Title.MenuDemo.prefab");
      rm.Unload("Title.MenuOption.prefab");
      rm.Unload("Title.EffectGenerator.prefab");
      rm.Unload("Cursor.CatPaw.prefab");
      rm.Unload("BGM.001");
      rm.Unload("SE.Select01");

      TitleLogo.Unload();
      Menu.Unload();
      Paw.Unload();
      EffectGenerator.Unload();
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    /// <summary>
    /// リソースからゲームオブジェクトを生成する
    /// </summary>
    private void OnSetupEnter()
    {
      // タイトルロゴ
      this.logo = Instantiate(this.logoPrefab).GetComponent<TitleLogo>();
      this.logo.SetParent(this.CacheTransform).SetActive(false);

      // カーソル
      this.cursor = Instantiate(this.cursorPrefab).GetComponent<Paw>();
      this.cursor.SetParent(this.CacheTransform).SetActive(false);
      this.cursor.PadNo = 0;

      // メニュー
      this.menuPrefabs.ForEach((prefab) => {
        var menu = Instantiate(prefab).GetComponent<Menu>();
        menu.SetParent(this.CacheTransform).SetActive(false);
        menu.SceneType = SceneSystem.SceneType.Title;
        this.menus.Add(menu);
      });

      // エフェクト
      this.effect = Instantiate(this.effectPrefab).GetComponent<EffectGenerator>();
      this.effect.SetParent(this.CacheTransform);

      // Introへ
      this.state.SetState(State.Intro);
    }

    /// <summary>
    /// タイトルシーンの最初の状態
    /// </summary>
    private void OnIntroEnter()
    {
      // BGM再生
      SoundSystem.Instance.PlayBGM("BGM.001");

      // タイトルロゴをバウンド状態へ、バウンド完了時に入力待ちになるようにコールバックを設定
      this.logo.SetActive(true);
      this.logo.ToBound();
      this.logo.CompletedBound = () => { this.state.SetState(State.MenuSelection); };

      // カーソルの初期設定
      this.cursor.SetType(Paw.Type.White);
      this.cursor.PadNo = 0;

      // エフェクト開始
      this.effect.ToUsual();
    }

    /// <summary>
    /// 何かしら入力があったらInputWaitへ遷移する
    /// </summary>
    private void OnIntroUpdate()
    {
      if (InputManager.Instance.GetCommand(Command.PressAnyButton, 0).IsFixed) {
        this.logo.CompletedBound = null;
        this.logo.ToUsual();
        this.state.SetState(State.MenuSelection);
      }
    }

    /// <summary>
    /// メニューの表示
    /// </summary>
    private void OnMenuSelectionEnter()
    {
      // カーソルを操作可能にする
      this.cursor.SetActive(true);
      this.cursor.ToOperable();
      
      this.menus.ForEach((menu) => { 
        menu.SetActive(true);
      });
    }

    /// <summary>
    /// メニュー選択
    /// </summary>
    private void OnMenuSelectionUpdate()
    {
      TryChangeReservedScene();
    }

    private void TryChangeReservedScene()
    {
      // 決定ボタンがおされていなければシーン変更しない
      if (!InputManager.Instance.GetCommand(Command.Decide, 0).IsFixed) return;

      // シーン予約がなければシーン遷移しない
      if (!SceneSystem.Instance.HasReservedScene) return;

      SoundSystem.Instance.PlaySE("SE.Select01");

      // タイトルシーンを破棄して予約シーンへ遷移
      SceneSystem.Instance.UnloadSceneAsync(SceneSystem.SceneType.Title, () => { 
        SceneSystem.Instance.LoadReservedSceneAdditive();
      });

      this.state.SetState(State.Idle);
    }
  }
}
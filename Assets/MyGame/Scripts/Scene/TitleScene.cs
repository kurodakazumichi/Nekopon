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
    private GameObject titleLogoPrefab = null;
    private List<GameObject> menuPrefabs = new List<GameObject>();
    private GameObject cursorPrefab = null;
    
    // GameObject
    private TitleLogo titleLogo = null;
    private List<Menu> menus = new List<Menu>();
    private CatPaw cursor = null;

    //-------------------------------------------------------------------------
    // メソッド

    protected override void MyStart()
    {
      this.state.Add(State.Idle);
      this.state.Add(State.Setup, SetupEnter, null, null);
      this.state.Add(State.Intro, IntroEnter, IntroUpdate, null);
      this.state.Add(State.MenuSelection, MenuSelectionEnter, MenuSelectionUpdate, null);
    }

    protected override IEnumerator Load()
    {
      var waitForCount = new WaitForCount();
      System.Action pre  = waitForCount.inc;
      System.Action done = waitForCount.dec;

      var rm = ResourceManager.Instance;
      rm.Load<GameObject>("TitleLogo"   , pre, done, (res) => { this.titleLogoPrefab = res; });
      rm.Load<GameObject>("CursorCatPaw", pre, done, (res) => { this.cursorPrefab = res;    });
      rm.Load<GameObject>("MenuCpu"     , pre, done, (res) => { this.menuPrefabs.Add(res);  });
      rm.Load<GameObject>("MenuVs"      , pre, done, (res) => { this.menuPrefabs.Add(res);  });
      rm.Load<GameObject>("MenuDemo"    , pre, done, (res) => { this.menuPrefabs.Add(res);  });
      rm.Load<GameObject>("MenuOption"  , pre, done, (res) => { this.menuPrefabs.Add(res);  });
      rm.Load<Sprite>("SpriteCursorCat01", pre, done);
      rm.Load<Sprite>("SpriteCursorCat02", pre, done);
      rm.Load<AudioClip>("BGM_001", pre, done);
      rm.Load<AudioClip>("SE_Bound001", pre, done);

      yield return waitForCount;

      this.isLoaded = true;
      this.state.SetState(State.Setup);
    }

    protected override void OnMyDestory()
    {
      var rm = ResourceManager.Instance;
      rm.Unload("TitleLogo");
      
    }

    /// <summary>
    /// リソースからゲームオブジェクトを生成する
    /// </summary>
    private void SetupEnter()
    {
      // タイトルロゴ
      this.titleLogo = Instantiate(this.titleLogoPrefab).GetComponent<TitleLogo>();
      this.titleLogo.SetParent(this.cacheTransform).SetActive(false);

      // カーソル
      this.cursor = Instantiate(this.cursorPrefab).GetComponent<CatPaw>();
      this.cursor.SetParent(this.cacheTransform).SetActive(false);
      this.cursor.PadNo = 0;

      // メニュー
      this.menuPrefabs.ForEach((prefab) => {
        var menu = Instantiate(prefab).GetComponent<Menu>();
        menu.SetParent(this.cacheTransform).SetActive(false);
        menu.SceneType = SceneManager.SceneType.Title;
        this.menus.Add(menu);
      });

      // Introへ
      this.state.SetState(State.Intro);
    }

    /// <summary>
    /// タイトルシーンの最初の状態
    /// </summary>
    private void IntroEnter()
    {
      // BGM再生
      SoundManager.Instance.PlayBGM("BGM_001");

      // タイトルロゴをバウンド状態へ、バンド完了時に入力待ちになるようにコールバックを設定
      this.titleLogo.SetActive(true);
      this.titleLogo.SetBound();
      this.titleLogo.CompletedBound = () => { this.state.SetState(State.MenuSelection); };

      // カーソルの設定
      this.cursor.SetStateMovable();
      this.cursor.SetType(CatPaw.Type.White);
      this.cursor.SetActive(true);
      this.cursor.PadNo = 0;
    }

    /// <summary>
    /// 何かしら入力があったらInputWaitへ遷移する
    /// </summary>
    private void IntroUpdate()
    {
      if (InputManager.Instance.GetCommand(Command.PressAnyButton, 0).IsFixed) {
        this.titleLogo.CompletedBound = null;
        this.titleLogo.SetFixed();
        this.state.SetState(State.MenuSelection);
      }
    }

    /// <summary>
    /// メニューの表示
    /// </summary>
    private void MenuSelectionEnter()
    {
      this.menus.ForEach((menu) => { 
        menu.SetActive(true);
      });
    }

    /// <summary>
    /// メニュー選択
    /// </summary>
    private void MenuSelectionUpdate()
    {
      TryChangeReservedScene();
    }

    private void TryChangeReservedScene()
    {
      // 決定ボタンがおされていなければシーン変更しない
      if (!InputManager.Instance.GetCommand(Command.Decide, 0).IsFixed) return;

      // シーン予約がなければシーン遷移しない
      if (!SceneManager.Instance.HasReservedScene) return;

      // タイトルシーンを破棄して予約シーンへ遷移
      SceneManager.Instance.UnloadSceneAsync(SceneManager.SceneType.Title, () => { 
        SceneManager.Instance.LoadReservedSceneAdditive();
      });

      this.state.SetState(State.Idle);
    }
  }
}
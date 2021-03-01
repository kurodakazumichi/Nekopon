using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Title;
using MyGame.Unit.Cursor;

namespace MyGame.Scene
{
  public class TitleScene : SceneBase<TitleScene.State>
  {
    public enum State
    {
      Setup,
      Intro,
      InputWait,
    }

    private GameObject titleLogoPrefab = null;
    private List<GameObject> menuPrefabs = new List<GameObject>();
    private GameObject cursorPrefab = null;
    
    private TitleLogo titleLogo = null;
   
    private List<Menu> menus = new List<Menu>();
    private CatPaw cursor = null;

    

    protected override void MyStart()
    {
      this.state.Add(State.Setup, SetupEnter, SetupUpdate, null);
      this.state.Add(State.Intro, IntroEnter, IntroUpdate, null);
      this.state.Add(State.InputWait, InputWaitEnter, InputWaitUpdate, null);
    }

    protected override IEnumerator Load()
    {
      var waitForCount = new WaitForCount();
      System.Action pre  = waitForCount.inc;
      System.Action done = waitForCount.dec;

      var rm = ResourceManager.Instance;
      rm.Load<GameObject>("TitleLogo"   , (res) => { this.titleLogoPrefab = res; }, pre, done);
      rm.Load<GameObject>("MenuCpu"     , (res) => { this.menuPrefabs.Add(res);  }, pre, done);
      rm.Load<GameObject>("MenuVs"      , (res) => { this.menuPrefabs.Add(res);  }, pre, done);
      rm.Load<GameObject>("MenuDemo"    , (res) => { this.menuPrefabs.Add(res);  }, pre, done);
      rm.Load<GameObject>("MenuOption"  , (res) => { this.menuPrefabs.Add(res);  }, pre, done);
      rm.Load<GameObject>("CursorCatPaw", (res) => { this.cursorPrefab = res;    }, pre, done);
      rm.Load<Sprite>("SpriteCursorCat01", null, pre, done);
      rm.Load<Sprite>("SpriteCursorCat02", null, pre, done);

      SoundManager.Instance.Load("BGM_001", pre, done);
      SoundManager.Instance.Load("SE_Bound001", waitForCount.inc, waitForCount.dec);

      yield return waitForCount;

      this.isLoaded = true;
      this.state.SetState(State.Setup);
    }

    protected override void MyUpdate()
    {
      this.state.Update();
    }

    private void SetupEnter()
    {
      this.titleLogo = Instantiate(this.titleLogoPrefab).GetComponent<TitleLogo>();
      this.titleLogo.SetParent(this.cacheTransform).SetActive(false);

      this.menuPrefabs.ForEach((prefab) => {
        var menu = Instantiate(prefab).GetComponent<Menu>();
        menu.SetParent(this.cacheTransform).SetActive(false);
        this.menus.Add(menu);
      });

      this.cursor = Instantiate(this.cursorPrefab).GetComponent<CatPaw>();
      this.cursor.SetParent(this.cacheTransform).SetActive(false);

    }

    private void SetupUpdate() {
      this.state.SetState(State.Intro);
    }



    private void IntroEnter()
    {
      SoundManager.Instance.PlayBGM("BGM_001");
      this.titleLogo.SetActive(true);
      this.titleLogo.SetBound();
      this.titleLogo.CompletedBound = () => { this.state.SetState(State.InputWait); };
      this.cursor.SetStateMovable();
      this.cursor.SetType(CatPaw.Type.White);
      this.cursor.SetActive(true);
      this.cursor.PadNo = 0;
    }

    private void IntroUpdate()
    {
      if (InputManager.Instance.GetCommand(InputManagement.Command.Decide, 0).IsFixed) {
        this.titleLogo.CompletedBound = null;
        this.titleLogo.SetFixed();
        this.state.SetState(State.InputWait);
      }
    }

    private void InputWaitEnter()
    {
      this.menus.ForEach((menu) => { 
        menu.SetActive(true);
      });
    }

    private void InputWaitUpdate()
    {
      //if (Input.GetMouseButtonDown(0)) {
      //  UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Title").completed += op => {
      //    Debug.Log(op);
      //  };
      //}
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Title;

namespace MyGame.Scene
{
  public class TitleScene : SceneBase
  {
    private enum State
    {
      Setup,
      Intro,
      InputWait,
    }

    private GameObject titleLogoPrefab = null;
    private List<GameObject> menuPrefabs = new List<GameObject>();

    private TitleLogo titleLogo = null;
    private List<Menu> menus = new List<Menu>();

    private StateMachine<State> state;

    protected override void MyStart()
    {
      this.state = new StateMachine<State>();
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
      rm.Load<GameObject>("TitleLogo" , pre, (res) => { this.titleLogoPrefab = res; }, done);
      rm.Load<GameObject>("MenuCpu"   , pre, (res) => { this.menuPrefabs.Add(res);  }, done);
      rm.Load<GameObject>("MenuVs"    , pre, (res) => { this.menuPrefabs.Add(res);  }, done);
      rm.Load<GameObject>("MenuDemo"  , pre, (res) => { this.menuPrefabs.Add(res);  }, done);
      rm.Load<GameObject>("MenuOption", pre, (res) => { this.menuPrefabs.Add(res);  }, done);

      SoundManager.Instance.Load("BGM_001", pre, done);
      
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
      this.titleLogo = Instantiate(this.titleLogoPrefab).GetComponent<Unit.Title.TitleLogo>();
      this.titleLogo.SetParent(this.cacheTransform).SetActive(false);

      this.menuPrefabs.ForEach((prefab) => {
        var menu = Instantiate(prefab).GetComponent<Menu>();
        menu.SetParent(this.cacheTransform).SetActive(false);
        this.menus.Add(menu);
      });
    }

    private void SetupUpdate()
    {
      if (this.titleLogo.IsLoaded) return;

      this.state.SetState(State.Intro);
    }



    private void IntroEnter()
    {
      SoundManager.Instance.PlayBGM("BGM_001");
      this.titleLogo.SetActive(true);
      this.titleLogo.SetBound();
      this.titleLogo.CompletedBound = () => { this.state.SetState(State.InputWait); };
    }

    private void IntroUpdate()
    {
      // TODO: InputManagerできたら差し替え
      if (Input.GetMouseButtonDown(0)) {
        this.titleLogo.CompletedBound = null;
        this.titleLogo.SetFixed();
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
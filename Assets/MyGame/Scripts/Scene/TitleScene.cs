using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MyGame.Scene
{
  public class TitleScene : SceneBase
  {
    private GameObject titleLogoPrefab = null;
    private List<GameObject> menuPrefabs = new List<GameObject>();

    private Title.TitleLogo titleLogo = null;
    private List<Title.Menu> menus = new List<Title.Menu>();

    private enum State
    {
      LoadWait,
      Intro,
      InputWait,
    }

    private StateMachine<State> state;

    protected override void MyStart()
    {
      this.state = new StateMachine<State>();
      this.state.Add(State.LoadWait, null, LoadUpdate, LoadExit);
      this.state.Add(State.Intro, IntroEnter, IntroUpdate, null);
      this.state.Add(State.InputWait, InputWaitEnter, InputWaitUpdate, null);

      this.isLoaded = false;
      StartCoroutine(Load());
      this.state.SetState(State.LoadWait);
    }

    protected override IEnumerator Load()
    {
      int count = 0;
      System.Action pre  = () => { count++; };
      System.Action done = () => { count--; };

      var rm = ResourceManager.Instance;
      rm.Load<GameObject>("TitleLogo" , pre, (res) => { this.titleLogoPrefab  = res; }, done);
      rm.Load<GameObject>("MenuCpu"   , pre, (res) => { this.menuPrefabs.Add(res);   }, done);
      rm.Load<GameObject>("MenuVs"    , pre, (res) => { this.menuPrefabs.Add(res);   }, done);
      rm.Load<GameObject>("MenuDemo"  , pre, (res) => { this.menuPrefabs.Add(res);   }, done);
      rm.Load<GameObject>("MenuOption", pre, (res) => { this.menuPrefabs.Add(res);   }, done);

      while (0 < count) {
        yield return  null;
      }

      this.isLoaded = true;
    }

    protected override void MyUpdate()
    {
      this.state.Update();
    }

    private void LoadUpdate()
    {
      if (this.isLoaded) {
        this.state.SetState(State.Intro);
      }
    }

    private void LoadExit()
    {
      this.titleLogo = Instantiate(this.titleLogoPrefab).GetComponent<Title.TitleLogo>();
      this.titleLogo.SetParent(this.cacheTransform).SetActive(false);
      
      this.menuPrefabs.ForEach((prefab) => { 
        var menu = Instantiate(prefab).GetComponent<Title.Menu>();
        menu.SetParent(this.cacheTransform).SetActive(false);
        this.menus.Add(menu);
      });
    }

    private void IntroEnter()
    {
      this.titleLogo.SetActive(true);
      this.titleLogo.SetBound();
      this.titleLogo.CompletedBound = () => { this.state.SetState(State.InputWait); };
    }

    private void IntroUpdate()
    {
      // TODO: InputManagerできたら差し替え
      if (Input.GetMouseButtonDown(0)) {
        this.titleLogo.SetFixed();
        this.titleLogo.CompletedBound = null;
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
      if (Input.GetMouseButtonDown(0)) {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Title").completed += op => {
          Debug.Log(op);
        };
      }
    }
  }
}
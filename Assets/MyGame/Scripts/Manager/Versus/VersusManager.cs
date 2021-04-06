using MyGame.Define;
using MyGame.VersusManagement;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Versus;

namespace MyGame
{
  public class VersusManager : SingletonMonoBehaviour<VersusManager>
  {
    /// <summary>
    /// 状態
    /// </summary>
    private enum State
    {
      Idle,
      Ready,
      Go,
      Usual,
      Result,
      Continue,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    private readonly StateMachine<State> state = new StateMachine<State>();

    /// <summary>
    /// 対戦画面の各種ロケーション
    /// </summary>
    private readonly Dictionary<App.Player, Location> locations = new Dictionary<App.Player, Location>();

    /// <summary>
    /// プレイヤー１
    /// </summary>
    private Player p1 = null;

    /// <summary>
    /// プレイヤー２
    /// </summary>
    private Player p2 = null;

    /// <summary>
    /// ガイドユニット
    /// </summary>
    private Guide guide = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Player.Load(pre, done);
      Puzzle.Load(pre, done);
      Guide.Load(pre, done);
    }

    public static void Unload()
    {
      Player.Unload();
      Puzzle.Unload();
      Guide.Unload();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      DebugSystem.Instance.Regist(this);

      // 状態のセットアップ
      this.state.Add(State.Idle);
      this.state.Add(State.Ready, OnReadyEnter, OnReadyUpdate);
      this.state.Add(State.Go, OnGoEnter, OnGoUpdate);
      this.state.Add(State.Usual, OnUsualEnter, OnUsualUpdate);
      this.state.Add(State.Result, OnResultEnter, OnResultUpdate);
      this.state.Add(State.Continue, OnContinueEnter, OnContinueUpdate);
      this.state.SetState(State.Idle);
    }

    protected override void OnMyDestory()
    {
      DebugSystem.Instance.Discard(this);
    }

    //-------------------------------------------------------------------------
    // 生成・準備に関するモノ

    /// <summary>
    /// 対戦に要するオブジェクトの準備、ロケーション情報を持ったオブジェクトを受け取る。
    /// </summary>
    public void Setup(GameObject locations)
    {
      // ロケーション生成
      this.locations.Add(App.Player.P1, new Location("P1", "P2", locations));
      this.locations.Add(App.Player.P2, new Location("P2", "P1", locations));

      // プレイヤーを生成し、脳を設定
      this.p1 = CreatePlayer(App.Player.P1);
      this.p2 = CreatePlayer(App.Player.P2);
      this.p1.SetBrain(BrainFactory.Create(App.Brain.Player, this.p1, this.p2));
      this.p2.SetBrain(BrainFactory.Create(App.Brain.Player, this.p1, this.p2));

      // ガイド生成
      this.guide = new GameObject("Guide").AddComponent<Guide>();
      this.guide.SetParent(CacheTransform);
      this.guide.Init(
        this.locations[App.Player.P1].Center,
        this.locations[App.Player.P2].Center
      );
      this.guide.Setup();

      this.state.SetState(State.Ready);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    private void OnReadyEnter()
    {
      // プレイヤーの準備(肉球がぱらぱらふってくる)
      PreparePlayers();

      // Ready!を表示
      this.guide.ToReady();
    }

    private void OnReadyUpdate()
    {
      // Ready表示が終わるのを待機
      if (!this.guide.IsFinished) return;

      // GO!のステートへ
      this.state.SetState(State.Go);
    }

    private void OnGoEnter()
    {
      // Go!を表示
      this.guide.ToGo();
    }

    private void OnGoUpdate()
    {
      // Go!の表示が終わるのを待つ
      if (!this.guide.IsFinished) return;

      // ガイドをアイドル状態にし、プレイヤーを始動
      this.guide.ToIdle();
      Instance.StartPlayers();

      // 通常(対戦状態)へ
      this.state.SetState(State.Usual);
    }

    private void OnUsualEnter()
    {
    }

    private void OnUsualUpdate()
    {
      UpdatePlayers();
    }

    private void OnResultEnter()
    {
    }

    private void OnResultUpdate()
    {

    }

    private void OnContinueEnter()
    {

    }

    private void OnContinueUpdate()
    {

    }

    public bool Move()
    {
      this.state.Update();
      return true;
    }

    //-------------------------------------------------------------------------
    // プレイヤーに関すること

    /// <summary>
    /// プレイヤーを生成
    /// </summary>
    private Player CreatePlayer(App.Player type)
    {
      Player.Props props = new Player.Props {
        Type     = type,
        Parent   = CacheTransform,
        Location = this.locations[type],
        Config   = SaveSystem.Instance.GetPlayerConfig(type),
        CatType  = App.Cat.Shiro,
      };

      return new Player(props).Init();
    }

    /// <summary>
    /// プレイヤーの準備を整える
    /// </summary>
    private void PreparePlayers()
    {
      DoPlayers((p) => { p.Setup(); });
    }

    /// <summary>
    /// プレイヤーを始動
    /// </summary>
    private void StartPlayers()
    {
      DoPlayers((p) => { p.Start(); });
    }

    /// <summary>
    /// プレイヤーの更新
    /// </summary>
    private void UpdatePlayers()
    {
      DoPlayers((p) => { p.Update(); });
    }

    /// <summary>
    /// 同時にプレイヤー2人のメソッドを実行したい事が多いけど
    /// わざわざループする量でもないので愚直に呼び出すヘルパー
    /// </summary>
    private void DoPlayers(System.Action<Player> func)
    {
      func(this.p1);
      func(this.p2);
    }

    /// <summary>
    /// 指定したプレイヤーを取得する
    /// </summary>
    public Player GetPlayer(App.Player type)
    {
      return (type == App.Player.P1)? this.p1 : this.p2;
    }

    /// <summary>
    /// 指定されたプレイヤーの対になる相手を取得する
    /// </summary>
    public Player GetTargetPlayerBy(App.Player type)
    {
      return (type == App.Player.P1)? this.p2 : this.p1;
    }

#if _DEBUG
    public override void OnDebug()
    {
      using (new GUILayout.HorizontalScope(GUI.skin.box)) {
        DoPlayers((p) => { p.OnDebug(); });
      }
    }
#endif
  }

}

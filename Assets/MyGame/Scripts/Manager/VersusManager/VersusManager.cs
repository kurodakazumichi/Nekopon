using MyGame.Define;
using MyGame.VersusManagement;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class VersusManager : SingletonMonoBehaviour<VersusManager>
  {
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

    private StateMachine<State> state = new StateMachine<State>();

    /// <summary>
    /// 対戦画面の各種ロケーション
    /// </summary>
    private Dictionary<App.Player, Location> locations = new Dictionary<App.Player, Location>();

    /// <summary>
    /// プレイヤー１
    /// </summary>
    private Player p1 = null;

    /// <summary>
    /// プレイヤー２
    /// </summary>
    private Player p2 = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Player.Load(pre, done);
      Puzzle.Load(pre, done);
    }

    public static void Unload()
    {
      Player.Unload();
      Puzzle.Unload();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      Debug.Manager.Instance.Regist(this);

      // 状態のセットアップ
      this.state.Add(State.Idle);
      this.state.Add(State.Ready, OnReadyEnter, OnReadyUpdate);
      this.state.Add(State.Go, OnGoEnter, OnGoUpdate);
      this.state.Add(State.Usual, OnUsualEnter, OnUsualUpdate);
      this.state.Add(State.Result, OnResultEnter, OnResultUpdate);
      this.state.Add(State.Continue, OnContinueEnter, OnContinueUpdate);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // 生成・準備に関するモノ

    /// <summary>
    /// 対戦に要するオブジェクトの準備、ロケーション情報を持ったオブジェクトを受け取る。
    /// </summary>
    public void Setup(GameObject locations)
    {
      this.locations.Add(App.Player.P1, new Location("P1", locations));
      this.locations.Add(App.Player.P2, new Location("P2", locations));

      this.p1 = CreatePlayer(App.Player.P1);
      this.p2 = CreatePlayer(App.Player.P2);

      this.state.SetState(State.Ready);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    private void OnReadyEnter()
    {
      PreparePlayers();
    }

    private void OnReadyUpdate()
    {
      this.state.SetState(State.Go);
    }

    private void OnGoEnter()
    {
      Instance.StartPlayers();
    }

    private void OnGoUpdate()
    {
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
      Player.Props props = new Player.Props();
      props.Type     = type;
      props.Parent   = CacheTransform;
      props.Location = this.locations[type];
      props.Config   = SaveManager.Instance.GetPlayerConfig(type);

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
    /// プレイヤーに対して攻撃をする
    /// </summary>
    public void AttackPlayer(Player attacker)
    {
      GetTargetPlayerBy(attacker).AcceptAttack(attacker);
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
    /// 指定されたプレイヤーの対になる相手を取得する
    /// </summary>
    private Player GetTargetPlayerBy(Player player)
    {
      return (player.Type == App.Player.P1)? p2 : p1;
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

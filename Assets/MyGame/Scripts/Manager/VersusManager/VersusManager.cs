using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.VersusManagement;

namespace MyGame
{
  public class VersusManager : SingletonMonoBehaviour<VersusManager>
  {
    /// <summary>
    /// 対戦画面の各種ロケーション
    /// </summary>
    private Dictionary<Define.App.Player, Location> locations = new Dictionary<Define.App.Player, Location>();

    /// <summary>
    /// プレイヤー
    /// </summary>
    private Dictionary<Define.App.Player, Player> players = new Dictionary<Define.App.Player, Player>();

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

    protected override void MyAwake()
    {
      Debug.Manager.Instance.Regist(this);
    }

    /// <summary>
    /// ロケーションをセットアップする、ロケーション情報を持ったオブジェクトを受け取る。
    /// </summary>
    public void Setup(GameObject locations)
    {
      this.locations.Add(Define.App.Player.P1, new Location("P1", locations));
      this.locations.Add(Define.App.Player.P2, new Location("P2", locations));

      CreatePlayer(Define.App.Player.P1);
      CreatePlayer(Define.App.Player.P2);

      this.players[Define.App.Player.P1].Setup();
      this.players[Define.App.Player.P2].Setup();
    }

    public void UpdatePlayer()
    {
      this.players[Define.App.Player.P1].Update();
      this.players[Define.App.Player.P2].Update();
    }

    /// <summary>
    /// プレイヤーを生成
    /// </summary>
    private void CreatePlayer(Define.App.Player type)
    {
      Player.Props props = new Player.Props();
      props.parent   = CacheTransform;
      props.location = this.locations[type];
      props.config   = SaveManager.Instance.GetPlayerConfig(type);

      this.players.Add(type, new Player(props).Init());
    }

#if _DEBUG
    public override void OnDebug()
    {
      using (new GUILayout.HorizontalScope(GUI.skin.box)) {
        this.players[Define.App.Player.P1].OnDebug();
        this.players[Define.App.Player.P2].OnDebug();
      }
    }
#endif
  }

}

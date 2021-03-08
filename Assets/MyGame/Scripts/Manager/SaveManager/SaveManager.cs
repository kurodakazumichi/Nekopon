using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.SaveManagement;
using MyGame.Define;

namespace MyGame
{
  /// <summary>
  /// セーブデータ管理
  /// </summary>
  public class SaveManager : SingletonMonoBehaviour<SaveManager>
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// プレイヤー設定(規定値)
    /// </summary>
    private PlayerConfig defaultPlayerConfig = null;

    /// <summary>
    /// プレイヤー設定データ
    /// </summary>
    private Dictionary<App.Player, PlayerConfig> players = new Dictionary<App.Player, PlayerConfig>();

    //-------------------------------------------------------------------------
    // Load, Unload

    public void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceManager.Instance;
      rm.Load<PlayerConfig>("Config.Player.asset", pre, done, (res) => { this.defaultPlayerConfig = res; });
    }

    public void Unload()
    {
      var rm = ResourceManager.Instance;
      rm.Unload("Config.Player.asset");
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyStart()
    {
      Debug.Manager.Instance.Regist(this);
    }

    //-------------------------------------------------------------------------
    // publicメソッド

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
      if (!this.defaultPlayerConfig) return;
      this.players.Add(App.Player.P1, Instantiate(this.defaultPlayerConfig));
      this.players.Add(App.Player.P2, Instantiate(this.defaultPlayerConfig));
    }

    /// <summary>
    /// PlayerConfigを取得
    /// </summary>
    public IPlayerConfig GetPlayerConfig(App.Player type)
    {
      return this.players[type];
    }

#if _DEBUG

    //-------------------------------------------------------------------------
    // デバッグ

    private string[] __Type = new string[] {"Default", "Save" };
    private int __SelectedTypeIndex = 0;

    public override void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) {
        __SelectedTypeIndex = GUILayout.SelectionGrid(__SelectedTypeIndex, __Type, __Type.Length);

        string type = __Type[__SelectedTypeIndex];

        if (type == "Default") {
          this.defaultPlayerConfig.OnDebug();
        }

        if (type == "Save") {
          using (new GUILayout.HorizontalScope()) {
            using (new GUILayout.VerticalScope()) {
              GUILayout.Label("Player1");
              this.players[App.Player.P1].OnDebug();
            }
            using (new GUILayout.VerticalScope()) {
              GUILayout.Label("Player2");
              this.players[App.Player.P2].OnDebug();
            }
          }
        }
        
      }
        
    }
#endif
  }
}
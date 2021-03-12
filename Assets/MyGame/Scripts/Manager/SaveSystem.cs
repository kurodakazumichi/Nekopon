using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;

namespace MyGame
{
  /// <summary>
  /// セーブデータシステム
  /// </summary>
  public class SaveSystem : SingletonMonoBehaviour<SaveSystem>
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
    private Dictionary<App.Player, PlayerConfig> players 
      = new Dictionary<App.Player, PlayerConfig>();

    /// <summary>
    /// JoyConfigデータ
    /// </summary>
    private Dictionary<App.JoyType, JoyConfig> joyConfigs 
      = new Dictionary<App.JoyType, JoyConfig>();

    /// <summary>
    /// キーボードの操作方法データ
    /// </summary>
    private Dictionary<App.OperationMethod, KeyConfig> keyConfigs 
      = new Dictionary<App.OperationMethod, KeyConfig>();

    //-------------------------------------------------------------------------
    // Load, Unload

    public void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceManager.Instance;

      // プレイヤー設定(HP,MPなど)
      rm.Load<PlayerConfig>("Config.Player.asset", pre, done, (res) => { 
        this.defaultPlayerConfig = res; 
      });

      // JoyConfig
      rm.Load<JoyConfig>("Config.Joy.X360.asset", pre, done, (res) => { 
        this.joyConfigs.Add(App.JoyType.X360, res);
      });
      rm.Load<JoyConfig>("Config.Joy.PS4.asset", pre, done, (res) => { 
        this.joyConfigs.Add(App.JoyType.PS4, res);
      });

      // KeyConfig
      rm.Load<KeyConfig>("Config.Key.Standard.asset", pre, done, (res) => { 
        this.keyConfigs.Add(App.OperationMethod.Standard, res);
      });
      rm.Load<KeyConfig>("Config.Key.Player1.asset", pre, done, (res) => {
        this.keyConfigs.Add(App.OperationMethod.Player1, res);
      });
      rm.Load<KeyConfig>("Config.Key.Player2.asset", pre, done, (res) => {
        this.keyConfigs.Add(App.OperationMethod.Player2, res);
      });
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

    protected override void OnMyDestory()
    {
      Debug.Manager.Instance.Discard(this);
    }

    //-------------------------------------------------------------------------
    // publicメソッド

    /// <summary>
    /// 初期化
    /// </summary>
    public void Setup()
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

    /// <summary>
    /// KeyConfigを取得
    /// </summary>
    public KeyConfig GetKeyConfig(App.OperationMethod type)
    {
      return this.keyConfigs[type];
    }

    /// <summary>
    /// JoyConfigを取得
    /// </summary>
    public JoyConfig GetJoyConfig(App.JoyType type)
    {
      return this.joyConfigs[type];
    }

    public JoyConfig GetJoyConfig(string name)
    {
      var names = Input.GetJoystickNames();
      for (int i = 0; i < names.Length; ++i) { 
        Debug.Logger.Log(names[i]);
      }
      return null;
    }

#if _DEBUG

    //-------------------------------------------------------------------------
    // デバッグ

    private int __SelectedTypeIndex = 0;
    private int __SelectedKeyConfigIndex = 0;
    private int __SelectedJoyConfigIndex = 0;

    public override void OnDebug()
    {
      string[] menus = new string[] { "Default", "Save" };

      using (new GUILayout.VerticalScope(GUI.skin.box)) {
        __SelectedTypeIndex = GUILayout.SelectionGrid(__SelectedTypeIndex, menus, menus.Length);

        string type = menus[__SelectedTypeIndex];

        if (type == "Default") {
          this.defaultPlayerConfig.OnDebug();
          OnDebugKeyConfig();
          OnDebugJoyConfig();
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

    private void OnDebugKeyConfig()
    {
      var menus = MyEnum.GetNames<App.OperationMethod>();

      using (new GUILayout.HorizontalScope()) 
      {
        GUILayout.Label("KeyConfig");
        __SelectedKeyConfigIndex
        = GUILayout.SelectionGrid(__SelectedKeyConfigIndex, menus, menus.Length);
      }

        App.OperationMethod type
        = MyEnum.Parse<App.OperationMethod>(menus[__SelectedKeyConfigIndex]);

      this.keyConfigs[type].OnDebug();
    }

    private void OnDebugJoyConfig()
    {
      var menus = MyEnum.GetNames<App.JoyType>();

      using (new GUILayout.HorizontalScope()) 
      {
        GUILayout.Label("JoyConfig");
        __SelectedJoyConfigIndex
          = GUILayout.SelectionGrid(__SelectedJoyConfigIndex, menus, menus.Length);
      }
      
      App.JoyType type = MyEnum.Parse<App.JoyType>(menus[__SelectedJoyConfigIndex]);
      this.joyConfigs[type].OnDebug();
    }
#endif
  }
}
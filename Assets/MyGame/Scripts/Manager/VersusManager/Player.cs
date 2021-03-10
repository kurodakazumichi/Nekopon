using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.VersusManagement
{
  /// <summary>
  /// 対戦プレイヤーに該当するクラス
  /// </summary>
  public class Player
  {
    //-------------------------------------------------------------------------
    // クラス

    /// <summary>
    /// プレイヤー生成時に必要なパラメータ
    /// </summary>
    public class Props
    {
      /// <summary>
      /// プレイヤータイプ
      /// </summary>
      public Define.App.Player Type;

      /// <summary>
      /// Playerの親に該当するオブジェクト
      /// </summary>
      public Transform Parent;

      /// <summary>
      /// 各種画面の配置情報
      /// </summary>
      public Location Location;

      /// <summary>
      /// プレイヤー設定
      /// </summary>
      public IPlayerConfig Config;
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// プレイヤータイプ
    /// </summary>
    public Define.App.Player Type { get; private set; }

    /// <summary>
    /// 親オブジェクト
    /// </summary>
    private Transform parent = null;

    /// <summary>
    /// Playerに関するゲームオブジェクトを格納しておくゲームオブジェクト
    /// </summary>
    private Transform folder = null;

    /// <summary>
    /// ロケーション情報
    /// </summary>
    private Location location;

    /// <summary>
    /// ステータス
    /// </summary>
    private PlayerStatus status = null;

    /// <summary>
    /// パズル
    /// </summary>
    private Puzzle puzzle = null;

    /// <summary>
    /// ゲージ
    /// </summary>
    private Gauges gauges = null;

    /// <summary>
    /// プレイヤー設定
    /// </summary>
    private IPlayerConfig config = null;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// 公開用Status
    /// </summary>
    public IPlayerStatus Status => this.status;

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Gauges.Load(pre, done);
      Puzzle.Load(pre, done);
    }

    public static void Unload()
    {
      Gauges.Unload();
      Puzzle.Unload();
    }

    //-------------------------------------------------------------------------
    // 生成・準備系

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Player(Props props)
    {
      this.Type     = props.Type;
      this.parent   = props.Parent;
      this.location = props.Location;
      this.config   = props.Config;
      this.status   = new PlayerStatus().Setup(config);
    }

    /// <summary>
    /// 各種オブジェクトの生成、初期化
    /// </summary>
    public Player Init()
    {
      // プレースフォルダーを作成
      this.folder = new GameObject("Player").transform;
      this.folder.parent = this.parent;

      // パズルを作成
      this.puzzle = new Puzzle(this.folder, this.location.Paw);
      this.puzzle.Init();
      this.puzzle.OnVanished = OnVanished;

      // ゲージを生成
      var props = new Gauges.Props();
      props.Location = this.location;
      props.Parent   = this.folder;

      this.gauges = new Gauges(props).Init();

      return this;
    }

    /// <summary>
    /// プレイヤーのセットアップ
    /// </summary>
    public void Setup()
    {
      // ゲージのセットアップ
      this.gauges.Setup(
        this.status.Hp.Rate,
        this.status.Dp.Rate,
        this.status.Ap.Rate
      );

      // パズルのセットアップ
      this.puzzle.Setup();
    }

    /// <summary>
    /// プレイヤー始動
    /// </summary>
    public void Start()
    {
      this.puzzle.ShowCursor();
    }

    public void Update()
    {
      this.status.Update();
      this.gauges.Hp = this.status.Hp.Rate;
      this.gauges.Dp = this.status.Dp.Rate;

      if (Input.GetKeyDown(KeyCode.Alpha2)) {
        this.puzzle.ShowCursor();
      }
      if (Input.GetKeyDown(KeyCode.Alpha3)) {
        this.puzzle.HideCursor();
      }

      if (Input.GetKeyDown(KeyCode.D)) {
        Debug.Logger.Log(this.puzzle.HasMovingPaw);
      }

      // カーソル移動(上下左右)
      if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        this.puzzle.MoveCursorL();
      }
      if (Input.GetKeyDown(KeyCode.RightArrow)) {
        this.puzzle.MoveCursorR();
      }
      if (Input.GetKeyDown(KeyCode.UpArrow)) {
        this.puzzle.MoveCursorU();
      }
      if (Input.GetKeyDown(KeyCode.DownArrow)) {
        this.puzzle.MoveCursorD();
      }

      // 肉球選択 or 入れ替え
      if (Input.GetKeyDown(KeyCode.Z)) {
        // 選択中の肉球があれば入れ替え
        if (this.puzzle.HasSelectedPaw) {
          this.puzzle.Swap();
        }

        // 選択中の肉中がなければカーソルのある所の肉球を選択
        else {
          this.puzzle.SelectPaw();
        }
      }

      // 肉球選択の解除
      if (Input.GetKeyDown(KeyCode.X)) {
        this.puzzle.ReleasePaw();
      }

      // 連鎖
      if (Input.GetKeyDown(KeyCode.S)) {
        this.puzzle.StartChain();
      }
      if (this.puzzle != null && this.puzzle.IsInChain) {
        this.puzzle.UpdateChain();

        if (this.puzzle.IsFinishedChain) {
          this.puzzle.EndChain();
          VersusManager.Instance.AttackPlayer(this);
        }
      }
    }

    //-------------------------------------------------------------------------
    // パズル系

    /// <summary>
    /// 肉球消滅時に呼ばれるコールバック
    /// </summary>
    private void OnVanished(ChainInfo score)
    {
      // 消えた肉球の数だけMPを回復
      MyEnum.ForEach<Define.App.Attribute>((attribute) => {
        this.status.Mp(attribute).Now += score.GetVanishCount(attribute);
      });

      // 攻撃力計算：連鎖数 * 合計消滅数 / 2
      this.status.Attack.Now += (score.ChainCount * score.TotalVanishCount / 2);
    }

    //-------------------------------------------------------------------------
    // プレイヤー関連

    /// <summary>
    /// 攻撃を受け入れる
    /// </summary>
    public void AcceptAttack(Player attacker)
    {
      this.status.Damage.Now += attacker.status.Attack.Now;
      attacker.status.Attack.BeToEmpty();
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    public void OnDebug()
    {
      using (new GUILayout.VerticalScope()) {
        this.status.OnDebug();
        this.puzzle.OnDebug();
      }
    }
#endif
  }
}


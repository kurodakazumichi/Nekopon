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
    // プレイヤーステータスクラス
    public class Status
    {
      /// <summary>
      /// HP
      /// </summary>
      public LimitedFloat HP = new LimitedFloat();

      /// <summary>
      /// MP
      /// </summary>
      private Dictionary<Define.App.Attribute, LimitedFloat> mp = new Dictionary<Define.App.Attribute, LimitedFloat>();

      /// <summary>
      /// コンストラクタ
      /// </summary>
      public Status()
      {
        MyEnum.ForEach<Define.App.Attribute>((attribute) => { 
          this.mp.Add(attribute, new LimitedFloat());
        });
      }
#if _DEBUG
      public void OnDebug()
      {
        using (new GUILayout.VerticalScope()) {
          GUILayout.Label($"HP:{HP.Now}");
          Util.ForEach(this.mp, (attr, mp) => { 
            GUILayout.Label($"MP_{attr}:{mp.Now}");
          });
        }
      }
#endif
    }

    //-------------------------------------------------------------------------
    // メンバ変数

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
    private Status status = null;

    /// <summary>
    /// パズル
    /// </summary>
    private Puzzle puzzle = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Puzzle.Load(pre, done);
    }

    public static void Unload()
    {
      Puzzle.Unload();
    }

    //-------------------------------------------------------------------------
    // 生成・準備系

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Player(Transform parent, Location location)
    {
      this.parent   = parent;
      this.location = location;
      this.status   = new Status();
    }

    /// <summary>
    /// 各種オブジェクトの生成、初期化
    /// </summary>
    public void Init()
    {
      // プレースフォルダーを作成
      this.folder = new GameObject("Player").transform;
      this.folder.parent = this.parent;

      // パズルを作成
      this.puzzle = new Puzzle(this.folder, this.location.Paw);
      this.puzzle.Init();
    }

    /// <summary>
    /// プレイヤーのセットアップ
    /// </summary>
    public void Setup()
    {
      this.puzzle.Setup();
    }

    public void Update()
    {
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
        }
      }
    }

#if _DEBUG
    public void OnDebug()
    {
      this.status.OnDebug();
    }
#endif
  }
}


using UnityEngine;
using MyGame.VersusManagement;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// 対戦プレイヤーに該当するクラス
  /// </summary>
  public partial class Player
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
      /// 猫のタイプ
      /// </summary>
      public Define.App.Cat CatType;

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
    /// ロケーション情報
    /// </summary>
    public Location Location { get; private set; } = null;

    /// <summary>
    /// 親オブジェクト
    /// </summary>
    private Transform parent = null;

    /// <summary>
    /// Playerに関するゲームオブジェクトを格納しておくゲームオブジェクト
    /// </summary>
    private Transform folder = null;

    /// <summary>
    /// ステータス
    /// </summary>
    private Status status = null;

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

    /// <summary>
    /// 猫
    /// </summary>
    private Cat cat = null;

    /// <summary>
    /// 猫の種類
    /// </summary>
    private Define.App.Cat catType = default;

    /// <summary>
    /// 攻撃ユニット
    /// </summary>
    private Attack attack = null;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// 攻撃を反射可能
    /// </summary>
    public bool CanReflect = false;

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
      this.Location = props.Location;
      this.config   = props.Config;
      this.catType  = props.CatType;
      this.status   = new Status().Init(config);
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
      this.puzzle = new Puzzle(this.folder, this.Location.Paw);
      this.puzzle.Init();
      this.puzzle.OnVanished = OnVanished;

      // ゲージを生成
      var props = new Gauges.Props();
      props.Location = this.Location;
      props.Parent   = this.folder;

      this.gauges = new Gauges(props).Init();

      // 猫を生成
      this.cat = new GameObject("Cat").AddComponent<Cat>();
      this.cat.SetParent(this.folder);
      this.cat.CacheTransform.position = this.Location.Cat;
      this.cat.Init(this.catType, this.Type == Define.App.Player.P2);

      return this;
    }

    /// <summary>
    /// プレイヤーのセットアップ
    /// </summary>
    public void Setup()
    {
      // ゲージのセットアップ
      this.gauges.Setup(
        this.status.HpRate,
        this.status.DpRate,
        this.status.ApRate
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
      this.gauges.Hp = this.status.HpRate;
      this.gauges.Dp = this.status.DpRate;


      if (Input.GetKeyDown(KeyCode.Alpha1)) {
        this.cat.ToUsual();
      }
      if (Input.GetKeyDown(KeyCode.Alpha2)) {
        this.cat.ToDamage();
      }
      if (Input.GetKeyDown(KeyCode.Alpha3)) {
        this.cat.ToAttack();
      }

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
          Attack();
        }
      }

      // 凍結
      if (Input.GetKeyDown(KeyCode.F)) {
        this.puzzle.Freeze();
      }
      // 麻痺
      if (Input.GetKeyDown(KeyCode.P)) {
        this.puzzle.Paralyze();
      }
      // 回復
      if (Input.GetKeyDown(KeyCode.O)) {
        this.puzzle.Cure();
      }
      // ランダム化
      if (Input.GetKeyDown(KeyCode.R)) {
        this.puzzle.Randomize();
      }

      if (Input.GetKeyDown(KeyCode.Alpha1)) {
        TakeDamage(1000);
      }

      if (Input.GetKeyDown(KeyCode.Alpha2)) {
        Recover(Random.Range(300, 600));
      }
    }

    //-------------------------------------------------------------------------
    // パズル系

    /// <summary>
    /// 肉球消滅時に呼ばれるコールバック
    /// </summary>
    private void OnVanished(Puzzle.ChainInfo score)
    {
      // 攻撃ユニットがなければ生成
      if (this.attack == null) {
        this.attack = SkillManager.Instance.Create(this.folder);
        this.attack.ToUsual(this.Location.AttackBase);
      }

      // 消えた肉球の数だけMPを回復
      MyEnum.ForEach<Define.App.Attribute>((attribute) => {
        this.status.AddMp(attribute, score.GetVanishCount(attribute));
      });

      // 攻撃力計算：連鎖数 * 合計消滅数 / 2
      this.status.AddPower(score.ChainCount * score.TotalVanishCount / 2);

      // 攻撃の大きさを設定する
      this.attack.SetIntensity(this.status.PowerRate);
    }

    //-------------------------------------------------------------------------
    // プレイヤー関連

    /// <summary>
    /// プレイヤーの通常攻撃
    /// </summary>
    private void Attack()
    {
      if (this.attack == null) return;

      var target = VersusManager.Instance.GetTargetPlayerBy(Type);
      IAction action = new Attack.Action(this.attack, this, target);
      this.attack.ToAttack(this.Location.TargetBase, action);
      this.attack = null;
    }

    /// <summary>
    /// プレイヤーが攻撃を受ける
    /// </summary>
    public void TakeAttack(Player attacker)
    {
      this.status.TakeAttack(attacker.status);
    }

    /// <summary>
    /// プレイヤーがダメージを受ける
    /// </summary>
    public void TakeDamage(float points)
    {
      this.status.TakeDamage(points);
    }

    /// <summary>
    /// 回復する
    /// </summary>
    public void Recover(float points)
    {
      this.status.Recover(points);
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


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
    private readonly Transform parent = null;

    /// <summary>
    /// Playerに関するゲームオブジェクトを格納しておくゲームオブジェクト
    /// </summary>
    private Transform folder = null;

    /// <summary>
    /// ステータス
    /// </summary>
    private readonly Status status = null;

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
    private readonly IPlayerConfig config = null;

    /// <summary>
    /// 猫
    /// </summary>
    private Cat cat = null;

    /// <summary>
    /// 猫の種類
    /// </summary>
    public Define.App.Cat catType { get; private set; } = default;

    /// <summary>
    /// 攻撃ユニット
    /// </summary>
    private Attack attack = null;

    /// <summary>
    /// 固有スキル
    /// </summary>
    private UniqueSkill unique = null;

    /// <summary>
    /// 脳
    /// </summary>
    private IBrain brain = null;

    /// <summary>
    /// 攻撃を反射可能
    /// </summary>
    public bool CanReflect { get; set; } = false;

    /// <summary>
    /// 無敵フラグ
    /// </summary>
    public bool IsInvincible { get; set; } = false;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// 猫の種類で固有スキルを決定
    /// </summary>
    private Define.App.UniqueSkill UniqueSkillType {
      get {
        switch (this.catType) {
          case Define.App.Cat.Minchi: return Define.App.UniqueSkill.Invincible;
          case Define.App.Cat.Nick: return Define.App.UniqueSkill.Reflection;
          case Define.App.Cat.Tii: return Define.App.UniqueSkill.Recovery;
          case Define.App.Cat.Shiro: return Define.App.UniqueSkill.Swap;
          default: return default;
        }
      }
    }

    /// <summary>
    /// パズルが入れ替え中かどうか
    /// </summary>
    public bool IsSwapping => this.puzzle.IsSwapping;

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Gauges.Load(pre, done);
      Puzzle.Load(pre, done);
      UniqueSkill.Load(pre, done);
    }

    public static void Unload()
    {
      Gauges.Unload();
      Puzzle.Unload();
      UniqueSkill.Unload();
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
      props.Parent = this.folder;

      this.gauges = new Gauges(props).Init();

      // 猫を生成
      this.cat = MyGameObject.Create<Cat>("Cat", this.folder);
      this.cat.CacheTransform.position = this.Location.Cat;
      this.cat.Init(this.catType, this.Type == Define.App.Player.P2);

      // ユニークスキルを生成
      this.unique = MyGameObject.Create<UniqueSkill>("Unique", this.folder);

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

      // 固有スキルのセットアップ
      this.unique.Setup(UniqueSkillType);
    }

    /// <summary>
    /// 脳をセットする
    /// </summary>
    public void SetBrain(IBrain brain)
    {
      this.brain = brain;
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
      // ゲージの更新
      UpdateGauge();

      // 思考の更新
      UpdateThink();

      // カーソル移動(上下左右)
      if (Type == Define.App.Player.P1) {

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
      }

      if (this.puzzle != null && this.puzzle.IsInChain) {
        this.puzzle.UpdateChain();

        if (this.puzzle.IsFinishedChain) {
          this.puzzle.EndChain();
          Attack();
        }
      }


      // 強制ダメージ(火)
      if (Input.GetKeyDown(KeyCode.Alpha1)) {
        FireAttributeSkill(Define.App.Attribute.Fir);
      }
      // 状態異常回復(水)
      if (Input.GetKeyDown(KeyCode.Alpha2)) {
        FireAttributeSkill(Define.App.Attribute.Wat);
      }
      // 麻痺(雷)
      if (Input.GetKeyDown(KeyCode.Alpha3)) {
        FireAttributeSkill(Define.App.Attribute.Thu);
      }
      // 凍結(氷)
      if (Input.GetKeyDown(KeyCode.Alpha4)) {
        FireAttributeSkill(Define.App.Attribute.Ice);
      }
      // ランダム化
      if (Input.GetKeyDown(KeyCode.Alpha5)) {
        FireAttributeSkill(Define.App.Attribute.Tre);
      }
      // 体力回復(聖)
      if (Input.GetKeyDown(KeyCode.Alpha6)) {
        FireAttributeSkill(Define.App.Attribute.Hol);
      }
      // 不可視(闇)
      if (Input.GetKeyDown(KeyCode.Alpha7)) {
        FireAttributeSkill(Define.App.Attribute.Dar);
      }

      if (Input.GetKeyDown(KeyCode.Alpha8) && Type == Define.App.Player.P1) {
        FireUniqueSkill();
      }

      if (Input.GetKeyDown(KeyCode.Alpha9) && Type == Define.App.Player.P2) {
        FireUniqueSkill();
      }
    }

    /// <summary>
    /// ゲージの更新
    /// </summary>
    private void UpdateGauge()
    {
      this.status.Update();
      this.gauges.Hp = this.status.HpRate;
      this.gauges.Dp = this.status.DpRate;
    }

    private void UpdateThink()
    {
      // 脳が設定されていなければ何もしない
      if (this.brain == null) {
        return;
      }

      // 思考する
      var action = this.brain.Think();

      // 行動が取得できていれば実行する
      if (action != null) {
        action.Execute();
      }
    }

    //-------------------------------------------------------------------------
    // 操作系

    /// <summary>
    /// カーソル移動
    /// </summary>
    public void MoveCursor(Define.App.Direction dir)
    {
      if (this.puzzle == null) {
        return;
      }

      switch(dir) {
        case Define.App.Direction.L: this.puzzle.MoveCursorL(); break;
        case Define.App.Direction.R: this.puzzle.MoveCursorR(); break;
        case Define.App.Direction.U: this.puzzle.MoveCursorU(); break;
        case Define.App.Direction.D: this.puzzle.MoveCursorD(); break;
        default: break;
      }
    }

    //-------------------------------------------------------------------------
    // スキル系

    public bool TryFireAttributeSkill(Define.App.Attribute attribute)
    {
      // 現在のMPと使用MPを取得
      var NowMp = this.status.GetMp(attribute);
      var useMp = this.config.GetUseMp(attribute);

      // スキルに必要なMPが不足していたらスキル発動できない
      if (NowMp < useMp) {
        return false;
      }

      // MP消費
      this.status.AddMp(attribute, -useMp);

      // スキル発動
      FireAttributeSkill(attribute);
      return true;
    }

    public void FireAttributeSkill(Define.App.Attribute attribute)
    {

      // 属性スキルを取得
      var skill = SkillManager.Instance.Create(attribute);

      // スキル発動者と対称を取得
      var owner = this;
      var target = VersusManager.Instance.GetTargetPlayerBy(Type);

      // 自分に対するスキルならtargetを自分に設定
      switch (attribute) {
        // 水と聖は対象が自分
        case Define.App.Attribute.Wat:
        case Define.App.Attribute.Hol:
          target = owner;
          break;
      }

      // スキル発動
      skill.Fire(owner, target);
    }

    /// <summary>
    /// 固有スキルを使用する
    /// </summary>
    private void FireUniqueSkill()
    {
      // 固有スキルが使用できない状況であれば処理を抜ける
      if (SkillManager.Instance.IsLockUniqueSkill) return;

      // 現在スキルが作動中であれば処理を抜ける
      if (!this.unique.IsIdle) return;

      // 固有スキル発動
      var owner = this;
      var target = VersusManager.Instance.GetTargetPlayerBy(Type);
      this.unique.Fire(owner, target);
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

    /// <summary>
    /// 回復する
    /// </summary>
    public void Cure()
    {
      this.puzzle.Cure();
    }

    /// <summary>
    /// 麻痺する
    /// </summary>
    public void Paralyze()
    {
      // ガード成功ならダメージを受けない
      if (TryGuard()) {
        return;
      }

      this.puzzle.Paralyze();
    }

    /// <summary>
    /// 凍結する
    /// </summary>
    public void Freeze()
    {
      // ガード成功ならダメージを受けない
      if (TryGuard()) {
        return;
      }

      this.puzzle.Freeze();
    }

    /// <summary>
    /// パズルをランダムに変更する
    /// </summary>
    public void Randomize()
    {
      // ガード成功ならダメージを受けない
      if (TryGuard()) {
        return;
      }

      this.puzzle.Randomize();
    }

    /// <summary>
    /// パズルを不可視にする
    /// </summary>
    public void Invisible()
    {
      // ガード成功ならダメージを受けない
      if (TryGuard()) {
        return;
      }

      this.puzzle.Invisible();
    }

    /// <summary>
    /// パズルの入れ替え
    /// </summary>
    public void SwapPuzzle(Player target)
    {
      var puzzle = this.puzzle;
      this.SwapPuzzleProc(target.puzzle);
      target.SwapPuzzleProc(puzzle);
    }

    /// <summary>
    /// パズルを入れ替える際に行う処理
    /// </summary>
    private void SwapPuzzleProc(Puzzle puzzle)
    {
      // 指定されたパズルの所有者を変更する
      this.puzzle = puzzle;
      this.puzzle.OnVanished = OnVanished;
      this.puzzle.ChangeOwner(this.folder, this.Location.Paw);

      // 攻撃があれば入れ替え時に攻撃発動
      if (this.attack != null) {
        Attack();
      }
    }

    //-------------------------------------------------------------------------
    // プレイヤー関連

    /// <summary>
    /// プレイヤーの通常攻撃
    /// </summary>
    private void Attack()
    {
      // 攻撃ユニットがなければ何もしない(できない)
      if (this.attack == null) return;

      // 対戦相手を取得する
      var target = VersusManager.Instance.GetTargetPlayerBy(Type);

      // 攻撃アクションを作成(攻撃ユニット、自分と相手をセット)
      IAction action = new Attack.Action(this.attack, this, target);

      // 攻撃を放つ(攻撃を放った後はActionに委ねる)
      this.attack.ToAttack(target.Location.Top, action);
      this.attack = null;
    }

    /// <summary>
    /// プレイヤーがダメージを受ける
    /// </summary>
    public void TakeDamage(float points)
    {
      // ガード成功ならダメージを受けない
      if (TryGuard()) {
        return;
      }

      this.status.TakeDamage(points);
    }

    /// <summary>
    /// 最大HPのrate%回復する
    /// </summary>
    public void Recover(float rate)
    {
      this.status.Recover(rate);
    }

    /// <summary>
    /// ステータスから攻撃力を抽出する
    /// </summary>
    public float ExtactPower()
    {
      return this.status.ExtractPower();
    }

    //-------------------------------------------------------------------------
    // 状態処理

    /// <summary>
    /// 無敵時ならガード出来るので、ガードを試す処理
    /// </summary>
    private bool TryGuard()
    {
      // 無敵でないなら何もしない
      if (!IsInvincible) return false;

      // TODO:ガード音再生

      return true;
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    public void OnDebug()
    {
      using (new GUILayout.VerticalScope()) {
        this.status.OnDebug();
        this.puzzle.OnDebug();

        using (new GUILayout.VerticalScope(GUI.skin.box)) {
          GUILayout.Label($"Invinsible:{this.IsInvincible}");
          GUILayout.Label($"Reflection:{this.CanReflect}");
        }

      }
    }
#endif
  }
}


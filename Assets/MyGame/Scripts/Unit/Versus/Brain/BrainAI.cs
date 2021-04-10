using UnityEngine;
using MyGame.Define;
using MyGame.Unit.Versus.BrainAction;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// AI用の脳
  /// </summary>
  public class BrainAI : IBrain
  {
    /// <summary>
    /// BrainAIの生成に必要な物が詰まっている
    /// </summary>
    public class Props
    {
      public Player owner = null;
      public Player target = null;
    }

    private enum State
    {
      Build, // 連鎖を構築する
    }

    /// <summary>
    /// コンストラクタで渡されるAIに必要な小道具
    /// </summary>
    protected readonly Props props = null;

    /// <summary>
    /// 待機用のタイマー
    /// 毎フレーム思考すると早すぎるので、思考に一定間隔持たせるためのタイマー
    /// </summary>
    protected float waitTimer = 0;

    /// <summary>
    /// 目的の属性
    /// </summary>
    private App.Attribute targetAttribute = default;

    /// <summary>
    /// 目的の座標
    /// </summary>
    private Vector2Int targetCoord = Vector2Int.zero;

    /// <summary>
    /// 待機
    /// </summary>
    private bool IsWait => 0 < this.waitTimer;

    /// <summary>
    /// 思考した結果のアクション
    /// </summary>
    private IAction decidedAction = null;

    /// <summary>
    /// ステートマシン
    /// </summary>
    private StateMachine<State> state = new StateMachine<State>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public BrainAI(Props props)
    {
      this.props = props;

      this.state.Add(State.Build, OnBuildEnter, OnBuildUpdate);
      this.state.SetState(State.Build);
    }

    /// <summary>
    /// 思考
    /// </summary>
    public IAction Think()
    {
      this.waitTimer -= TimeSystem.Instance.DeltaTime;
      if (IsWait) return null;
      
      this.waitTimer = 0.05f;

      this.state.Update();


      return this.decidedAction;
    }

    /// <summary>
    /// 目的の属性を取得
    /// </summary>
    private App.Attribute GetTargetAttribute() {

        // TODO: 欲しい属性を決める要素
        // 1. MPのチャージ状況
        // 2. HPの状況
        // 3. キャラの好み

        return MyEnum.Random<App.Attribute>();
    }



    //-------------------------------------------------------------------------
    // Build:連鎖の構築を行うState
    // 
    private void OnBuildEnter()
    {
      // 目標の座標を設定する
      this.targetCoord.x = Random.Range(0, Define.Versus.PAW_COL);
      this.targetCoord.y = Random.Range(0, Define.Versus.PAW_ROW);
    }

    private void OnBuildUpdate()
    {
      // 現在のカーソルの座標を取得
      var currentCoord = this.props.owner.CursorCoord;

      // TODO:目的の座標にある肉球の状態が変わっていないかチェック
      // 属性スキル(木)や、入替スキルによって肉球が変わる可能性があるし
      // 属性スキル(雷)で動かせなくなる可能性もあるのでそのあたりを確認する
      // その場合は、次の行動を考えるステートへ飛ばす

      // 目標地点にたどり着いていたら肉球を選択するアクションを登録
      if (this.targetCoord == currentCoord) {
        this.decidedAction = new SelectPawAction(this.props.owner);
        this.state.SetState(State.Build);
        return;
      }

      // カーソル移動アクションを生成
      this.decidedAction = CreateMoveCursorAction(currentCoord, this.targetCoord);
    }

    /// <summary>
    /// 現在の座標から目的の座標に向かってカーソルを動かすためのアクションを生成する
    /// </summary>
    private IAction CreateMoveCursorAction(Vector2Int current, Vector2Int target) 
    {
      var sub = target - current;

      if (sub.x < 0) {
        return new MoveCursorAction(this.props.owner, Vector3.left);
      }
      if (0 < sub.x) {
        return new MoveCursorAction(this.props.owner, Vector3.right);
      }
      if (sub.y < 0) {
        return new MoveCursorAction(this.props.owner, Vector3.down);
      }
      if (0 < sub.y) {
        return new MoveCursorAction(this.props.owner, Vector3.up);
      }

      return null;
    }
  }
}


using System;

namespace MyGame.Unit.Versus.UniqueSkillExecutors
{
  /// <summary>
  /// 固有スキル実行者のベースクラス
  /// </summary>
  public class ExecutorBase<T> : UniqueSkill.IExecutor where T : Enum
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// ステートマシン
    /// </summary>
    protected StateMachine<T> state = new StateMachine<T>();

    /// <summary>
    /// 汎用タイマー
    /// </summary>
    protected float timer = 0;

    /// <summary>
    /// スキルを発動したプレイヤー
    /// </summary>
    protected Player owner = null;

    /// <summary>
    /// 対戦プレイヤー
    /// </summary>
    protected Player target = null;

    /// <summary>
    /// スキルロックを解除するためのコールバック
    /// </summary>
    protected Action onUnlock = null;

    /// <summary>
    /// スキル終了時に呼ばれるコールバック
    /// </summary>
    protected Action onDone = null;

    //-------------------------------------------------------------------------
    // メソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExecutorBase(Action onUnlock, Action onDone)
    {
      this.onUnlock = onUnlock;
      this.onDone   = onDone;
    }

    /// <summary>
    /// スキル発動
    /// </summary>
    public virtual void Fire(Player owner, Player target)
    {
      this.owner = owner;
      this.target = target;
    }

    /// <summary>
    /// 更新
    /// </summary>
    public virtual void Update() 
    {
      this.state.Update();
    }

    /// <summary>
    /// タイマーを更新
    /// </summary>
    protected virtual void UpdateTimer()
    {
      this.timer += TimeSystem.Instance.DeltaTime;
    }
  }

}

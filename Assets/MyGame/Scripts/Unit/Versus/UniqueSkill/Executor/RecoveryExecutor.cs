using System;

namespace MyGame.Unit.Versus.UniqueSkillExecutors
{
  /// <summary>
  /// 一定時間回復のスキル
  /// 一定間隔でMP消費なしで属性スキル(聖)を複数回発動する
  /// </summary>
  public class RecoveryExecutor : ExecutorBase<RecoveryExecutor.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Cure,
      Wait,
    }

    /// <summary>
    /// スキル使用回数
    /// </summary>
    private int count = 0;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public RecoveryExecutor(Action onUnlock, Action onDone)
      : base(onUnlock, onDone)
    {
      this.state.Add(State.Idle);
      this.state.Add(State.Cure, OnCureEnter, OnCureUpdate);
      this.state.Add(State.Wait, OnWaitEnter, OnWaitUpdate);
      this.state.SetState(State.Idle);
    }

    /// <summary>
    /// スキル発動
    /// </summary>
    public override void Fire(Player owner, Player target)
    {
      base.Fire(owner, target);

      // 回復スキルは発動した段階で他のスキルを作動させても大丈夫なのでここで解除
      onUnlock?.Invoke();

      // スキル使用数をリセット
      this.count = 0;

      // 回復スキルを発動
      this.state.SetState(State.Cure);
    }

    /// <summary>
    /// 回復スキル発動
    /// </summary>
    private void OnCureEnter()
    {
      // 回復スキル発動
      this.owner.FireAttributeSkill(Define.App.Attribute.Hol);
      this.count++;
    }

    /// <summary>
    /// スキル終了か、次回スキル発動のための待機へ向かうか分岐
    /// </summary>
    private void OnCureUpdate()
    {
      const int LIMIT = 5; // 合計5回

      if (this.count < LIMIT) {
        this.state.SetState(State.Wait);
      } else {
        this.onDone?.Invoke();
        this.state.SetState(State.Idle);
      }
    }

    /// <summary>
    /// 待機開始
    /// </summary>
    private void OnWaitEnter() 
    { 
      this.timer = 0;
    }

    /// <summary>
    /// 待機中
    /// </summary>
    private void OnWaitUpdate() 
    {
      const float TIME = 5f; // 回復スキルは5秒間隔

      UpdateTimer();

      if (TIME <= this.timer) {
        this.state.SetState(State.Cure);
      }
    }
  }
}

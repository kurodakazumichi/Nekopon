using System;

namespace MyGame.Unit.Versus.UniqueSkillExecutors
{
  /// <summary>
  /// 反射スキルの実行者
  /// </summary>
  public class ReflectionExecutor : ExecutorBase<ReflectionExecutor.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Active,
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ReflectionExecutor(Action onUnlock, Action onDone)
      : base(onUnlock, onDone)
    {
      this.state.Add(State.Idle);
      this.state.Add(State.Active, OnActiveEnter, OnActiveUpdate, OnActiveExit);
      this.state.SetState(State.Idle);
    }

    /// <summary>
    /// スキル発動
    /// </summary>
    public override void Fire(Player owner, Player target)
    {
      base.Fire(owner, target);

      // 反射スキルは発動した段階で他のスキルを作動させても大丈夫なのでここで解除
      onUnlock?.Invoke();

      // アクティブへ
      this.state.SetState(State.Active);
    }

    /// <summary>
    /// スキル発動
    /// </summary>
    private void OnActiveEnter()
    {
      // スキルを発動した者を反射可能にする
      this.owner.CanReflect = true;
      this.timer = 0;
    }

    /// <summary>
    /// スキル発動中
    /// </summary>
    private void OnActiveUpdate()
    {
      // 反射は0.5秒
      const float TIME = 0.5f;

      UpdateTimer();

      if (TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    /// <summary>
    /// スキル終了
    /// </summary>
    private void OnActiveExit()
    {
      // スキル終了のコールバックを実行し、反射フラグを落とす
      this.onDone?.Invoke();
      this.owner.CanReflect = false;
    }
  }
}

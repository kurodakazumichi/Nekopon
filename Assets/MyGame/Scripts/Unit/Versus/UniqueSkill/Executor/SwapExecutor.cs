using System;

namespace MyGame.Unit.Versus.UniqueSkillExecutors
{
  /// <summary>
  /// 入れ替えスキルの実行者
  /// </summary>
  public class SwapExecutor : ExecutorBase<SwapExecutor.State>
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
    public SwapExecutor(Action onUnlock, Action onDone)
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

      // アクティブへ
      this.state.SetState(State.Active);
    }

    /// <summary>
    /// スキル発動
    /// </summary>
    private void OnActiveEnter() 
    {
      // スワップの実行
      this.owner.SwapPuzzle(this.target);
    }

    /// <summary>
    /// スキル発動中
    /// </summary>
    private void OnActiveUpdate()
    {
      if (this.owner.IsSwapping) return;
      if (this.target.IsSwapping) return;

      this.state.SetState(State.Idle);
    }

    /// <summary>
    /// スキル終了
    /// </summary>
    private void OnActiveExit()
    {
      // スキルのロック解除、スキル終了処理をコール
      this.onUnlock?.Invoke();
      this.onDone?.Invoke();
    }
  }
}

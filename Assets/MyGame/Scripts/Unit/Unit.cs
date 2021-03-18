using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit
{
  /// <summary>
  /// 画面上のオブジェクトの基底となるUnitBase
  /// </summary>
  /// <typeparam name="TState">状態の列挙型</typeparam>
  public class Unit<TState> : MyMonoBehaviour where TState:System.Enum
  {
    /// <summary>
    /// ステートマシン
    /// </summary>
    protected StateMachine<TState> state = new StateMachine<TState>();

    /// <summary>
    /// 汎用タイマー
    /// </summary>
    protected float timer = 0;

    protected override void Start()
    {
      MyStart();
    }

    protected override void Update()
    {
      MyUpdate();
    }

    protected override void MyUpdate()
    {
      this.state.Update();
    }

    /// <summary>
    /// タイマーの更新
    /// </summary>
    protected virtual void UpdateTimer()
    {
      this.timer += TimeSystem.Instance.DeltaTime;
    }
  }
}
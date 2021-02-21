using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Title
{

  public class TitleLogo : MyMonoBehaviour
  {
    //-------------------------------------------------------------------------
    // Inspector設定項目
    //-------------------------------------------------------------------------
    // 開始位置の高さ
    public float _StartY = 0;
    // 終了位置の高さ
    public float _EndY = 0;
    // バウンドスピード
    public float _Speed = 0;
    // バウンド回数制限
    public int _BoundLimit = 0;

    //-------------------------------------------------------------------------
    // メンバ
    //-------------------------------------------------------------------------
    private enum State
    {
      Idle,
      Bound,
      Fixed,
    }

    private StateMachine<State> state;

    // 速度
    private Vector3 velocity = Vector3.zero;

    // バウンド階数
    private int boundCount = 0;

    // バウンドが完了した時に呼ばれるコールバック
    private System.Action completedBound = null;
    public System.Action CompletedBound {
      set { this.completedBound = value; }
    }

    public void SetBound()
    {
      this.state.SetState(State.Bound);
    }

    public void SetFixed()
    {
      this.state.SetState(State.Fixed);
    }

    protected override void MyAwake()
    {
      this.state = new StateMachine<State>();
      this.state.Add(State.Idle);
      this.state.Add(State.Bound, BoundEnter, BoundUpdate, BoundExit);
      this.state.Add(State.Fixed, FixedEnter);
      this.state.SetState(State.Idle);
    }

    protected override void MyStart()
    {
    }

    protected override void MyUpdate()
    {
      this.state.Update();
    }

    private void BoundEnter()
    {
      this.cacheTransform.position = new Vector3(0, _StartY, 0);
      this.velocity = Vector3.zero;
      this.boundCount = 0;
    }

    private void BoundUpdate()
    {
      this.velocity.y -= _Speed * TimeManager.Instance.DeltaTime;
      transform.position += this.velocity;

      if (transform.position.y < _EndY) 
      {
        transform.position = new Vector3(0, _EndY, 0);
        this.velocity.y *= -0.9f;
        this.boundCount++;
      }

      if (_BoundLimit <= this.boundCount) {
        this.state.SetState(State.Fixed);
      }
    }

    private void BoundExit()
    {
      this.completedBound?.Invoke();
    }

    private void FixedEnter()
    {
      this.cacheTransform.position = new Vector3(0, _EndY, 0);
    }
  }

}
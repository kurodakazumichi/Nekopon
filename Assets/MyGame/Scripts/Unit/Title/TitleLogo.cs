using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Title
{
  /// <summary>
  /// NekoPonというタイトルロゴにアタッチするスクリプト
  /// </summary>
  public class TitleLogo : Unit
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
    // メンバ変数
    //-------------------------------------------------------------------------
    // 状態
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


    //-------------------------------------------------------------------------
    // プロパティ
    //-------------------------------------------------------------------------
    public System.Action CompletedBound {
      private get; set;
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

    protected override IEnumerator Load()
    {
      var waitForCount = new WaitForCount();
      SoundManager.Instance.Load("SE_Bound001", waitForCount.inc, waitForCount.dec);
      
      yield return waitForCount;

      this.IsLoaded = true;
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
      cacheTransform.position += this.velocity * TimeManager.Instance.DeltaTime;

      if (cacheTransform.position.y < _EndY) 
      {
        SoundManager.Instance.PlaySE("SE_Bound001");
        cacheTransform.position = new Vector3(0, _EndY, 0);
        this.velocity.y *= -0.9f;
        this.boundCount++;
      }

      if (_BoundLimit <= this.boundCount) {
        this.state.SetState(State.Fixed);
      }
    }

    private void BoundExit()
    {
      this.CompletedBound?.Invoke();
    }

    private void FixedEnter()
    {
      this.cacheTransform.position = new Vector3(0, _EndY, 0);
    }

  }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame.Unit.Title
{
  /// <summary>
  /// NekoPonというタイトルロゴにアタッチするスクリプト
  /// </summary>
  public class TitleLogo : Unit<TitleLogo.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Bound,
      Usual,
    }

    //-------------------------------------------------------------------------
    // Inspector設定項目

    /// <summary>
    /// 開始位置の高さ 
    /// </summary>
    public float _StartY = 0;

    /// <summary>
    /// 終了位置の高さ 
    /// </summary>
    public float _EndY = 0;

    /// <summary>
    /// バウンドスピード 
    /// </summary>
    public float _Speed = 0;

    /// <summary>
    /// バウンド回数制限
    /// </summary>
    public int _BoundLimit = 0;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 速度
    /// </summary>
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// バウンド階数 
    /// </summary>
    private int boundCount = 0;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// バウンド完了時に呼ばれるコールバック
    /// </summary>
    public Action CompletedBound {
      private get; set;
    }

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(Action pre, Action done)
    {
      ResourceManager.Instance.Load<AudioClip>("SE.Bound001", pre, done);
    }

    public static void Unload()
    {
      ResourceManager.Instance.Unload("SE.Bound001");
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      this.state = new StateMachine<State>();
      this.state.Add(State.Idle);
      this.state.Add(State.Bound, BoundEnter, BoundUpdate, BoundExit);
      this.state.Add(State.Usual, UsualEnter);
      this.state.SetState(State.Idle);
    }

    protected override void MyUpdate()
    {
      this.state.Update();
    }

    //-------------------------------------------------------------------------
    // 公開メソッド

    /// <summary>
    /// バウンド状態に
    /// </summary>
    public void ToBound()
    {
      this.state.SetState(State.Bound);
    }

    /// <summary>
    /// 通常状態に
    /// </summary>
    public void ToUsual()
    {
      this.state.SetState(State.Usual);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

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
        SoundManager.Instance.PlaySE("SE.Bound001");
        cacheTransform.position = new Vector3(0, _EndY, 0);
        this.velocity.y *= -0.9f;
        this.boundCount++;
      }

      if (_BoundLimit <= this.boundCount) {
        this.state.SetState(State.Usual);
      }
    }

    private void BoundExit()
    {
      this.CompletedBound?.Invoke();
    }

    private void UsualEnter()
    {
      this.cacheTransform.position = new Vector3(0, _EndY, 0);
    }

  }

}
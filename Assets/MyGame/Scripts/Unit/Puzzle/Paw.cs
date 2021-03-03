﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define.Puzzle;

namespace MyGame.Unit.Puzzle
{
  /// <summary>
  /// パズル内の肉球
  /// </summary>
  public class Paw : Unit<Paw.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State { 
      Idle,
      Vanish, // 消滅
      Move,   // 移動
      Moved,  // 移動した
    }

    //-------------------------------------------------------------------------
    // Inspector設定項目

    /// <summary>
    /// 消滅にかかる時間
    /// </summary>
    [SerializeField]
    private float _VanishingTime = 0.5f;

    [SerializeField]
    private float _VanishingAngularSpeed = 1080f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    /// <summary>
    /// 属性
    /// </summary>
    private Attribute attribute = default;

    /// <summary>
    /// 移動時の動きの種類
    /// </summary>
    private Tween.Type tweenType = default;

    /// <summary>
    /// 移動前の座標
    /// </summary>
    private Vector3 moveStartPosition = Vector3.zero;

    /// <summary>
    /// 移動後の座標
    /// </summary>
    private Vector3 moveEndPosition = Vector3.zero;

    /// <summary>
    /// 移動に要する時間
    /// </summary>
    private float movingTime = 1f;

    /// <summary>
    /// 汎用タイマー
    /// </summary>
    private float timer = 0;

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      this.spriteRenderer = CacheTransform.Find("Sprite").GetComponent<SpriteRenderer>();

      this.state.Add(State.Idle);
      this.state.Add(State.Vanish, VanishEnter, VanishUpdate, VanishExit);
      this.state.Add(State.Move, MoveEnter, MoveUpdate, MoveExit);
      this.state.Add(State.Moved);
      this.state.SetState(State.Idle);
    }

    protected override void MyUpdate()
    {
      if (Input.GetKeyDown(KeyCode.A)) {
        ToVanish();
      }
      if (Input.GetKeyDown(KeyCode.B)) {
        ToMove(new Vector3(Random.Range(-1f, 1f), Random.Range(-0.7f, 0.7f), 0), 1f);
      }
      base.MyUpdate();
    }

    //-------------------------------------------------------------------------
    // Publicメソッド

    public void ToVanish()
    {
      this.state.SetState(State.Vanish);
    }

    public void ToMove(Vector3 target, float time, Tween.Type type = Tween.Type.EaseInSine)
    {
      this.tweenType = type;
      this.movingTime = time;
      this.moveEndPosition = target;
      this.state.SetState(State.Move);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    //-------------------------------------------------------------------------
    // 消滅

    private void VanishEnter()
    {
      SetDefaultTransform();
      this.timer = 0;
    }

    private void VanishUpdate()
    {
      float deltaTime = TimeManager.Instance.DeltaTime;
      
      this.CacheTransform.Rotate(0, 0, _VanishingAngularSpeed * deltaTime);
      this.CacheTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, this.timer/_VanishingTime);

      if (_VanishingTime <= this.timer) { this.state.SetState(State.Idle); }

      this.timer += deltaTime;
    }

    private void VanishExit()
    {
      CacheTransform.localScale = Vector3.zero;
    }

    //-------------------------------------------------------------------------
    // 移動

    private void MoveEnter()
    {
      this.SetDefaultTransform();
      this.moveStartPosition = CacheTransform.position;
      this.timer = 0;
    }

    private void MoveUpdate()
    {
      // 現在の進行度
      float t = this.timer/movingTime;
      t = Tween.easing(this.tweenType, t);

      // 座標更新
      CacheTransform.position = Vector3.Lerp(this.moveStartPosition, this.moveEndPosition, t);

      // 時間経過していたら移動完了フェーズへ
      if (movingTime <= this.timer) { this.state.SetState(State.Moved); }
      this.timer += TimeManager.Instance.DeltaTime;
    }

    private void MoveExit()
    {
      CacheTransform.position = this.moveEndPosition;
    }

    /// <summary>
    /// 回転、スケールをデフォルトにする
    /// </summary>
    private void SetDefaultTransform()
    {
      CacheTransform.localScale = Vector3.one;
      CacheTransform.rotation   = Quaternion.identity;
    }
  }

}
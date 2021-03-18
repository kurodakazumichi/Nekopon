using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Props
{
  public class GlowMover : Unit<GlowMover.State>, IPoolable
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Move,  // 移動
      Scale, // スケーリング
      Flash, // ぴかぴか
      Usual, // 通常
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// Main Sprite Renderer
    /// </summary>
    private SpriteRenderer main = null;

    /// <summary>
    /// 加算合成 Sprite Renderer
    /// </summary>
    private SpriteRenderer additive = null;

    /// <summary>
    /// 光沢用の色
    /// </summary>
    private Color color = Color.white;

    /// <summary>
    /// 開始ベクトル
    /// </summary>
    private Vector3 start = Vector3.zero;

    /// <summary>
    /// 目標ベクトル
    /// </summary>
    private Vector3 end = Vector3.zero;

    /// <summary>
    /// 降り注ぐ時間
    /// </summary>
    private float time = 0;

    /// <summary>
    /// 加算合成変化のサイクル
    /// </summary>
    private float cycle = 1f;

    /// <summary>
    /// 最大のアルファ値
    /// </summary>
    private float maxAlpha = 1f;

    /// <summary>
    /// Tweenタイプ
    /// </summary>
    public Tween.Type Tween { private get; set; } = default;

    /// <summary>
    /// 何かしら状態遷移が終わった際に呼ばれるコールバック
    /// </summary>
    public System.Action<GlowMover> OnFinish { private get; set; } = null;

    /// <summary>
    /// 最小のアルファ値
    /// </summary>
    private float minAlpha = 0;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// アイドルかどうか
    /// </summary>
    public bool IsIdle => (this.state.StateKey == State.Idle);

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // Component取得
      this.main = MyGameObject.Create<SpriteRenderer>("Main", CacheTransform);
      this.main.sortingOrder = Define.Layer.Order.Layer00;

      this.additive = MyGameObject.Create<SpriteRenderer>("Addtive", CacheTransform);
      this.additive.sortingOrder = Define.Layer.Order.Layer00 + 1;

      // デフォルトでFlashは無効
      DisableFlash();

      // 状態の構築
      this.state.Add(State.Idle, OnIdleEnter);
      this.state.Add(State.Move, OnMoveEnter, OnMoveUpdate);
      this.state.Add(State.Scale, OnScaleEnter, OnScaleUpdate);
      this.state.Add(State.Flash, OnFlashEnter, OnFlashUpdate);
      this.state.Add(State.Usual);
      this.state.SetState(State.Idle);
    }

    protected override void MyUpdate()
    {
      base.MyUpdate();

      if (this.state.StateKey != State.Idle) {
        UpdateFlash();
        UpdateTimer();
      }
    }

    /// <summary>
    /// 光る処理
    /// </summary>
    private void UpdateFlash()
    {
      float rate = Mathf.Abs(Mathf.Sin(this.timer * this.cycle));
      this.color.a = rate * this.maxAlpha;
      this.color.a = Mathf.Max(minAlpha, this.color.a);
      this.additive.color = this.color;
    }

    //-------------------------------------------------------------------------
    // セットアップ

    /// <summary>
    /// メインのスプライトと加算合成用マテリアルを渡す
    /// </summary>
    public void Setup(Sprite sprite, Material material, string layerName)
    {
      this.main.sprite  = this.additive.sprite = sprite;
      this.additive.material = material;
      this.main.sortingLayerName = this.additive.sortingLayerName = layerName;
    }

    private void Setup(Vector3 start, Vector3 end, float time)
    {
      this.start = start;
      this.end = end;
      this.time = time;
    }

    /// <summary>
    /// Flashの設定
    /// </summary>
    public void SetFlash(float cycle, float min, float max)
    {
      this.cycle = cycle;
      this.minAlpha = min;
      this.maxAlpha = max;

      this.color.a = 0;
      this.additive.color = this.color;
    }

    /// <summary>
    /// Flashの無効化
    /// </summary>
    public void DisableFlash()
    {
      SetFlash(0, 0, 0);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    public void ToMove(Vector3 start, Vector3 end, float time)
    {
      Setup(start, end, time);
      this.state.SetState(State.Move);
    }

    public void ToScale(Vector3 start, Vector3 end, float time)
    {
      Setup(start, end, time);
      this.state.SetState(State.Scale);
    }

    public void ToFlash(float time)
    {
      this.time = time;
      this.state.SetState(State.Flash);
    }

    public void ToUsual()
    {
      this.state.SetState(State.Usual);
    }

    //-------------------------------------------------------------------------
    // Idle

    private void OnIdleEnter()
    {
      DisableFlash();
    }

    //-------------------------------------------------------------------------
    // Move
    // startからendまで移動

    private void OnMoveEnter()
    {
      CacheTransform.position = start;
      this.timer = 0;
    }

    private void OnMoveUpdate()
    {
      float rate = MyGame.Tween.easing(this.Tween, this.timer / this.time);
      CacheTransform.position = Vector3.Lerp(this.start, this.end, rate);

      if (this.time < this.timer) {
        OnFinish?.Invoke(this);
        this.state.SetState(State.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // Scale
    // startからendまでスケーリング

    private void OnScaleEnter()
    {
      this.timer = 0;
      CacheTransform.localScale = start;
    }

    private void OnScaleUpdate()
    {
      float rate = MyGame.Tween.easing(this.Tween, this.timer / this.time);

      CacheTransform.localScale
        = MyVector3.Lerp(start, end, rate);

      if (this.time < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // Flash
    // ピカピカする

    private void OnFlashEnter()
    {
      CacheTransform.localScale = Vector3.one;
      this.timer = 0;
      this.color.a = 0;
      this.additive.color = this.color;
    }

    private void OnFlashUpdate()
    {
      if (this.time < this.timer) {
        this.state.SetState(State.Idle);
      }
    }
  }

}

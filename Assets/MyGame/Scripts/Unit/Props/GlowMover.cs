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
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// Main Sprite Renderer
    /// </summary>
    private SpriteRenderer main = null;

    /// <summary>
    /// Sub Sprite Renderer
    /// </summary>
    private SpriteRenderer sub = null;

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
    public float MinAlpha { private get; set; } = 0f;

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

      this.sub = MyGameObject.Create<SpriteRenderer>("Addtive", CacheTransform);
      this.sub.sortingOrder = Define.Layer.Order.Layer00 + 1;

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Move, OnMoveEnter, OnMoveUpdate);
      this.state.Add(State.Scale, OnScaleEnter, OnScaleUpdate);
      this.state.Add(State.Flash, OnFlashEnter, OnFlashUpdate);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // セットアップ

    /// <summary>
    /// メインのスプライトと加算合成用マテリアルを渡す
    /// </summary>
    public void Setup(Sprite sprite, Material material, string layerName)
    {
      this.main.sprite  = this.sub.sprite = sprite;
      this.sub.material = material;
      this.main.sortingLayerName = this.sub.sortingLayerName = layerName;
    }

    private void Setup(Vector3 start, Vector3 end, float time)
    {
      this.start = start;
      this.end = end;
      this.time = time;
    }

    public void SetAdditive(float cycle, float maxAlpha)
    {
      this.cycle = cycle;
      this.maxAlpha = maxAlpha;
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

    public void ToFlash(float time, float cycle, float maxAlpha)
    {
      this.SetAdditive(cycle, maxAlpha);
      this.time = time;
      this.state.SetState(State.Flash);
    }

    //-------------------------------------------------------------------------
    // Move
    // startからendまで移動

    private void OnMoveEnter()
    {
      CacheTransform.position = start;
      this.color.a = Random.Range(0, 1f);
      this.sub.color = this.color;
      this.timer = 0;
    }

    private void OnMoveUpdate()
    {
      float rate = MyGame.Tween.easing(this.Tween, this.timer / this.time);

      CacheTransform.position = Vector3.Lerp(this.start, this.end, rate);

      this.color.a = Mathf.Abs(Mathf.Sin(rate * cycle)) * maxAlpha;
      this.sub.color = this.color;

      UpdateTimer();

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
      CacheTransform.localScale = start;
      this.color.a = 0;
      this.sub.color = this.color;
      this.timer = 0;
    }

    private void OnScaleUpdate()
    {
      float rate = MyGame.Tween.easing(this.Tween, this.timer / this.time);

      CacheTransform.localScale
        = MyVector3.Lerp(start, end, rate);

      UpdateTimer();

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
      this.sub.color = this.color;
    }

    private void OnFlashUpdate()
    {
      float rate = this.timer / this.time;
      rate = MyGame.Tween.easing(this.Tween, Mathf.Abs(Mathf.Sin(rate * this.cycle)));
      this.color.a = rate * this.maxAlpha;
      this.color.a = Mathf.Max(MinAlpha, this.color.a);
      this.sub.color = this.color;

      UpdateTimer();

      if (this.time < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

  }

}

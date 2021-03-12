using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyGame.Unit.Versus
{
  /// <summary>
  /// 肉球エフェクト
  /// </summary>
  public class PawEffect : Unit<PawEffect.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Usual,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    /// <summary>
    /// Spriteリスト
    /// </summary>
    private List<Sprite> sprites = new List<Sprite>();

    /// <summary>
    /// 汎用タイマー
    /// </summary>
    private float timer = 0;

    /// <summary>
    /// スプライトの枚数
    /// </summary>
    private int spriteCount = 0;

    /// <summary>
    /// アニメーション間隔
    /// </summary>
    public float Interval { private get; set; }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      this.spriteRenderer = AddComponent<SpriteRenderer>();
      this.spriteRenderer.sortingLayerName = Define.Layer.Sorting.Effect;

      // 状態を構築
      this.state.Add(State.Idle, OnIdleEnter);
      this.state.Add(State.Usual, OnUsualEnter, OnUsualUpdate);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // 設定

    public void Reset()
    {
      this.sprites.Clear();    
      this.timer    = 0;
      this.Interval = 0;
      this.spriteRenderer.sprite = null;
    }

    public PawEffect AddSprite(Sprite sprite)
    {
      this.sprites.Add(sprite);
      return this;
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    public void ToIdle()
    {
      this.state.SetState(State.Idle);
    }

    public void ToUsual()
    {
      this.state.SetState(State.Usual);
    }

    private void OnIdleEnter()
    {
      Reset();
    }

    private void OnUsualEnter()
    {
      // Spriteが設定されてなければIdleへ
      if (this.sprites.Count <= 0) {
        this.state.SetState(State.Idle);
        return;
      }

      this.spriteCount = this.sprites.Count;
      this.timer = 0;
      this.Interval = Mathf.Max(0.01f, Interval);
      this.SetActive(true);
    }

    private void OnUsualUpdate()
    {
      int index = (int)(this.timer/this.Interval) % this.spriteCount;
      this.spriteRenderer.sprite = this.sprites[index];
      this.timer += TimeSystem.Instance.DeltaTime;
    }

  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyGame.Unit.Versus
{
  /// <summary>
  /// 肉球エフェクト
  /// </summary>
  public class PawEffect : Unit<PawEffect.State>, PawEffectManager.IPawEffect
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
    // 定数

    /// <summary>
    /// アニメーション速度の下限値
    /// </summary>
    private const float MIN_ANIM_SPEED = 0.01f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    /// <summary>
    /// Spriteリスト
    /// </summary>
    private readonly List<Sprite> sprites = new List<Sprite>();

    /// <summary>
    /// スプライトの枚数
    /// </summary>
    private int spriteCount = 0;

    /// <summary>
    /// アニメーション間隔
    /// </summary>
    private float interval = 0;

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      this.spriteRenderer = AddComponent<SpriteRenderer>();
      this.spriteRenderer.sortingLayerName = Define.Layer.Sorting.PawEffect;

      // 状態を構築
      this.state.Add(State.Idle, OnIdleEnter);
      this.state.Add(State.Usual, OnUsualEnter, OnUsualUpdate);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // 設定

    /// <summary>
    /// リセット
    /// </summary>
    private void Reset()
    {
      this.sprites.Clear();
      this.timer = 0;
      this.interval = 0;
      this.spriteRenderer.sprite = null;
    }

    /// <summary>
    /// セットアップ
    /// </summary
    void PawEffectManager.IPawEffect.Setup(List<Sprite> sprites, float animApeed, int sortingOrder)
    {
      // 前の設定が残ってるかもなのでとりまリセット
      Reset();

      // Sprite、アニメーション間隔、ソート順を指定
      sprites.ForEach((sprite) => { this.sprites.Add(sprite); });
      this.interval = animApeed;
      this.spriteRenderer.sortingOrder = sortingOrder;
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
      this.interval = Mathf.Max(MIN_ANIM_SPEED, interval);
      this.SetActive(true);
    }

    private void OnUsualUpdate()
    {
      int index = (int)(this.timer/this.interval) % this.spriteCount;
      this.spriteRenderer.sprite = this.sprites[index];
      this.timer += TimeSystem.Instance.DeltaTime;
    }

  }
}
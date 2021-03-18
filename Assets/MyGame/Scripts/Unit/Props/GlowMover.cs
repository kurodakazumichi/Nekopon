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
      Move,
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
    /// 開始地点
    /// </summary>
    private Vector3 startPosition = Vector3.zero;

    /// <summary>
    /// 目標地点
    /// </summary>
    private Vector3 endPosition = Vector3.zero;

    /// <summary>
    /// 降り注ぐ時間
    /// </summary>
    private float time = 0;

    /// <summary>
    /// 偏り
    /// </summary>
    private float bias = 0f;

    /// <summary>
    /// 何かしら状態遷移が終わった際に呼ばれるコールバック
    /// </summary>
    public System.Action<GlowMover> OnFinish { private get; set; } = null;

    /// <summary>
    /// 加算合成変化のサイクル
    /// </summary>
    private float cycle = 1f;

    /// <summary>
    /// 最大のアルファ値
    /// </summary>
    private float maxAlpha = 1f;

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

    public void SetAdditive(float cycle, float maxAlpha)
    {
      this.cycle = cycle;
      this.maxAlpha = maxAlpha;
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    public void ToMove(Vector3 start, Vector3 end, float time)
    {
      this.startPosition = start;
      this.endPosition = end;
      this.time = time;
      this.state.SetState(State.Move);
    }

    //-------------------------------------------------------------------------
    // Move
    // startPositionからendPositionに向かって移動

    private void OnMoveEnter()
    {
      CacheTransform.position = startPosition;
      this.color.a = Random.Range(0, 1f);
      this.sub.color = this.color;
      this.timer = 0;
    }

    private void OnMoveUpdate()
    {
      float rate = this.timer / this.time;

      CacheTransform.position = Vector3.Lerp(this.startPosition, this.endPosition, rate);

      this.color.a = Mathf.Abs(Mathf.Sin(rate * cycle)) * maxAlpha;
      this.sub.color = this.color;

      UpdateTimer();

      if (this.time < this.timer) {
        OnFinish?.Invoke(this);
        this.state.SetState(State.Idle);
      }
    }

  }

}

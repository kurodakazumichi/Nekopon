using UnityEngine;

namespace MyGame.Unit.Mover
{
  /// <summary>
  /// Moverを操作するクラス
  /// </summary>
  public class MoverOperator
  {
    /// <summary>
    /// 操作の種類
    /// </summary>
    public enum State
    {
      Idle,
      Move,
      Scale,
      Usual,
      Flash,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// Tweenタイプ
    /// </summary>
    public Tween.Type Tween { private get; set; } = default;

    /// <summary>
    /// Idleになった時に呼ばれるコールバック
    /// </summary>
    public System.Action OnIdle = null;

    /// <summary>
    /// 操作する対称
    /// </summary>
    private IMover target = null;

    /// <summary>
    /// 状態
    /// </summary>
    private StateMachine<State> state = new StateMachine<State>();

    /// <summary>
    /// 開始ベクトル
    /// </summary>
    private Vector3 start = Vector3.zero;

    /// <summary>
    /// 目標ベクトル
    /// </summary>
    private Vector3 end = Vector3.zero;

    /// <summary>
    /// アルファ変化のサイクル
    /// </summary>
    private float cycle = 1f;

    /// <summary>
    /// 最小のアルファ値
    /// </summary>
    private float minAlpha = 0;

    /// <summary>
    /// 最大のアルファ値
    /// </summary>
    private float maxAlpha = 1f;

    /// <summary>
    /// 汎用タイマー
    /// </summary>
    private float timer = 0;

    /// <summary>
    /// 汎用タイム(制限時間)
    /// </summary>
    private float time = 0;

    /// <summary>
    /// Flashが有効かどうか
    /// </summary>
    private bool isEnableFlash = false;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// アイドルかどうか
    /// </summary>
    public bool IsIdle => (this.state.StateKey == State.Idle);

    /// <summary>
    /// 輝度
    /// </summary>
    private float Brightness {
      set { this.target.Brightness = value; }
    }

    //-------------------------------------------------------------------------
    // コンストラクタ

    public MoverOperator()
    {
      this.state.Add(State.Idle, OnIdleEnter);
      this.state.Add(State.Move, OnMoveEnter, OnMoveUpdate);
      this.state.Add(State.Scale, OnScaleEnter, OnScaleUpdate);
      this.state.Add(State.Flash, OnFlashEnter, OnFlashUpdate);
      this.state.Add(State.Usual);
      this.state.SetState(State.Idle);
    }

    public void Update()
    {
      this.state.Update();

      if (this.isEnableFlash) {
        UpdateBrightness();
      }

      if (!IsIdle) {
        this.timer += TimeSystem.Instance.DeltaTime;
      }
    }

    /// <summary>
    /// 輝度の更新
    /// </summary>
    private void UpdateBrightness()
    {
      float rate = Mathf.Abs(Mathf.Sin(this.timer * this.cycle));
      Brightness = Mathf.Max(minAlpha, rate * this.maxAlpha);
    }

    //-------------------------------------------------------------------------
    // セットアップ

    /// <summary>
    /// 動かす対称となるMoverをセットする
    /// </summary>
    public void SetMover(IMover mover)
    {
      this.target = mover;
    }

    /// <summary>
    /// SpriteRendererの基本セットアップ
    /// </summary>
    public void Setup(SpriteRenderer sr, Sprite sprite, Material material, string layerName)
    {
      sr.sprite = sprite;

      if (material) {
        sr.material = material;
      }

      sr.sortingLayerName = layerName;
    }

    public void Setup(Sprite sprite, Material material, string layerName)
    {
      Setup(this.target.Renderer, sprite, material, layerName);
    }

    /// <summary>
    /// 位置、スケール、時間のセットアップ
    /// </summary>
    protected void Setup(Vector3 start, Vector3 end, float time)
    {
      this.start = start;
      this.end   = end;
      this.time  = time;
    }

    /// <summary>
    /// Flashの設定
    /// </summary>
    public void SetFlash(float cycle, float minBrightness, float maxBrightness)
    {
      this.cycle = cycle;
      this.minAlpha = minBrightness;
      this.maxAlpha = maxBrightness;
      Brightness = 0;
      this.isEnableFlash = true;
    }

    /// <summary>
    /// Flashの無効化
    /// </summary>
    public void DisableFlash()
    {
      this.isEnableFlash = false;
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    /// <summary>
    /// Idleへ
    /// </summary>
    public void ToIdle()
    {
      this.state.SetState(State.Idle);
    }

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

    public void ToUsual(Vector3 position, Vector3 scale)
    {
      this.target.CacheTransform.position = position;
      this.target.CacheTransform.localScale = scale;
      this.state.SetState(State.Usual);
    }

    //-------------------------------------------------------------------------
    // Idle

    private void OnIdleEnter()
    {
      OnIdle?.Invoke();
    }

    //-------------------------------------------------------------------------
    // Move
    // startからendまで移動

    private void OnMoveEnter()
    {
      this.target.CacheTransform.position = start;
      this.timer = 0;
    }

    private void OnMoveUpdate()
    {
      float rate = MyGame.Tween.easing(this.Tween, this.timer / this.time);
      this.target.CacheTransform.position = MyVector3.Lerp(this.start, this.end, rate);

      if (this.time < this.timer) ToIdle();
    }

    //-------------------------------------------------------------------------
    // Scale
    // startからendまでスケーリング

    private void OnScaleEnter()
    {
      this.timer = 0;
      this.target.CacheTransform.localScale = start;
    }

    private void OnScaleUpdate()
    {
      float rate = MyGame.Tween.easing(this.Tween, this.timer / this.time);

      this.target.CacheTransform.localScale = MyVector3.Lerp(start, end, rate);

      if (this.time < this.timer) ToIdle();
    }

    //-------------------------------------------------------------------------
    // Flash

    private void OnFlashEnter()
    {
      this.target.CacheTransform.localScale = Vector3.one;
      this.timer = 0;
      Brightness = 0;
    }

    private void OnFlashUpdate()
    {
      if (this.time < this.timer) {
        this.state.SetState(State.Idle);
        DisableFlash();
      }
    }
  }
}
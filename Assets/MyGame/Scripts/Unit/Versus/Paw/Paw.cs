using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;
using System;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// パズル内の肉球
  /// </summary>
  public partial class Paw : Unit<Paw.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State {
      Idle,     // アイドル
      Vanish,   // 消滅
      Move,     // 移動
      Usual,    // 通常
      Selected, // 選択されている
    }

    /// <summary>
    /// 状態異常などのサブステート(配列の添え字に使う)
    /// </summary>
    public enum SubState
    {
      Freeze    = 0, // 凍結
      Paralysis = 1,   // 麻痺
    }

    //-------------------------------------------------------------------------
    // Inspector設定項目

    /// <summary>
    /// 消滅にかかる時間
    /// </summary>
    [SerializeField]
    private float _VanishingTime = 0.5f;

    /// <summary>
    /// 消滅時に回転するけど、その時の回転スピード
    /// </summary>
    [SerializeField]
    private float _VanishingAngularSpeed = 1080f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

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

    /// <summary>
    /// ステータス(凍結、麻痺)
    /// </summary>
    private List<Status> status = new List<Status>();

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// 属性
    /// </summary>
    public App.Attribute Attribute { get; private set; } = default;

    /// <summary>
    /// 連鎖時に使用する、評価済フラグ
    /// </summary>
    public bool IsEvaluated { get; set; } = false;

    /// <summary>
    /// 消滅可能かどうか、状態によっては消滅状態へ遷移できないなどの判定をここで行う
    /// </summary>
    public bool CanVanish {
      get {
        return !IsEvaluated; // まだ評価されてない
      }
    }

    /// <summary>
    /// 選択可能かどうか
    /// まず麻痺っていたら選択不可で、すでに選択中とか消えてる最中とかも
    /// 選択できるとまずいので、通常状態の時だけ選択可能ということにする
    /// </summary>
    public bool CanSelect
    {
      get {
        if (this.IsParalyzed) return false;
        if (this.state.StateKey != State.Usual) return false;
        return true;
      }
    }

    /// <summary>
    /// 入れ替え可能かどうか
    /// 麻痺している肉球は動かせないので入れ替えも不可
    /// </summary>
    public bool CanSwap {
      get {
        if (this.IsParalyzed) return false;
        return true;
      }
    }

    /// <summary>
    /// 消滅状態へ移行可能かどうか
    /// </summary>
    private bool CanToVanish {
      get {
        if (this.state.StateKey == State.Usual) return true;
        if (this.state.StateKey == State.Selected) return true;
        return false;
      }
    }

    /// <summary>
    /// アイドルです
    /// </summary>
    public bool IsIdle => (this.state.StateKey == State.Idle);

    /// <summary>
    /// 移動中です
    /// </summary>
    public bool IsMoving => (this.state.StateKey == State.Move);

    /// <summary>
    /// 肉球が選択されているかどうかの状態を表すフラグ
    /// </summary>
    public bool IsSelected {
      get { return this.state.StateKey == State.Selected; }
    }

    /// <summary>
    /// 消失中です
    /// </summary>
    public bool IsVanishing => (this.state.StateKey == State.Vanish);
    
    /// <summary>
    /// 凍結しているか？
    /// </summary>
    public bool IsFreeze => this.status[(int)SubState.Freeze].IsActive;

    /// <summary>
    /// 麻痺しているか？
    /// </summary>
    public bool IsParalyzed => this.status[(int)SubState.Paralysis].IsActive;

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// Spriteリソース
    /// </summary>
    private static readonly Dictionary<App.Attribute, Sprite> Sprites 
      = new Dictionary<App.Attribute, Sprite>(App.AttributeCount);

    public static void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceSystem.Instance;
      rm.Load<Sprite>("Paw.Fir.sprite", pre, done, (res) => { Sprites[App.Attribute.Fir] = res; });
      rm.Load<Sprite>("Paw.Wat.sprite", pre, done, (res) => { Sprites[App.Attribute.Wat] = res; });
      rm.Load<Sprite>("Paw.Thu.sprite", pre, done, (res) => { Sprites[App.Attribute.Thu] = res; });
      rm.Load<Sprite>("Paw.Ice.sprite", pre, done, (res) => { Sprites[App.Attribute.Ice] = res; });
      rm.Load<Sprite>("Paw.Tre.sprite", pre, done, (res) => { Sprites[App.Attribute.Tre] = res; });
      rm.Load<Sprite>("Paw.Hol.sprite", pre, done, (res) => { Sprites[App.Attribute.Hol] = res; });
      rm.Load<Sprite>("Paw.Dar.sprite", pre, done, (res) => { Sprites[App.Attribute.Dar] = res; });
    }

    public static void Unload()
    {
      var rm = ResourceSystem.Instance;
      rm.Unload("Paw.Fir.sprite");
      rm.Unload("Paw.Wat.sprite");
      rm.Unload("Paw.Thu.sprite");
      rm.Unload("Paw.Ice.sprite");
      rm.Unload("Paw.Tre.sprite");
      rm.Unload("Paw.Hol.sprite");
      rm.Unload("Paw.Dar.sprite");
      Sprites.Clear();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      this.spriteRenderer = CacheTransform.Find("Sprite").GetComponent<SpriteRenderer>();

      this.state.Add(State.Idle);
      this.state.Add(State.Vanish, OnVanishEnter, OnVanishUpdate, OnVanishExit);
      this.state.Add(State.Move, OnMoveEnter, OnMoveUpdate, OnMoveExit);
      this.state.Add(State.Usual, OnUsualEnter);
      this.state.Add(State.Selected, OnSelectedEnter, OnSelectedUpdate);
      this.state.SetState(State.Idle);

      // Freeze→Paralysisの順で登録する
      this.status.Add(new Status(PawEffectManager.Type.Freeze   , Define.Versus.PAW_FREEZE_TIME, CacheTransform));
      this.status.Add(new Status(PawEffectManager.Type.Paralysis, Define.Versus.PAW_PARALYSIS_TIME, CacheTransform));
    }

    protected override void MyUpdate()
    {
      base.MyUpdate();
      this.status.ForEach((status) => { status.Update(); });
    }

    //-------------------------------------------------------------------------
    // Publicメソッド

    /// <summary>
    /// 消滅状態へ
    /// </summary>
    public void ToVanish()
    {
      if (!CanToVanish) return;
      this.state.SetState(State.Vanish);
    }

    public void ToMove(Vector3 target, float time, Tween.Type type = Tween.Type.EaseInSine)
    {
      this.tweenType = type;
      this.movingTime = time;
      this.moveEndPosition = target;
      this.state.SetState(State.Move);
    }

    /// <summary>
    /// 通常状態へ
    /// </summary>
    public void ToUsual()
    {
      this.state.SetState(State.Usual);
    }

    /// <summary>
    /// 選択状態へ
    /// </summary>
    public void ToSelected()
    {
      this.state.SetState(State.Selected);
    }

    /// <summary>
    /// 属性をランダムに変更
    /// </summary>
    public void RandomAttribute()
    {
      this.Attribute = MyEnum.Random<Define.App.Attribute>();
      this.spriteRenderer.sprite = Sprites[this.Attribute];
    }

    /// <summary>
    /// 指定された肉球と繋がる事ができるかどうか
    /// </summary>
    public bool CanConnect(Paw paw)
    {
      // 凍結していたら繋がれない
      if (this.IsFreeze) return false;

      // 属性が異なるなら繋がれない
      if (this.Attribute != paw.Attribute) return false;

      // Idle状態なら繋がれない
      if (this.state.StateKey == State.Idle) return false;

      return true;
    }

    /// <summary>
    /// 肉球を凍結させる
    /// </summary>
    public void Freeze()
    {
      this.status[(int)SubState.Freeze].Start();
    }

    /// <summary>
    /// 肉球を麻痺らせる
    /// </summary>
    public void Paralyze()
    {
      this.status[(int)SubState.Paralysis].Start();
    }

    /// <summary>
    /// 肉球を治療する
    /// </summary>
    public void Cure()
    {
      this.status.ForEach((status) => { status.Finish(); });
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    //-------------------------------------------------------------------------
    // 消滅

    private void OnVanishEnter()
    {
      SetDefaultParams();
      this.timer = 0;
    }

    private void OnVanishUpdate()
    {
      float deltaTime = TimeSystem.Instance.DeltaTime;
      
      this.CacheTransform.Rotate(0, 0, _VanishingAngularSpeed * deltaTime);
      this.CacheTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, this.timer/_VanishingTime);

      if (_VanishingTime <= this.timer) { this.state.SetState(State.Idle); }

      this.timer += deltaTime;
    }

    private void OnVanishExit()
    {
      Cure();
      CacheTransform.localScale = Vector3.zero;
    }

    //-------------------------------------------------------------------------
    // 移動

    private void OnMoveEnter()
    {
      this.SetDefaultParams();
      this.moveStartPosition = CacheTransform.position;
      this.timer = 0;
    }

    private void OnMoveUpdate()
    {
      // 現在の進行度
      float t = this.timer/movingTime;
      t = Tween.easing(this.tweenType, t);

      // 座標更新
      CacheTransform.position = Vector3.Lerp(this.moveStartPosition, this.moveEndPosition, t);

      // 時間経過していたら移動完了フェーズへ
      if (movingTime <= this.timer) { this.state.SetState(State.Usual); }
      this.timer += TimeSystem.Instance.DeltaTime;
    }

    private void OnMoveExit()
    {
      CacheTransform.position = this.moveEndPosition;
    }

    //-------------------------------------------------------------------------
    // 通常

    private void OnUsualEnter()
    {
      SetDefaultParams();
    }

    //-------------------------------------------------------------------------
    // 選択中

    private void OnSelectedEnter()
    {
      SetDefaultParams();
      this.spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
      this.timer = 0;
    }

    private void OnSelectedUpdate()
    {
      CacheTransform.localScale = Vector3.Lerp(
        Vector3.one, 
        Vector3.one * 0.9f, 
        Tween.EaseInSine(Mathf.Abs(Mathf.Sin(this.timer * Mathf.PI)))
      );

      this.timer += TimeSystem.Instance.DeltaTime;
    }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 各種パラメータをデフォルト状態にする
    /// </summary>
    private void SetDefaultParams()
    {
      CacheTransform.localScale = Vector3.one;
      CacheTransform.rotation = Quaternion.identity;
      this.spriteRenderer.color = Color.white;
    }

  }

}
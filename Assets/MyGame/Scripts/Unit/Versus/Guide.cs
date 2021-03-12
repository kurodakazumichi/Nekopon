using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.VersusManagement;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// Ready, Goなどのガイド用ユニット
  /// 単純な動きが多いので１つのガイドとしてまとめてしまう。
  /// </summary>
  public class Guide : Unit<Guide.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Ready,
      ReadyWait,
      Go,
      GoWait,
      GoBack,
      Result,
      ResultWait,
      Retry,
      RetryWait,
      Finished,
    }

    /// <summary>
    /// ガイドの種類(※Spriteの名称と一致させておく)
    /// </summary>
    private enum Type
    {
      Ready,
      Go,
      Win,
      Lose,
      Continue,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// P1のベース位置
    /// </summary>
    private Vector3 p1Base = Vector3.zero;

    /// <summary>
    /// P2のベース位置
    /// </summary>
    private Vector3 p2Base = Vector3.zero;

    /// <summary>
    /// ガイドオブジェクト
    /// </summary>
    private Dictionary<Type, MyMonoBehaviour> guides = new Dictionary<Type, MyMonoBehaviour>();

    /// <summary>
    /// Readyオブジェクト
    /// </summary>
    private MyMonoBehaviour Ready => this.guides[Type.Ready];

    /// <summary>
    /// Goオブジェクト
    /// </summary>
    private MyMonoBehaviour Go => this.guides[Type.Go];

    /// <summary>
    /// Winオブジェクト
    /// </summary>
    private MyMonoBehaviour Win => this.guides[Type.Win];

    /// <summary>
    /// Loseオブジェクト
    /// </summary>
    private MyMonoBehaviour Lose => this.guides[Type.Lose];

    /// <summary>
    /// Retryオブジェクト
    /// </summary>
    private MyMonoBehaviour Retry => this.guides[Type.Continue];

    /// <summary>
    /// 汎用タイマー
    /// </summary>
    private float timer = 0;

    /// <summary>
    /// 動作に要する時間
    /// </summary>
    private float time = 0;

    /// <summary>
    /// 勝者
    /// </summary>
    private Define.App.Player winner = default;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// 終了ステートにいるか
    /// </summary>
    public bool IsFinished => (this.state.StateKey == State.Finished);

    /// <summary>
    /// 勝者の位置
    /// </summary>
    private Vector3 WinnerPos => (this.winner == Define.App.Player.P1) ? p1Base : p2Base;

    /// <summary>
    /// 敗者の位置
    /// </summary>
    private Vector3 LoserPos => (this.winner == Define.App.Player.P1)? p2Base : p1Base;

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// Spriteリソース
    /// </summary>
    private static readonly Dictionary<Type, Sprite> Sprites
      = new Dictionary<Type, Sprite>();

    public static void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceSystem.Instance;
      MyEnum.ForEach<Type>((type) => { 
        rm.Load<Sprite>($"Logo.{type}.sprite", pre, done, (res) => { Sprites[type] = res; });
      });
    }

    public static void Unload()
    {
      var rm = ResourceSystem.Instance;
      MyEnum.ForEach<Type>((type) => { rm.Unload($"Logo.{type}.sprite"); });
      Sprites.Clear();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      MyEnum.ForEach<Type>((type) => {  
        this.guides[type] = CreateGameObject(type);
      });

      this.state.Add(State.Idle, OnIdleEnter);
      this.state.Add(State.Ready, OnReadyEnter, OnReadyUpdate);
      this.state.Add(State.ReadyWait, OnReadyWaitEnter, OnReadyWaitUpdate);
      this.state.Add(State.Go, OnGoEnter, OnGoUpdate);
      this.state.Add(State.GoWait, OnGoWaitEnter, OnGoWaitUpdate);
      this.state.Add(State.GoBack, OnGoBackEnter, OnGoBackUpdate);
      this.state.Add(State.Result, OnResultEnter, OnResultUpdate);
      this.state.Add(State.ResultWait, OnResultWaitEnter);
      this.state.Add(State.Retry, OnRetryEnter, OnRetryUpdate);
      this.state.Add(State.RetryWait, OnRetryWaitEnter);
      this.state.Add(State.Finished, OnFinisedEnter);
      this.state.SetState(State.Idle);
    }

    /// <summary>
    /// GameObjectを生成する
    /// </summary>
    private MyMonoBehaviour CreateGameObject(Type type)
    {
      var go = new GameObject($"{type}");
      go.AddComponent<SpriteRenderer>().sortingLayerName = Define.Layer.Sorting.UI;

      return go.AddComponent<MyMonoBehaviour>().SetParent(CacheTransform);
    }

    //-------------------------------------------------------------------------
    // 初期処理

    /// <summary>
    /// 初期処理
    /// </summary>
    public void Init(Vector3 p1Base, Vector3 p2Base) 
    {
      this.p1Base = p1Base;
      this.p2Base = p2Base;
    }

    /// <summary>
    /// セットアップ(呼ぶのはInitの後)
    /// </summary>
    public void Setup()
    {
      // Spriteをセット
      Util.ForEach(this.guides, (type, guide) => {
        guide.gameObject.GetComponent<SpriteRenderer>().sprite = Sprites[type];
      });

      SetActives(false);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    /// <summary>
    /// アイドルにする
    /// </summary>
    public void ToIdle()
    {
      this.state.SetState(State.Idle);
    }

    /// <summary>
    /// Ready?を表示する
    /// </summary>
    public void ToReady()
    {
      this.state.SetState(State.Ready);
    }

    /// <summary>
    /// Go!を表示する
    /// </summary>
    public void ToGo()
    {
      this.state.SetState(State.Go);
    }

    /// <summary>
    /// Result(Win/Loase)を表示する
    /// </summary>
    public void ToResult(Define.App.Player winner)
    {
      this.winner = winner;
      this.state.SetState(State.Result);
    }
    
    public void ToRetry()
    {
      this.state.SetState(State.Retry);
    }

    //-------------------------------------------------------------------------
    // アイドル状態

    private void OnIdleEnter()
    {
      this.SetActives(false);
    }

    //-------------------------------------------------------------------------
    // Ready? のガイド表示ステート

    private void OnReadyEnter()
    {
      this.Ready.SetActive(true);
      this.timer = 0;
      this.time  = 0.7f;
    }

    private void OnReadyUpdate()
    {
      this.Ready.CacheTransform.position 
        = MyVector3.Lerp(Vector3.left, Vector3.zero, Tween.EaseOutBack(this.timer/this.time));

      this.timer += TimeSystem.Instance.DeltaTime;

      if (this.timer < this.time) return;
      this.state.SetState(State.ReadyWait);
    }

    private void OnReadyWaitEnter()
    {
      this.Ready.CacheTransform.position = Vector3.zero;
      this.timer = 0;
      this.time  = 1f;
    }

    private void OnReadyWaitUpdate()
    {
      this.timer += TimeSystem.Instance.DeltaTime;

      if (this.timer < this.time) return;
      this.state.SetState(State.Finished);
    }

    //-------------------------------------------------------------------------
    // Go!のガイド表示

    private void OnGoEnter()
    {
      this.time = 0.7f;
      this.timer = 0;
      this.Go.SetActive(true);
    }

    private void OnGoUpdate()
    {
      this.Go.CacheTransform.position
        = Vector3.Lerp(Vector3.up, Vector3.zero, Tween.EaseOutBounce(this.timer/this.time));

      this.timer += TimeSystem.Instance.DeltaTime;

      if (this.timer < this.time) return;
      this.state.SetState(State.GoWait);
    }

    private void OnGoWaitEnter()
    {
      this.Go.CacheTransform.position = Vector3.zero;
      this.time = 0.6f;
      this.timer = 0;
    }

    private void OnGoWaitUpdate()
    {
      this.timer += TimeSystem.Instance.DeltaTime;

      if (this.timer < this.time) return;
      this.state.SetState(State.GoBack);
    }

    private void OnGoBackEnter()
    {
      this.Go.CacheTransform.position = Vector3.zero;
      this.time = 0.7f;
      this.timer = 0;
    }

    private void OnGoBackUpdate()
    {
      this.Go.CacheTransform.position
        = MyVector3.Lerp(Vector3.zero, Vector3.up * 2, Tween.EaseInOutBack(this.timer / this.time));

      this.timer += TimeSystem.Instance.DeltaTime;

      if (this.timer < this.time) return;
      this.state.SetState(State.Finished);
    }

    //-------------------------------------------------------------------------
    // Win・Loseの表示State

    private void OnResultEnter()
    {
      this.SetActives(false);
      this.Win.SetActive(true);
      this.Lose.SetActive(true);
      this.timer = 0;
      this.time  = 1f;
    }

    private void OnResultUpdate()
    {
      float rate = this.timer / this.time;

      this.Win.CacheTransform.position
        = MyVector3.Lerp(Vector3.down, WinnerPos, Tween.EaseOutElastic(rate));

      this.Lose.CacheTransform.position
        = MyVector3.Lerp(Vector3.up, LoserPos, Tween.EaseOutElastic(rate));

      this.timer += TimeSystem.Instance.DeltaTime;

      if (this.timer < this.time) return;
      this.state.SetState(State.ResultWait);
    }

    private void OnResultWaitEnter()
    {
      this.Win.CacheTransform.position  = WinnerPos;
      this.Lose.CacheTransform.position = LoserPos;
    }

    //-------------------------------------------------------------------------
    // Continueの表示

    private void OnRetryEnter()
    {
      this.SetActives(false);
      this.Retry.SetActive(true);
      this.timer = 0;
      this.time  = 0.2f;
    }

    private void OnRetryUpdate()
    {
      this.Retry.CacheTransform.localScale
        = MyVector3.Lerp(Vector3.zero, Vector3.one, Tween.EaseOutBack(this.timer / this.time));

      this.timer += TimeSystem.Instance.DeltaTime;

      if (this.timer < this.time) return;
      this.state.SetState(State.RetryWait);
    }

    private void OnRetryWaitEnter()
    {
      this.Retry.CacheTransform.localScale = Vector3.one;
    }

    //-------------------------------------------------------------------------
    // ReadyやGoが終了するとここにくる

    private void OnFinisedEnter()
    {
      SetActives(false);
    }

    /// <summary>
    /// Activeを一括制御
    /// </summary>
    private void SetActives(bool isActive)
    {
      Util.ForEach(this.guides, (type, guide) => { guide.SetActive(isActive); });
    }
  }
}
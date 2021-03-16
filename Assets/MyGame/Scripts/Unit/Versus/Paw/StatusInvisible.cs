using UnityEngine;

namespace MyGame.Unit.Versus
{
  public partial class Paw : Unit<Paw.State>
  {
    /// <summary>
    /// 不可視ステータス
    /// </summary>
    private class StatusInvisible : IStatus
    {
      public enum State
      {
        Idle,
        FadeIn,
        Usual,
        FadeOut,
      }

      //-------------------------------------------------------------------------
      // 定数

      /// <summary>
      /// フェードにかける時間
      /// </summary>
      private const float FADE_TIME = 0.5f;

      /// <summary>
      /// 黒
      /// </summary>
      private static readonly Color BLACK = Color.black;

      /// <summary>
      /// 白
      /// </summary>
      private static readonly Color WHITE = Color.white;

      //-------------------------------------------------------------------------
      // メンバ変数

      /// <summary>
      /// 肉球ユニット
      /// </summary>
      private readonly Paw paw = null;

      /// <summary>
      /// 汎用タイマー
      /// </summary>
      private float timer = 0;

      /// <summary>
      /// ステートマシン
      /// </summary>
      private StateMachine<State> state = new StateMachine<State>();

      //-------------------------------------------------------------------------
      // プロパティ

      /// <summary>
      /// ステータスが有効かどうか
      /// </summary>
      public bool IsActive => (this.state.StateKey != State.Idle);

      //-------------------------------------------------------------------------
      // メソッド

      /// <summary>
      /// コンストラクタ
      /// </summary>
      public StatusInvisible(Paw paw)
      {
        this.paw = paw;

        this.state.Add(State.Idle, OnIdleEnter);
        this.state.Add(State.FadeIn, OnFadeInEnter, OnFadeInUpdate);
        this.state.Add(State.Usual, OnUsualEnter, OnUsualUpdate);
        this.state.Add(State.FadeOut, OnFadeOutEnter, OnFadeOutUpdate);
        this.state.SetState(State.Idle);
      }

      /// <summary>
      /// ステータス効果開始
      /// </summary>
      public void Start()
      {
        switch(this.state.StateKey) 
        {
          // 非アクティブならフェードイン
          case State.Idle: 
            this.state.SetState(State.FadeIn); 
            break;

          // フェードイン中ならなにもしない
          case State.FadeIn: 
            return;

          // 降下中ならタイマーリセット(効果を延長)
          case State.Usual: 
            this.timer = 0; 
            break;

          // フェードアウト中だったらフェードインに以降するけど、タイマーは引き継ぐ
          case State.FadeOut: 
            float timer = FADE_TIME - this.timer;
            this.state.SetState(State.FadeIn); 
            this.timer = timer;
            break;
        }
      }

      /// <summary>
      /// Updateは常に呼び出す
      /// </summary>
      public void Update()
      {
        this.state.Update();
      }

      /// <summary>
      /// Status効果終了
      /// </summary>
      public void Finish()
      {
        // 肉球がアイドルならステータスもアイドルへ
        if (this.paw.IsIdle) {
          this.state.SetState(State.Idle);
          return;
        }

        switch(this.state.StateKey) {
          // 非アクティブの場合は強制Idleへ
          case State.Idle:
            this.state.SetState(State.Idle);
            break;
          // フェードイン中のフェードアウトはタイマーを維持
          case State.FadeIn:
            float timer = FADE_TIME - this.timer;
            this.state.SetState(State.FadeOut);
            this.timer = timer;
            break;
          // 通常状態なら普通にフェードアウト
          case State.Usual:
            this.state.SetState(State.FadeOut);
            break;
          // フェードアウト中なら何もしない
          case State.FadeOut:
            break;
        }
      }

      //-------------------------------------------------------------------------
      // ステートマシン

      private void OnIdleEnter()
      {
        SetColor(WHITE);
      }

      //-------------------------------------------------------------------------
      // フェードイン(白→黒)

      private void OnFadeInEnter()
      {
        this.timer = 0;
      }

      private void OnFadeInUpdate()
      {
        var color = Color.Lerp(WHITE, BLACK, this.timer / FADE_TIME);
        SetColor(color);

        this.timer += TimeSystem.Instance.DeltaTime;

        if (FADE_TIME < this.timer) {
          this.state.SetState(State.Usual);
        }
      }

      //-------------------------------------------------------------------------
      // 通常

      private void OnUsualEnter()
      {
        // 属性変更可能、かつ、聖属性だったら闇属性へ変更する
        if (this.paw.CanChangeAttribute && this.paw.Attribute == Define.App.Attribute.Hol) {
          this.paw.SetAttribute(Define.App.Attribute.Dar);
        }

        // 色を黒くする
        SetColor(BLACK);
        this.timer = 0;
      }

      private void OnUsualUpdate()
      {
        this.timer += TimeSystem.Instance.DeltaTime;

        // 一定時間たったらフェードアウトへ
        if (Define.Versus.PAW_INVISIBLE_TIME < this.timer) {
          this.state.SetState(State.FadeOut);
        }
      }

      //-------------------------------------------------------------------------
      // フェードアウト(黒→白)

      private void OnFadeOutEnter()
      {
        this.timer = 0;
      }

      private void OnFadeOutUpdate()
      {
        var color = Color.Lerp(BLACK, WHITE, this.timer / FADE_TIME);
        SetColor(color);

        this.timer += TimeSystem.Instance.DeltaTime;

        if (FADE_TIME < this.timer) {
          SetColor(WHITE);
          this.state.SetState(State.Idle);
        }
      }

      /// <summary>
      /// 色の更新(RGBのみ更新)
      /// </summary>
      private void SetColor(Color color)
      {
        color.a = this.paw.spriteRenderer.color.a;
        this.paw.spriteRenderer.color = color;
      }
    }
  }
}
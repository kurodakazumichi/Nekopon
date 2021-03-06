using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;

namespace MyGame.Unit.Versus
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
      Idle,     // アイドル
      Vanish,   // 消滅
      Move,     // 移動
      Moved,    // 移動した
      Usual,    // 通常
      Selected, // 選択されている
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
    /// 属性
    /// </summary>
    private Define.App.Attribute attribute = default;

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
    // プロパティ

    /// <summary>
    /// 肉球が選択されているかどうかの状態を表すフラグ
    /// </summary>
    public bool IsSelected {
      get { return this.state.StateKey == State.Selected; }
    }

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// Spriteリソース
    /// </summary>
    private static Dictionary<Define.App.Attribute, Sprite> Sprites = new Dictionary<Define.App.Attribute, Sprite>(Define.App.AttributeCount);

    public static void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceManager.Instance;
      rm.Load<Sprite>("Paw.Fir.sprite", pre, done, (res) => { Sprites[Define.App.Attribute.Fir] = res; });
      rm.Load<Sprite>("Paw.Wat.sprite", pre, done, (res) => { Sprites[Define.App.Attribute.Wat] = res; });
      rm.Load<Sprite>("Paw.Thu.sprite", pre, done, (res) => { Sprites[Define.App.Attribute.Thu] = res; });
      rm.Load<Sprite>("Paw.Ice.sprite", pre, done, (res) => { Sprites[Define.App.Attribute.Ice] = res; });
      rm.Load<Sprite>("Paw.Tre.sprite", pre, done, (res) => { Sprites[Define.App.Attribute.Tre] = res; });
      rm.Load<Sprite>("Paw.Hol.sprite", pre, done, (res) => { Sprites[Define.App.Attribute.Hol] = res; });
      rm.Load<Sprite>("Paw.Dar.sprite", pre, done, (res) => { Sprites[Define.App.Attribute.Dar] = res; });
    }

    public static void Unload()
    {
      var rm = ResourceManager.Instance;
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
      this.state.Add(State.Vanish, EnterVanish, UpdateVanish, ExitVanish);
      this.state.Add(State.Move, EnterMove, UpdateMove, ExitMove);
      this.state.Add(State.Moved);
      this.state.Add(State.Usual, EnterUsual);
      this.state.Add(State.Selected, EnterSelected, UpdateSelected);
      this.state.SetState(State.Idle);
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

    public void ToUsual()
    {
      this.state.SetState(State.Usual);
    }

    public void ToSelected()
    {
      this.state.SetState(State.Selected);
    }

    public void RandomAttribute()
    {
      this.attribute = MyEnum.Random<Define.App.Attribute>();
      this.spriteRenderer.sprite = Sprites[this.attribute];
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    //-------------------------------------------------------------------------
    // 消滅

    private void EnterVanish()
    {
      SetDefaultParams();
      this.timer = 0;
    }

    private void UpdateVanish()
    {
      float deltaTime = TimeManager.Instance.DeltaTime;
      
      this.CacheTransform.Rotate(0, 0, _VanishingAngularSpeed * deltaTime);
      this.CacheTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, this.timer/_VanishingTime);

      if (_VanishingTime <= this.timer) { this.state.SetState(State.Idle); }

      this.timer += deltaTime;
    }

    private void ExitVanish()
    {
      CacheTransform.localScale = Vector3.zero;
    }

    //-------------------------------------------------------------------------
    // 移動

    private void EnterMove()
    {
      this.SetDefaultParams();
      this.moveStartPosition = CacheTransform.position;
      this.timer = 0;
    }

    private void UpdateMove()
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

    private void ExitMove()
    {
      CacheTransform.position = this.moveEndPosition;
    }

    //-------------------------------------------------------------------------
    // 通常

    private void EnterUsual()
    {
      SetDefaultParams();
    }

    //-------------------------------------------------------------------------
    // 選択中

    private void EnterSelected()
    {
      SetDefaultParams();
      this.spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
      this.timer = 0;
    }

    private void UpdateSelected()
    {
      CacheTransform.localScale = Vector3.Lerp(
        Vector3.one, 
        Vector3.one * 0.9f, 
        Tween.EaseInSine(Mathf.Abs(Mathf.Sin(this.timer * Mathf.PI)))
      );

      this.timer += TimeManager.Instance.DeltaTime;
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
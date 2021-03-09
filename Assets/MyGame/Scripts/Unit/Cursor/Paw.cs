using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;
using System;

namespace MyGame.Unit.Cursor
{
  /// <summary>
  /// 肉球カーソルクラス
  /// </summary>
  public class Paw : Unit<Paw.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Operable, // 操作可能
    }

    /// <summary>
    /// パッドのタイプ
    /// </summary>
    public enum Type { 
      White,
      Black,
    }

    //-------------------------------------------------------------------------
    // Inspector設定項目

    /// <summary>
    /// カーソルの加速度
    /// </summary>
    [Range(0.1f, 2.0f)]
    public float _Acceleration = 0;

    /// <summary>
    /// カーソルが停止するまでの時間
    /// </summary>
    [Range(0.1f, 2.0f)]
    public float _SuspensionTime = 0;

    /// <summary>
    /// カーソルが移動できる範囲
    /// </summary>
    public Vector2 _MovableRange = new Vector2(1, 0.75f);

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// カーソルの速度ベクトル
    /// </summary>
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// 停止用タイマー
    /// </summary>
    private float suspensionTimer;

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Spriteリソース
    /// </summary>
    private Dictionary<Type, Sprite> sprites = new Dictionary<Type, Sprite>();

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// パッド番号
    /// </summary>
    public int PadNo { private get; set; }

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(Action pre, Action done)
    {
      var rm = ResourceManager.Instance;
      rm.Load<Sprite>("Cursor.Cat01.sprite", pre, done);
      rm.Load<Sprite>("Cursor.Cat02.sprite", pre, done);
    }

    public static void Unload()
    {
      var rm = ResourceManager.Instance;
      rm.Unload("Cursor.Cat01.sprite");
      rm.Unload("Cursor.Cat02.sprite");
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      this.spriteRenderer = GetComponent<SpriteRenderer>();
      this.state.Add(State.Idle);
      this.state.Add(State.Operable, null, OnOperableUpdate);
      this.state.SetState(State.Idle);

      this.sprites[Type.White] = ResourceManager.Instance.GetCache<Sprite>("Cursor.Cat01.sprite");
      this.sprites[Type.Black] = ResourceManager.Instance.GetCache<Sprite>("Cursor.Cat02.sprite");
    }

    protected override void OnMyDestory()
    {
      this.sprites.Clear();
    }

    //-------------------------------------------------------------------------
    // 公開メソッド

    /// <summary>
    /// 操作可能に
    /// </summary>
    public void ToOperable()
    {
      this.state.SetState(State.Operable);
    }

    /// <summary>
    /// カーソルの種類を設定
    /// </summary>
    public void SetType(Type type)
    {
      this.spriteRenderer.sprite = this.sprites[type];
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    /// <summary>
    /// 操作可能状態(Update)
    /// </summary>
    private void OnOperableUpdate()
    {
      var deltaTime = TimeManager.Instance.DeltaTime;
      var cmdMove   = InputManager.Instance.GetCommand(Command.Move, this.PadNo);

      // Moveコマンド中はカーソルを加速
      if (cmdMove.IsFixed) 
      {
        Vector3 v = cmdMove.Axis;
        this.velocity += v * _Acceleration * deltaTime;
        this.suspensionTimer = 0;
      } 
      
      // 非Moveコマンド中はカーソルを減速
      else 
      {
        var rate = Mathf.Min(1.0f, this.suspensionTimer / _SuspensionTime);
        this.velocity = Vector3.Lerp(this.velocity, Vector3.zero, rate * rate);
        this.suspensionTimer += deltaTime;
      }

      // 座標を更新
      CacheTransform.position += this.velocity * deltaTime;

      // 可動範囲を超えたら速度を反転、位置補正
      Vector3 pos = transform.position;

      if (Mathf.Abs(_MovableRange.x) <= Mathf.Abs(pos.x)) 
      {
        this.velocity.x *= -1f;
        pos.x = (pos.x < 0)? -_MovableRange.x : _MovableRange.x;
      }

      if (Mathf.Abs(_MovableRange.y) <= Mathf.Abs(pos.y)) {
        this.velocity.y *= -1f;
        pos.y = (pos.y < 0)? -_MovableRange.y : _MovableRange.y;
      }

      CacheTransform.position = pos;
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.InputManagement;

namespace MyGame.Unit.Cursor
{

  public class CatPaw : Unit<CatPaw.State>
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
    public float Acceleration = 0;

    /// <summary>
    /// カーソルが停止するまでの時間
    /// </summary>
    [Range(0.1f, 2.0f)]
    public float SuspensionTime = 0;

    /// <summary>
    /// カーソルが移動できる範囲
    /// </summary>
    public Vector2 MovableRange = new Vector2(1, 0.75f);

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

    /// <summary>
    /// パッド番号
    /// </summary>
    public int PadNo { private get; set; }

    //-------------------------------------------------------------------------
    // メソッド

    protected override void MyAwake()
    {
      this.spriteRenderer = GetComponent<SpriteRenderer>();
      this.state.Add(State.Idle, null, null, null);
      this.state.Add(State.Operable, null, MovableUpdate, null);
      this.state.SetState(State.Idle);

      this.Load();
    }

    private void Load()
    {
      var rm = ResourceManager.Instance;
      this.sprites[Type.White] = rm.GetCache<Sprite>("SpriteCursorCat01");
      this.sprites[Type.Black] = rm.GetCache<Sprite>("SpriteCursorCat02");
    }

    protected override void OnMyDestory()
    {
      this.sprites = null;
    }

    public void SetStateMovable()
    {
      this.state.SetState(State.Operable);
    }

    public void SetType(Type type)
    {
      this.spriteRenderer.sprite = this.sprites[type];
    }

    /// <summary>
    /// 操作可能状態(Update)
    /// </summary>
    private void MovableUpdate()
    {
      var deltaTime = TimeManager.Instance.DeltaTime;
      var cmdMove   = InputManager.Instance.GetCommand(Command.Move, this.PadNo);

      // Moveコマンド中はカーソルを加速
      if (cmdMove.IsFixed) 
      {
        Vector3 v = cmdMove.Axis;
        this.velocity += v * Acceleration * deltaTime;
        this.suspensionTimer = 0;
      } 
      
      // 非Moveコマンド中はカーソルを減速
      else 
      {
        var rate = Mathf.Min(1.0f, this.suspensionTimer / SuspensionTime);
        this.velocity = Vector3.Lerp(this.velocity, Vector3.zero, rate * rate);
        this.suspensionTimer += deltaTime;
      }

      // 座標を更新
      cacheTransform.position += this.velocity * deltaTime;

      // 可動範囲を超えたら速度を反転、位置補正
      Vector3 pos = transform.position;

      if (Mathf.Abs(MovableRange.x) <= Mathf.Abs(pos.x)) 
      {
        this.velocity.x *= -1f;
        pos.x = (pos.x < 0)? -MovableRange.x : MovableRange.x;
      }

      if (Mathf.Abs(MovableRange.y) <= Mathf.Abs(pos.y)) {
        this.velocity.y *= -1f;
        pos.y = (pos.y < 0)? -MovableRange.y : MovableRange.y;
      }

      cacheTransform.position = pos;
    }
  }
}
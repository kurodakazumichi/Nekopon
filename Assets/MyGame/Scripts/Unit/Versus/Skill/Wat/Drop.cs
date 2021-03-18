using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus.Skill
{
  /// <summary>
  /// 雫エフェクト
  /// </summary>
  public class Drop : Unit<Drop.State>, IPoolable
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Moving,
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
    /// 雫が消える際に呼ばれるコールバック
    /// </summary>
    private System.Action<Drop> OnFinish;

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雫のテクスチャ
    /// </summary>
    public static List<Sprite> Sprites = new List<Sprite>();

    /// <summary>
    /// 加算合成用マテリアル
    /// </summary>
    public static Material Material;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Wat.01.sprite", pre, done, (res) => { Sprites.Add(res); });
      rs.Load<Sprite>("Skill.Wat.02.sprite", pre, done, (res) => { Sprites.Add(res); });
      rs.Load<Material>("Common.Additive.material", pre, done, (res) => { Material = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Wat.01.sprite");
      rs.Unload("Skill.Wat.02.sprite");
      rs.Unload("Common.Additive.material");
      Sprites.Clear();
      Material = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // Component取得
      this.main = MyGameObject.Create<SpriteRenderer>("Main", CacheTransform);
      this.main.sortingLayerName = Define.Layer.Sorting.Effect;
      this.main.sortingOrder = Define.Layer.Order.Layer00;

      this.sub = MyGameObject.Create<SpriteRenderer>("Addtive", CacheTransform);
      this.sub.sortingLayerName = Define.Layer.Sorting.Effect;
      this.sub.sortingOrder = Define.Layer.Order.Layer00 + 1;
      this.sub.material = Material;

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Moving, OnMovingEnter, OnMovingUpdate);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // セットアップ

    public void Setup(System.Action<Drop> onFinish)
    {
      this.OnFinish = onFinish;
      int count = Sprites.Count - 1;
      this.main.sprite  = this.sub.sprite = Sprites[Random.Range(0, count)];
      this.sub.material = Material;
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    public void Fire(Vector3 start, Vector3 end, float time)
    {
      this.startPosition = start;
      this.endPosition   = end;
      this.time = time;
      this.state.SetState(State.Moving);
    }

    private void OnMovingEnter()
    {
      CacheTransform.position = startPosition;
      this.color.a = Random.Range(0, 1f);
      this.sub.color = this.color;
      this.timer = 0;
      this.bias = Random.Range(0, Mathf.PI);
    }

    private void OnMovingUpdate()
    {
      float rate = this.timer / this.time;

      CacheTransform.position = Vector3.Lerp(this.startPosition, this.endPosition, rate);

      this.color.a = Mathf.Abs(Mathf.Sin(this.bias + (rate * 5f)));
      this.sub.color = this.color;
      UpdateTimer();

      if (this.time < this.timer) {
        OnFinish?.Invoke(this);
        this.state.SetState(State.Idle);
      }
    }
  }

}
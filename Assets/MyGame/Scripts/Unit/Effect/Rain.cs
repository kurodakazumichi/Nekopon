using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Effect
{
  public class Rain : EffectBase<Rain.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State {
      Idle,
      Rain,
      Wait,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 雨が降り注ぐ時間
    /// </summary>
    private const float ACTIVE_TIME = 0.7f;

    /// <summary>
    /// 雫の落ちる時間(最小)
    /// </summary>
    private const float MIN_RAIN_TIME = 0.5f;

    /// <summary>
    /// 雫の落ちる時間(最大)
    /// </summary>
    private const float MAX_RAIN_TIME = 1f;

    /// <summary>
    /// 雫をランダム配置する際のばらつき幅(半分)
    /// </summary>
    private const float DROP_WIDTH = 0.3f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 雫ユニットプール
    /// </summary>
    private ObjectPool<Mover.Glow> pool = new ObjectPool<Mover.Glow>();

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雫のテクスチャ
    /// </summary>
    public static List<Sprite> Sprites = new List<Sprite>();

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Wat.01.sprite", pre, done, (res) => { Sprites.Add(res); });
      rs.Load<Sprite>("Skill.Wat.02.sprite", pre, done, (res) => { Sprites.Add(res); });
      Mover.Glow.Load(pre, done);
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Wat.01.sprite");
      rs.Unload("Skill.Wat.02.sprite");
      Mover.Glow.Unload();
      Sprites.Clear();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 雫ユニットのオブジェクトプール初期設定
      this.pool.SetGenerator(() => {
        return MyGameObject.Create<Mover.Glow>("Drop", CacheTransform);
      });

      // 50個予約しとく
      this.pool.Reserve(50);

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Rain, OnRainEnter, OnRainUpdate);
      this.state.Add(State.Wait, OnWaitEnter, OnWaitUpdate, OnWaitExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // IEffectの実装

    /// <summary>
    /// 雨を降らしている状態以外はIdle扱いにする
    /// </summary>
    public override bool IsIdle => (this.state.StateKey != State.Rain);

    /// <summary>
    /// 発動
    /// </summary>
    public override void Fire(Vector3 position)
    {
      base.Fire(position);
      this.state.SetState(State.Rain);
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    //-------------------------------------------------------------------------
    // Rain
    // 雨を降らす(Effect的にはこの状態以外の時はIdleとして扱う)

    private void OnRainEnter()
    {
      this.timer = 0;
    }

    private void OnRainUpdate()
    {
      CreateDrop();
      CreateDrop();

      UpdateTimer();

      if (ACTIVE_TIME < this.timer) {
        this.state.SetState(State.Wait);
      }
    }

    //-------------------------------------------------------------------------
    // Wait
    // 雨が降り終わるのを待って、終わったら返却する

    private void OnWaitEnter()
    {
      this.timer = 0;
    }

    private void OnWaitUpdate()
    {
      UpdateTimer();

      if (MAX_RAIN_TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnWaitExit()
    {
      EffectManager.Instance.Release(EffectManager.Type.Rain, this);
    }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 雨の雫を生成する
    /// </summary>
    private void CreateDrop()
    {
      // 雫を作る
      var drop = this.pool.Create();

      // セットアップ
      drop.Setup(
        Util.GetRandom(Sprites),
        Define.Layer.Sorting.Effect
      );

      // 加算合成具合を設定
      drop.SetFlash(Random.Range(0, 10f), 0f, 0.7f);

      // 移動後に呼ばれるコールバック設定
      drop.OnIdle = (unit) => {
        this.pool.Release(unit, CacheTransform);
      };

      // ToMoveに渡すパラメータを用意
      Vector3 start = CacheTransform.position;
      start.x += Random.Range(-DROP_WIDTH, DROP_WIDTH);

      Vector3 end = start;
      end.y = -1;

      float time = Random.Range(MIN_RAIN_TIME, MAX_RAIN_TIME);

      // 発動
      drop.ToMove(start, end, time);
    }
  }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Title
{
  /// <summary>
  /// タイトル画面で表示されるシャボン玉エフェクト
  /// </summary>
  public class Effect : Unit<Effect.State>
  {
    /// <summary>
    /// 状態(タイトルだけなので状態なしで作成)
    /// </summary>
    public enum State { }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 汎用タイマー
    /// </summary>
    private float timer = 0;

    /// <summary>
    /// 寿命、最初にランダムで設定し、寿命を迎えたら消える
    /// </summary>
    private float lifeTime = 0;

    /// <summary>
    /// 移動速度
    /// </summary>
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// 回転速度
    /// </summary>
    private float angularSpeed = 0;

    //-------------------------------------------------------------------------
    // ライフサイクル
    
    protected override void MyStart()
    {
      this.lifeTime = Random.Range(5, 10);
      this.SetRandom();
    }

    // Update is called once per frame
    protected override void MyUpdate()
    {
      // 寿命を迎えたら消える
      if (this.lifeTime <= this.timer) {
        Destroy(this.gameObject);
        return;
      }

      // timerが0になったらランダムに状態を変更
      if (this.timer <= 0) {
        SetRandom();
      }

      // 移動、回転、タイマーの更新
      float deltaTime = TimeSystem.Instance.DeltaTime;

      this.CacheTransform.position += this.velocity * deltaTime;
      this.CacheTransform.Rotate(0, 0, this.angularSpeed * deltaTime);

      this.timer    -= Time.deltaTime;
      this.lifeTime -= TimeSystem.Instance.DeltaTime;
    }

    private void SetRandom()
    {
      this.timer = Random.Range(1f, 4f);
      this.velocity.x = Random.Range(-0.1f, 0.1f);
      this.velocity.y = Random.Range(0, 0.5f);
      this.angularSpeed = Random.Range(-60f, 60f);
    }
  }
}
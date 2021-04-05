using UnityEngine;

namespace MyGame.Unit.Versus
{
  public partial class Paw
  {
    /// <summary>
    /// 相手プレイヤーと肉球を入れ替える際の入れ替え処理実行クラス
    /// </summary>
    public class SwapExecutor
    {
      //-------------------------------------------------------------------------
      // メンバ変数

      /// <summary>
      /// 開始座標
      /// </summary>
      private Vector3 startPosition = Vector3.zero;

      /// <summary>
      /// 目標座標
      /// </summary>
      private Vector3 targetPosition = Vector3.zero;

      /// <summary>
      /// 操作対象の肉球
      /// </summary>
      private Paw paw = null;

      /// <summary>
      /// タイマー
      /// </summary>
      private float timer = 0;
      
      /// <summary>
      /// 入れ替えに要する時間を設定する
      /// </summary>
      private float time = 0;

      /// <summary>
      /// 入れ替え中かどうか
      /// </summary>
      public bool IsActive { get; private set; }

      //-------------------------------------------------------------------------
      // メソッド

      /// <summary>
      /// コンストラクタ
      /// </summary>
      public SwapExecutor(Paw paw)
      {
        this.paw = paw;
      }

      /// <summary>
      /// 入れ替え開始
      /// </summary>
      public void Start(float targetX)
      {
        if (this.paw == null) {
          return;
        }

        // 入れ替えに要する時間
        const float MIN_TIME = 1f;
        const float MAX_TIME = 1.5f;
        this.time = Random.Range(MIN_TIME, MAX_TIME);

        // 入れ替え元、入れ替え先の座標を設定
        this.startPosition  = this.paw.CacheTransform.position;
        this.targetPosition = this.paw.CacheTransform.position;
        this.targetPosition.x = targetX;

        // 現在の肉球の開始位置、目標位置も入れ替え先の座標に更新
        this.paw.startPosition.x  = targetX;
        this.paw.targetPosition.x = targetX;

        // アクティブにする
        IsActive = true;
        this.timer = 0;
      }

      /// <summary>
      /// 更新
      /// </summary>
      public void Update()
      {
        if (!IsActive) {
          return;
        }

        // 経過時間から割合を算出
        var rate = this.timer / this.time;

        // 肉球の座標を更新
        this.paw.CacheTransform.position
          = MyVector3.Lerp(this.startPosition, this.targetPosition, Tween.EaseInOutBack(rate));

        // スキル時間で作動させる
        this.timer += TimeSystem.Instance.SkillDeltaTime;

        // 移動完了したらアクティブを解除
        if (this.time < this.timer) {
          this.paw.CacheTransform.position = this.targetPosition;
          this.IsActive = false;
        }
      }
    }
  }
}

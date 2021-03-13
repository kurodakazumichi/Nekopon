using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public partial class Paw : Unit<Paw.State>
  {
    /// <summary>
    /// 肉球ステータス(凍結や麻痺の状態)
    /// </summary>
    private class Status
    {
      //-------------------------------------------------------------------------
      // メンバ変数

      /// <summary>
      /// エフェクトの種類
      /// </summary>
      private PawEffectManager.Type type = default;

      /// <summary>
      /// 親オブジェクト
      /// </summary>
      public Transform parent = null;

      /// <summary>
      /// 肉球エフェクト
      /// </summary>
      private PawEffect effect = null;

      /// <summary>
      /// 状態異常の時間
      /// </summary>
      private float time = 0;
      
      /// <summary>
      /// タイマー
      /// </summary>
      private float timer = 0;

      //-------------------------------------------------------------------------
      // プロパティ

      /// <summary>
      /// エフェクトが有効な時はtrue
      /// </summary>
      public bool IsActive => (this.effect != null);

      //-------------------------------------------------------------------------
      // メソッド

      public Status(PawEffectManager.Type type, float time, Transform parent)
      {
        this.type = type;
        this.time = time;
        this.parent = parent;
      }

      /// <summary>
      /// Startを呼ぶと設定されている肉球エフェクトが有効になる
      /// </summary>
      public void Start()
      {
        this.timer = 0;
        
        if (this.effect == null) {
          this.effect = PawEffectManager.Instance.Create(this.type);
          this.effect.SetParent(this.parent);
          this.effect.CacheTransform.localPosition = Vector3.zero;
        }
      }

      /// <summary>
      /// Updateは常に呼び出す、肉球エフェクトの効果が切れると自動的に終了する
      /// </summary>
      public void Update()
      {
        if (this.effect == null) return;

        if (this.time < this.timer) {
          Finish();
          return;
        }
          
        this.timer += TimeSystem.Instance.DeltaTime;
      }

      /// <summary>
      /// Finishを呼び出すと肉球エフェクトが無効になる
      /// </summary>
      public void Finish()
      {
        PawEffectManager.Instance.Release(this.effect);
        this.effect = null;
      }

    }
  }
}
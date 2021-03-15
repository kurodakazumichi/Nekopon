using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// 攻撃オブジェクトを光らせるための加算合成用クラス
  /// </summary>
  public class InnerAttack: Unit<InnerAttack.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State {}

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 最大のアルファ値
    /// </summary>
    private const float MAX_ALPHA = 0.1f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    /// <summary>
    /// タイマー
    /// </summary>
    private float timer = 0;

    /// <summary>
    /// 色
    /// </summary>
    private Color color = Color.white;

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      this.spriteRenderer = AddComponent<SpriteRenderer>();
      this.spriteRenderer.sortingLayerName = Define.Layer.Sorting.Effect;
      this.spriteRenderer.sortingOrder     = Define.Layer.Order.Layer10;
    }

    protected override void MyUpdate()
    {
      // 輝き変化の間隔
      const float TIME = Define.Versus.ATTACK_BRIGHTNESS_INTERVAL;

      // 経過時間よりアルファ値を決定
      float alpha = MAX_ALPHA * Mathf.Abs(Mathf.Sin(this.timer / TIME));

      // アルファを設定
      this.color.a = alpha;
      this.spriteRenderer.color = color;

      // 時間を加算
      this.timer += TimeSystem.Instance.DeltaTime;
    }

    //-------------------------------------------------------------------------
    // 各種セッター

    /// <summary>
    /// Spriteをセット
    /// </summary>
    public InnerAttack SetSprite(Sprite sprite)
    {
      this.spriteRenderer.sprite = sprite;
      return this;
    }

    /// <summary>
    /// マテリアルをセット
    /// </summary>
    public InnerAttack SetMaterial(Material material)
    {
      this.spriteRenderer.material = material;
      return this;
    }

    /// <summary>
    /// スケールをセット
    /// </summary>
    public InnerAttack SetScale(float rate)
    {
      CacheTransform.localScale = Vector3.one * rate;
      return this;
    }
  }
}
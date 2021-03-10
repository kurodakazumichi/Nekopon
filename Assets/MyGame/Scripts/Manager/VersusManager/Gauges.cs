using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Versus;
namespace MyGame.VersusManagement
{
  /// <summary>
  /// ゲージ取りまとめようクラス
  /// </summary>
  public class Gauges
  {
    //-------------------------------------------------------------------------
    // クラス

    /// <summary>
    /// Gauges生成時に必要なパラメータ
    /// </summary>
    public class Props
    {
      /// <summary>
      /// Playerの親に該当するオブジェクト
      /// </summary>
      public Transform parent;

      /// <summary>
      /// 各種画面の配置情報
      /// </summary>
      public Location location;
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// Props
    /// </summary>
    private Props props;

    /// <summary>
    /// ゲージオブジェクトを格納しておくゲームオブジェクト
    /// </summary>
    private Transform folder = null;

    /// <summary>
    /// HPゲージ
    /// </summary>
    private Gauge hp = null;

    /// <summary>
    /// ダメージゲージ
    /// </summary>
    private Gauge dp = null;

    /// <summary>
    /// APゲージ
    /// </summary>
    private Gauge ap = null;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// HPゲージ setter
    /// </summary>
    public float Hp {
      set { this.hp.Rate = value; }
    }

    /// <summary>
    /// DPゲージ setter
    /// </summary>
    public float Dp {
      set { this.dp.Rate = value; }
    }

    /// <summary>
    /// APゲージ setter
    /// </summary>
    public float Ap {
      set { this.ap.Rate = value; }
    }

    //-------------------------------------------------------------------------
    // Load, Unload

    private static GameObject GaugePrefab = null;

    public static void Load(System.Action pre, System.Action done)
    {
      ResourceManager.Instance.Load<GameObject>("VS.Gauge.prefab", pre, done, (res) => {
        GaugePrefab = res;
      });
      Gauge.Load(pre, done);
    }

    public static void Unload()
    {
      Gauge.Unload();
    }

    //-------------------------------------------------------------------------
    // 生成・準備系

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Gauges(Props props)
    {
      this.props = props;
    }

    /// <summary>
    /// 各種オブジェクトの生成、初期化
    /// </summary>
    public Gauges Init()
    {
      // プレースフォルダーを作成
      this.folder = new GameObject("Gauges").transform;
      this.folder.parent = this.props.parent;

      // ゲージを生成
      this.hp = Object.Instantiate(GaugePrefab, this.folder).GetComponent<Gauge>();
      this.dp = Object.Instantiate(GaugePrefab, this.folder).GetComponent<Gauge>();
      this.ap = Object.Instantiate(GaugePrefab, this.folder).GetComponent<Gauge>();

      // 位置設定
      this.hp.CacheTransform.position = this.props.location.HpGuage;
      this.dp.CacheTransform.position = this.props.location.HpGuage;
      this.ap.CacheTransform.position = this.props.location.ApGuage;

      // 色設定
      this.hp.ToHpColor();
      this.dp.ToDpColor();
      this.ap.ToApColor();

      return this;
    }

    /// <summary>
    /// セットアップ
    /// </summary>
    public void Setup(float hp, float dp, float ap)
    {
      Hp = hp;
      Dp = dp;
      Ap = ap;
    }

  }
}
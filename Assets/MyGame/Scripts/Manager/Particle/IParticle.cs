using MyGame.Unit.Particle;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// パーティクルのインターフェース
  /// </summary>
  public interface IParticle : IPoolable
  {
    //-------------------------------------------------------------------------
    // メソッド

    /// <summary>
    /// 標準セットアップ
    /// </summary>
    void Setup(ParticleManager.Type type);

    /// <summary>
    /// Propsを要するセットアップ
    /// </summary>
    void Setup(Props props);

    /// <summary>
    /// 軌跡の設定
    /// </summary>
    void SetTrace(Props props, float time = 0);

    /// <summary>
    /// バウンドの設定
    /// </summary>
    void SetBound(bool enabled, float elasticity);

    /// <summary>
    /// 発動
    /// </summary>
    void Fire(Vector3 position, Vector3? scale = null, Quaternion? rotation = null);

    //-------------------------------------------------------------------------
    // プロパティ系

    /// <summary>
    /// パーティクルのタイプ
    /// </summary>
    ParticleManager.Type Type { get; }

    /// <summary>
    /// Transform
    /// </summary>
    Transform CacheTransform { get; }

    /// <summary>
    /// スプライト
    /// </summary>
    Sprite Sprite { set; }

    /// <summary>
    /// Mainスプライト
    /// </summary>
    Sprite MainSprite { get; set; }

    /// <summary>
    /// Glowスプライト
    /// </summary>
    Sprite GlowSprite { get; set; }

    /// <summary>
    /// 描画レイヤー名
    /// </summary>
    string LayerName { set; }

    /// <summary>
    /// アルファ
    /// </summary>
    float Alpha { set; }

    /// <summary>
    /// 輝度
    /// </summary>
    float Brightness { set; }

    /// <summary>
    /// 速度
    /// </summary>
    Vector3 Velocity { set; }

    /// <summary>
    /// スケーリング速度
    /// </summary>
    float ScaleAcceleration { set; }

    /// <summary>
    /// 回転速度
    /// </summary>
    float RotationAcceleration { set; }

    /// <summary>
    /// 透過速度
    /// </summary>
    float AlphaAcceleration { set; }

    /// <summary>
    /// 寿命
    /// </summary>
    float LifeTime { set; }

    /// <summary>
    /// 自滅するかどうかのフラグ
    /// </summary>
    bool IsSelfDestructive { set; }

    /// <summary>
    /// 軌跡生成時間
    /// </summary>
    float TraceTime { set; }
  }
}
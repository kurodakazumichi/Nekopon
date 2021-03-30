using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Particle
{
  /// <summary>
  /// パーティクルを生成するのに必要なパラメータ
  /// </summary>
  public class Props
  {
    //-------------------------------------------------------------------------
    // SpriteRenderer系

    public Sprite Sprite = null;
    public Material MainMaterial = null;
    public Material GlowMaterial = null;
    public float Alpha = 1f;
    public float Brightness = 0f;
    public string LayerName = Define.Layer.Sorting.Effect;
    public bool MainIsEnabled = true;
    public bool GlowIsEnabled = true;

    //-------------------------------------------------------------------------
    // 操作系

    public Vector3 Velocity;
    public float Gravity = 0f;
    public float AlphaAcceleration = 0f;
    public float LifeTime = 1f;
    public float RotationAcceleration = 0f;
    public float ScaleAcceleration = 0f;

    /// <summary>
    /// 自分自身で勝手に破棄するかどうか
    /// </summary>
    public bool IsSelfDestructive = true;

    public void Copy(Props props)
    {
      if (props == null) {
        return;
      }

      Sprite               = props.Sprite;
      MainMaterial         = props.MainMaterial;
      GlowMaterial         = props.GlowMaterial;
      Alpha                = props.Alpha;
      Brightness           = props.Brightness;
      LayerName            = props.LayerName;
      MainIsEnabled        = props.MainIsEnabled;
      GlowIsEnabled        = props.GlowIsEnabled;
      Velocity             = props.Velocity;
      Gravity              = props.Gravity;
      AlphaAcceleration    = props.AlphaAcceleration;
      LifeTime             = props.LifeTime;
      RotationAcceleration = props.RotationAcceleration;
      ScaleAcceleration    = props.ScaleAcceleration;
      IsSelfDestructive    = props.IsSelfDestructive;
    }

    public Props Clone()
    {
      var props = new Props()
      {
        Sprite               = this.Sprite,
        LayerName            = this.LayerName,
        MainMaterial         = this.MainMaterial,
        GlowMaterial         = this.GlowMaterial,
        MainIsEnabled        = this.MainIsEnabled,
        GlowIsEnabled        = this.GlowIsEnabled,
        Alpha                = this.Alpha,
        Brightness           = this.Brightness,
        Velocity             = this.Velocity,
        Gravity              = this.Gravity,
        ScaleAcceleration    = this.ScaleAcceleration,
        RotationAcceleration = this.RotationAcceleration,
        AlphaAcceleration    = this.AlphaAcceleration,
        LifeTime             = this.LifeTime,
        IsSelfDestructive    = this.IsSelfDestructive,
      };
      return props;
    }
  };
}
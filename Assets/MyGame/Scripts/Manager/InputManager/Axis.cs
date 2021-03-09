using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyGame.Define;

namespace MyGame.Define
{
  /// <summary>
  /// ゲームパッドのスティック(軸)に該当するクラス
  /// </summary>
  public class Axis
  {
    /// <summary>
    /// 軸の種類
    /// </summary>
    private AxisType type;

    /// <summary>
    /// Input.GetAxisで入力を取得する際に指定する軸の名称(Joy1_Axis1など) 
    /// </summary>
    private string name = "";

    /// <summary>
    /// Y軸などは入力値の符号を反転させたいケースがあるので、そのためのフラグ
    /// </summary>
    private bool invert = false;

    /// <summary>
    /// 初回入力時のみtrueになる
    /// </summary>
    public bool IsDown = false;

    /// <summary>
    /// 入力し続けた状態で、入力がなくなったタイミングでtrueになる
    /// </summary>
    public bool IsUp = false;

    /// <summary>
    /// 入力がある間、常にtrue
    /// </summary>
    public bool IsHold {
      get { return (0 < Time); }
    }

    /// <summary>
    /// 軸の入力値-1 ～ 0の値をとる
    /// </summary>
    public float Value = 0;

    /// <summary>
    /// 継続して入力されている時間(秒)を保持する
    /// </summary>
    public float Time = 0;

    /// <summary>
    /// コンストラクタでは実際のゲームパッドの軸との関連付けをする
    /// </summary>
    public Axis(AxisType type, string name, bool invert)
    {
      Setup(type, name, invert);
    }

    /// <summary>
    /// 軸の設定を行う、この設定によりどの軸からの入力を受け取るか決まる。
    /// </summary>
    public void Setup(AxisType type, string name, bool invert)
    {
      this.type = type;
      this.name = name;
      this.invert = invert;
    }

    /// <summary>
    /// 軸の入力状態から各種パラメータの状態を更新する
    /// </summary>
    public void Update()
    {
      // 軸の入力を受け取る
      var value = Input.GetAxis(this.name);
      Value = (invert) ? -value : value;

      // 押された瞬間、離された瞬間、入力し続けている時間などを更新
      IsDown = IsUp = false;

      // 入力がある
      if (0 < Mathf.Abs(value)) {
        if (Time <= 0) {
          IsDown = true;
        }

        Time += UnityEngine.Time.deltaTime;
      }

      // 入力がない
      else {
        if (0 < Time) {
          IsUp = true;
        }
        Time = 0;
      }
    }

#if _DEBUG
    public void OnDebug()
    {
      using (new GUILayout.HorizontalScope()) {
        GUILayout.Label(Enum.GetName(typeof(AxisType), this.type));
        GUILayout.Label($"Value:{MyMath.Round(Value, 3)}");
        GUILayout.Label($"Down:{IsDown}");
        GUILayout.Label($"Up:{IsUp}");
        GUILayout.Label($"Hold:{IsHold}");
        GUILayout.Label($"Time:{MyMath.Round(Time, 3)}");
      }
    }
#endif
  }

}
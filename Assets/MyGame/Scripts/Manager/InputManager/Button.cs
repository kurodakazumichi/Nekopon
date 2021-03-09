using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame.Define
{
  /// <summary>
  /// ゲームパッドのボタンに該当するクラス
  /// </summary>
  public class Button
  {
    /// <summary>
    /// ボタンの種類
    /// </summary>
    private ButtonType type;

    /// <summary>
    /// Input.GetButtonで入力を取得する際に指定するボタンの名称(Joy1_Button0など)
    /// </summary>
    private string name;

    /// <summary>
    /// 初回入力時のみtrueになる
    /// </summary>
    public bool IsDown = false;

    /// <summary>
    /// 入力しつづけた状態で、入力がなくなったタイミングでtrueになる
    /// </summary>
    public bool IsUp = false;

    /// <summary>
    /// 入力がある間、常にtrue
    /// </summary>
    public bool IsHold {
      get { return (0 < Time); }
    }

    /// <summary>
    /// 継続して入力されている時間(秒)を保持する
    /// </summary>
    public float Time = 0;

    /// <summary>
    /// コンストラクタで実際のボタンとの関連付けを行う
    /// </summary>
    public Button(ButtonType type, string name)
    {
      Setup(type, name);
    }

    /// <summary>
    /// ボタンの設定を行う、この設定によりどの軸からの入力を受け取るか決まる。
    /// </summary>
    public void Setup(ButtonType type, string name)
    {
      this.type = type;
      this.name = name;
    }

    /// <summary>
    /// ボタンの入力状態から各種パラメータの状態を更新する
    /// </summary>
    public void Update()
    {
      bool value = Input.GetButton(this.name);
      IsDown = IsUp = false;

      // 入力があった場合
      if (value) {
        if (Time <= 0) IsDown = true;
        Time += UnityEngine.Time.deltaTime;
      }

      // 入力がない場合
      else {
        if (0 < Time) IsUp = true;
        Time = 0;
      }
    }

#if _DEBUG
    public void OnDebug()
    {
      using (new GUILayout.HorizontalScope()) {
        GUILayout.Label(Enum.GetName(typeof(ButtonType), this.type));
        GUILayout.Label($"Down:{IsDown}");
        GUILayout.Label($"Up:{IsUp}");
        GUILayout.Label($"IsHold:{IsHold}");
        GUILayout.Label($"Time:{MyMath.Round(Time, 3)}");
      }
    }
#endif
  }

}
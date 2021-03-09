using System;
using UnityEngine;
using MyGame.Define;

namespace MyGame.InputManagement
{
  /// <summary>
  /// キーボードのキーに該当するクラス
  /// </summary>
  public class Key
  {
    /// <summary>
    /// キーの種類
    /// </summary>
    private KeyType type;

    /// <summary>
    /// 対応するキーボードのKeyCode
    /// </summary>
    private KeyCode code;

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
    /// 何かしら入力があるかどうか
    /// </summary>
    public bool HasInput {
      get {
        return (IsUp || IsHold);
      }
    }

    /// <summary>
    /// 継続して入力されている時間(秒)を保持する
    /// </summary>
    public float Time = 0;

    /// <summary>
    /// コンストラクタではキーボードとの関連付けをする
    /// </summary>
    public Key(KeyType type, KeyCode code)
    {
      Setup(type, code);
    }

    /// <summary>
    /// 軸の設定を行う、この設定によりどの軸からの入力を受け取るか決まる。
    /// </summary>
    public void Setup(KeyType type, KeyCode code)
    {
      this.type = type;
      this.code = code;
    }

    /// <summary>
    /// 軸の入力状態から各種パラメータの状態を更新する
    /// </summary>
    public void Update()
    {
      // キーコードが設定されてないなら何もしない
      if (this.code == KeyCode.None) return;

      // 軸の入力を受け取る
      var value = Input.GetKey(this.code);

      IsDown = IsUp = false;

      // 入力がある
      if (value) {
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
        GUILayout.Label(Enum.GetName(typeof(KeyType), this.type));
        GUILayout.Label($"Down:{IsDown}");
        GUILayout.Label($"Up:{IsUp}");
        GUILayout.Label($"Hold:{IsHold}");
        GUILayout.Label($"Time:{MyMath.Round(Time, 3)}");
      }
    }
#endif
  }
}
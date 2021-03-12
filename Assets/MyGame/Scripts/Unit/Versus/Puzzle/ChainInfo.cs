using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// 消滅情報
  /// </summary>
  public class ChainInfo
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 連鎖数
    /// </summary>
    public int ChainCount { get; private set; } = 0;

    /// <summary>
    /// 消滅した数(属性ごとに記録)
    /// </summary>
    private List<int> vanishCounts = new List<int>();

    /// <summary>
    /// 消滅した肉球の総数
    /// </summary>
    public int TotalVanishCount {
      get {
        int sum = 0;
        this.vanishCounts.ForEach((count) => { sum += count; });
        return sum;
      }
    }

    //-------------------------------------------------------------------------
    // publicメソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ChainInfo()
    {
      MyEnum.ForEach<Define.App.Attribute>((attribute) => {
        this.vanishCounts.Add(0);
      });
    }

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset()
    {
      // 連鎖、消失に関する情報をリセット
      ChainCount = 0;

      // 消滅した肉球の数
      MyEnum.ForEach<Define.App.Attribute>((attribute) => {
        this.vanishCounts[(int)attribute] = 0;
      });
    }

    /// <summary>
    /// 連鎖数を更新
    /// </summary>
    public void UpdateChainCount()
    {
      ChainCount++;
    }

    /// <summary>
    /// 消滅情報の更新
    /// </summary>
    public void UpdateVanish(Define.App.Attribute attribute, int count)
    {
      // 属性ごとに消えた数を保持
      this.vanishCounts[(int)attribute] += count;
    }

    /// <summary>
    /// 消滅数を取得
    /// </summary>
    public int GetVanishCount(Define.App.Attribute attribute)
    {
      return this.vanishCounts[(int)attribute];
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ
    public void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) 
      {
        GUILayout.Label($"連鎖数:{ChainCount}");

        using (new GUILayout.HorizontalScope()) {
          MyEnum.ForEach<Define.App.Attribute>((attribute) => {
            GUILayout.Label($"{attribute}:{this.vanishCounts[(int)attribute]}");
          });
        }

        GUILayout.Label($"総数:{TotalVanishCount}");
      }
    }
#endif
  }
}
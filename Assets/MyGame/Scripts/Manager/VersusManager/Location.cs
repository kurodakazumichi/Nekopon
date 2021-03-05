using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.VersusManagement
{
  /// <summary>
  /// 対戦画面内の各種ロケーション(座標)を格納したクラス
  /// </summary>
  public struct Location
  {
    public Vector3 Paw { get; private set; }

    public Location(string prefix, GameObject go)
    {
      Transform locations = go.transform;
      Paw = locations.Find($"{prefix}.Paw").position;
    }
  }
}


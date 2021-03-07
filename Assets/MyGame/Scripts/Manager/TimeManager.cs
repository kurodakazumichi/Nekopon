using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// ゲーム内時間を管理するクラス
  /// </summary>
  public class TimeManager : SingletonMonobehaviour<TimeManager>
  {
    [SerializeField]
    private float timeScale = 1f;

    public float TimeScale {
      get { return this.timeScale; }
      set { this.timeScale = Mathf.Max(0, value); }
    }

    public float DeltaTime {
      get { return this.timeScale * Time.deltaTime; }
    }
    
    public long Ticks => (System.DateTime.Now.Ticks);
  }
}
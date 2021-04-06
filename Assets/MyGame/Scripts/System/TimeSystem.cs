using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// ゲーム内時間を管理するクラス
  /// </summary>
  public class TimeSystem : SingletonMonoBehaviour<TimeSystem>
  {
    [SerializeField]
    private float timeScale = 1f;

    private float skillTimeScale = 1f;

    public float SkillTimeScale {
      get { return this.skillTimeScale; }
      set { this.skillTimeScale = Mathf.Max(0, value); }
    }

    public bool Pause {
      set {
        this.timeScale = value? 0f : 1f;
      }
    }

    public float DeltaTime {
      get { return this.timeScale * Time.deltaTime; }
    }

    public float SkillDeltaTime {
      get { return this.skillTimeScale * Time.deltaTime; }
    }

    public long Ticks => (System.DateTime.Now.Ticks);
  }
}
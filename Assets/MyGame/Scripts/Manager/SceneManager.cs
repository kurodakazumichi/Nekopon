using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Origin = UnityEngine.SceneManagement;

namespace MyGame
{
  public class SceneManager : SingletonMonoBehaviour<SceneManager>
  {
    public enum SceneType
    {
      None = -1,
      Boot,
      Title,
      Versus,
    }

    /// <summary>
    /// 予約シーン
    /// </summary>
    public SceneType ReservedScene { private get; set; } = SceneType.None;

    /// <summary>
    /// 予約されているシーンがあるかどうか
    /// </summary>
    public bool HasReservedScene => (ReservedScene != SceneType.None);

    public void LoadSceneAdditive(SceneType type)
    {
      if (type == SceneType.None) return;

      Origin.SceneManager.sceneLoaded += LoadedCallback;

      Origin.SceneManager
        .LoadScene(this.getSceneNameOfType(type), Origin.LoadSceneMode.Additive);
    }

    private void LoadedCallback(Origin.Scene next, Origin.LoadSceneMode mode)
    {
      Debug.Logger.Log(VersusManager.Instance);


      Origin.SceneManager.sceneLoaded -= LoadedCallback;
    }

    public void UnloadSceneAsync(SceneType type, Action completed)
    {
      Origin.SceneManager.UnloadSceneAsync(this.getSceneNameOfType(type))
        .completed += (op) => { completed?.Invoke(); };
    }

    public void LoadReservedSceneAdditive()
    {
      if (ReservedScene == SceneType.None) return;

      Origin.SceneManager
        .LoadScene(this.getSceneNameOfType(ReservedScene), Origin.LoadSceneMode.Additive);
    }
    

    private string getSceneNameOfType(SceneType type)
    {
      switch(type) {
        case SceneType.Boot : return "Boot";
        case SceneType.Versus: return "Versus";
        default: return "Title";
      }
    }
  }

}

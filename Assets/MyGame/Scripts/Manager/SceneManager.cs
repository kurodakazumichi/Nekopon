using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class SceneManager : SingletonMonobehaviour<SceneManager>
  {
    public enum SceneType
    {
      Title,
    }

    public void LoadSceneAdditive(SceneType type)
    {
      UnityEngine.SceneManagement.SceneManager
        .LoadScene(this.getSceneNameOfType(type), UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    private string getSceneNameOfType(SceneType type)
    {
      switch(type) {
        default: return "Title";
      }
    }
  }

}

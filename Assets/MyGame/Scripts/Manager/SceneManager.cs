using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Origin = UnityEngine.SceneManagement;

namespace MyGame
{
  /// <summary>
  /// シーン遷移や合成を司るクラス
  /// このゲームではBootシーンから始まり、原則として他のシーンは加算合成を前提とする。
  /// </summary>
  public class SceneManager : SingletonMonoBehaviour<SceneManager>
  {
    /// <summary>
    /// シーンの種類
    /// </summary>
    public enum SceneType
    {
      None = -1,
      Title,
      Versus,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 予約シーン
    /// </summary>
    public SceneType ReservedScene { private get; set; } = SceneType.None;

    /// <summary>
    /// 予約されているシーンがあるかどうか
    /// </summary>
    public bool HasReservedScene => (ReservedScene != SceneType.None);

    /// <summary>
    /// ロード中のカウント
    /// </summary>
    private int LoadingCount = 0;

    /// <summary>
    /// アンロード中のカウント
    /// </summary>
    private int UnloadingCount = 0;

    //-------------------------------------------------------------------------
    // プロパティ(導出)

    /// <summary>
    /// ローディング中
    /// </summary>
    public bool IsLoading => (0 < LoadingCount);

    /// <summary>
    /// アンロード中
    /// </summary>
    public bool IsUnloading => (0 < UnloadingCount);

    /// <summary>
    /// 忙しい
    /// </summary>
    public bool IsBusy => (IsLoading || IsUnloading);

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyStart()
    {
      Debug.Manager.Instance.Regist(this);
    }

    protected override void OnMyDestory()
    {
      Debug.Manager.Instance.Discard(this);
    }

    //-------------------------------------------------------------------------
    // メソッド

    /// <summary>
    /// 指定したシーンを加算合成でロード
    /// </summary>
    public void LoadSceneAdditive(SceneType type)
    {
      if (type == SceneType.None) return;

      Origin.SceneManager.sceneLoaded += OnLoaded;

      Origin.SceneManager
        .LoadScene(this.getSceneNameOfType(type), Origin.LoadSceneMode.Additive);

      LoadingCount++;
    }

    /// <summary>
    /// 予約シーンを加算合成でロード
    /// </summary>
    public void LoadReservedSceneAdditive()
    {
      LoadSceneAdditive(ReservedScene);
    }


    /// <summary>
    /// シーンロード後に実行されるコールバック
    /// </summary>
    /// <param name="next"></param>
    /// <param name="mode"></param>
    private void OnLoaded(Origin.Scene next, Origin.LoadSceneMode mode)
    {
      Origin.SceneManager.sceneLoaded -= OnLoaded;
      LoadingCount--;
    }

    /// <summary>
    /// 非同期シーンアンロード
    /// </summary>
    public void UnloadSceneAsync(SceneType type, Action completed)
    {
      UnloadingCount++;

      Origin.SceneManager.UnloadSceneAsync(this.getSceneNameOfType(type))
        .completed += (op) => { 
          completed?.Invoke(); 
          UnloadingCount--;
        };
    }

    /// <summary>
    /// シーンタイプからシーン名を取得
    /// </summary>
    private string getSceneNameOfType(SceneType type)
    {
      switch(type) {
        case SceneType.Versus: return "Versus";
        default: return "Title";
      }
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    private string __SceneName = "";

    public override void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) {

        GUILayout.Label($"IsBusy:{IsBusy}, IsLoading:{IsLoading}, IsUnloading:{IsUnloading}");
        GUILayout.Label($"Loading:{LoadingCount}, Unloading:{UnloadingCount}");

        using (new GUILayout.HorizontalScope()) 
        {
          __SceneName = GUILayout.TextField(__SceneName);

          if (GUILayout.Button("Load")) {
            if (MyEnum.TryParse(__SceneName, out SceneType type)) {
              LoadSceneAdditive(type);
            }
          }

          if (GUILayout.Button("Unload")) {
            if (MyEnum.TryParse(__SceneName, out SceneType type)) {
              UnloadSceneAsync(type, null);
            }
          }
        }
      }
    }
#endif

  }

}

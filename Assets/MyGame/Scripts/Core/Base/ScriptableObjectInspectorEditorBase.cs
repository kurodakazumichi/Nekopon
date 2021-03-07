using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace MyGame
{
  /// <summary>
  /// ScriptableObjectのInspector拡張をする際に継承するベースクラス
  /// </summary>
  public class ScriptableObjectInspectorEditorBase<T> : Editor where T : ScriptableObject
  {
    /// <summary>
    /// Inspectorで操作する対称
    /// </summary>
    protected T config;

    /// <summary>
    /// Inspectorが表示される際に実行される
    /// </summary>
    private void Awake()
    {
      this.config = target as T;
    }

    /// <summary>
    /// 保存ボタン
    /// </summary>
    protected void SaveButton()
    {
      if (GUILayout.Button("Save")) {
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
      }
    }
  }
}

#endif
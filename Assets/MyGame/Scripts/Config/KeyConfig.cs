using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyGame.Define;

namespace MyGame
{
  /// <summary>
  /// ゲーム内の仮想KeyとUnityのKeyCodeとのマッピング
  /// </summary>
  [CreateAssetMenu(menuName = "MyGame/Create/KeyConfig")]
  public class KeyConfig : ScriptableObject
  {
    //-------------------------------------------------------------------------
    // クラス

    [System.Serializable]
    private class KeyMap
    {
      public KeyType Type;
      public KeyCode Code = default;

      public KeyMap(KeyType type)
      {
        Type = type;
      }
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// KeyTypeとKeyCodeのマッピングデータ
    /// </summary>
    [SerializeField]
    private List<KeyMap> keyMap = new List<KeyMap>();

    //-------------------------------------------------------------------------
    // ライフサイクル

    /// <summary>
    /// 右クリックメニューから生成されたときに一度動作する
    /// </summary>
    private void Awake()
    {
      MyEnum.ForEach<KeyType>((type) => { 
        this.keyMap.Add(new KeyMap(type));
      });
    }

    //-------------------------------------------------------------------------
    // publicメソッド

    /// <summary>
    /// KeyMapの要素をコールバックを指定して取得する
    /// </summary>
    public void Gets(System.Action<KeyType, KeyCode> func)
    {
      this.keyMap.ForEach((map) => { 
        func(map.Type, map.Code);
      });
    }

#if _DEBUG

    //-------------------------------------------------------------------------
    // デバッグ

    public void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) 
      {
        this.keyMap.ForEach((map) => 
        {
          GUILayout.Label($"{map.Type}:{map.Code}");
        });
      }
    }

#endif

#if UNITY_EDITOR
    //---------------------------------------------------------------------------
    // Inspector拡張

    /// <summary>
    /// KeyConfigのInspector拡張
    /// </summary>
    [CustomEditor(typeof(KeyConfig))]
    public class KeyConfigEditor : ScriptableObjectInspectorEditorBase<KeyConfig>
    {
      protected override void OnMyInspectorGUI()
      {
        MyEnum.ForEach<KeyType>((type, index) => 
        {
          using (new EditorGUILayout.HorizontalScope()) 
          {
            EditorGUILayout.LabelField($"{type}");
            config.keyMap[index].Code = (KeyCode)EditorGUILayout.EnumPopup(config.keyMap[index].Code);
          }
        });
      }
    }
#endif
  }
}
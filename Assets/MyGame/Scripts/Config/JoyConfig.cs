using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;
using UnityEditor;

namespace MyGame
{
  /// <summary>
  /// ゲーム内の仮想Axis・ButtonとUnityのInputManagerで定義した設定とのマッピング
  /// </summary>
  [CreateAssetMenu(menuName = "MyGame/Create/JoyConfig")]
  public class JoyConfig : ScriptableObject
  {
    //-------------------------------------------------------------------------
    // クラス

    [System.Serializable]
    private class AxisMap
    {
      public AxisType Type = default;
      public int No = -1;
    }

    [System.Serializable]
    private class ButtonMap
    {
      public ButtonType Type = default;
      public int No = -1;
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// Axisの設定
    /// </summary>
    [SerializeField]
    private List<AxisMap> axisMap = new List<AxisMap>();

    /// <summary>
    /// Buttonの設定
    /// </summary>
    [SerializeField]
    private List<ButtonMap> buttonMap = new List<ButtonMap>();

    //-------------------------------------------------------------------------
    // ライフサイクル

    /// <summary>
    /// 右クリックメニューから生成されたときに一度動作する
    /// </summary>
    private void Awake()
    {
      MyEnum.ForEach<AxisType>((_) => {
        this.axisMap.Add(new AxisMap());
      });

      MyEnum.ForEach<ButtonType>((_) => { 
        this.buttonMap.Add(new ButtonMap());
      });
    }

    //-------------------------------------------------------------------------
    // publicメソッド

    /// <summary>
    /// axisMapの要素をコールバックを指定して取得する
    /// </summary>
    public void GetAxes(System.Action<AxisType, int> func)
    {
      this.axisMap.ForEach((map) => {
        func(map.Type, map.No);
      });
    }

    /// <summary>
    /// buttonMapの要素をコールバックを指定して取得する
    /// </summary>
    /// <param name="func"></param>
    public void GetButtons(System.Action<ButtonType, int> func)
    {
      this.buttonMap.ForEach((map) => { 
        func(map.Type, map.No);
      });
    }

#if _DEBUG

    //-------------------------------------------------------------------------
    // デバッグ

    public void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) {
        this.axisMap.ForEach((map) => {
          GUILayout.Label($"{map.Type}:axis {map.No}");
        });
        this.buttonMap.ForEach((map) => { 
          GUILayout.Label($"{map.Type}:button {map.No}");
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
    [CustomEditor(typeof(JoyConfig))]
    public class KeyConfigEditor : ScriptableObjectInspectorEditorBase<JoyConfig>
    {
      protected override void OnMyInspectorGUI()
      {
        // Axisの設定
        EditorGUILayout.LabelField("Axes");
        MyEnum.ForEach<AxisType>((type, index) => {
          using (new EditorGUILayout.HorizontalScope()) {
            EditorGUILayout.LabelField($"{type}");
            config.axisMap[index].No =EditorGUILayout.IntField(config.axisMap[index].No);
          }
        });

        EditorGUILayout.Separator();

        // Buttonの設定
        EditorGUILayout.LabelField("Buttons");
        MyEnum.ForEach<ButtonType>((type, index) => {
          using (new EditorGUILayout.HorizontalScope()) {
            EditorGUILayout.LabelField($"{type}");
            config.buttonMap[index].No = EditorGUILayout.IntField(config.buttonMap[index].No);
          }
        });
      }
    }
#endif

  }
}
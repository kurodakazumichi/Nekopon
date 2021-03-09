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
      public bool Invert = false;

      public AxisMap(AxisType type)
      {
        Type = type;
      }
    }

    [System.Serializable]
    private class ButtonMap
    {
      public ButtonType Type = default;
      public int No = -1;

      public ButtonMap (ButtonType type)
      {
        Type = type;
      }
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
      MyEnum.ForEach<AxisType>((type) => {
        this.axisMap.Add(new AxisMap(type));
      });

      MyEnum.ForEach<ButtonType>((type) => { 
        this.buttonMap.Add(new ButtonMap(type));
      });
    }

    //-------------------------------------------------------------------------
    // publicメソッド

    /// <summary>
    /// axisMapの要素をコールバックを指定して取得する
    /// </summary>
    public void GetAxes(System.Action<AxisType, int, bool> func)
    {
      this.axisMap.ForEach((map) => {
        func(map.Type, map.No, map.Invert);
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

        config.axisMap.ForEach((axis) => 
        {
          using (new EditorGUILayout.HorizontalScope()) 
          {
            EditorGUILayout.LabelField($"{axis.Type}");
            axis.No = EditorGUILayout.IntField(axis.No);
            GUILayout.Label("Invert");
            axis.Invert = EditorGUILayout.Toggle(axis.Invert);
          }
        });

        EditorGUILayout.Separator();

        // Buttonの設定
        EditorGUILayout.LabelField("Buttons");

        config.buttonMap.ForEach((button) => 
        {
          using (new EditorGUILayout.HorizontalScope()) 
          {
            EditorGUILayout.LabelField($"{button.Type}");
            button.No = EditorGUILayout.IntField(button.No);
          }
        });
      }
    }
#endif

  }
}
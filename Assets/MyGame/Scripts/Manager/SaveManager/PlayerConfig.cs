using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyGame.Define;
namespace MyGame.SaveManagement
{
  /**
   * プレイヤーのHPやMPなどのステータスに関する設定
   */
  [CreateAssetMenu(menuName ="MyGame/Create/PlayerConfig")]
  public class PlayerConfig : ScriptableObject
  {
    /// <summary>
    /// 最大HP
    /// </summary>
    public int MaxHp = 0;
    
    /// <summary>
    /// 最大MP
    /// </summary>
    [SerializeField]
    private List<int> maxMp = new List<int>(App.AttributeCount);

    /// <summary>
    /// 使用MP(スキル使用に必要なMPの設定)
    /// </summary>
    [SerializeField]
    private List<int> useMp = new List<int>(App.AttributeCount);

    /// <summary>
    /// 右クリックメニューから生成されたときに一度動作する
    /// </summary>
    private void Awake()
    {
      MyEnum.ForEach<App.Attribute>((attribute) => { 
        this.maxMp.Add(0);
        this.useMp.Add(0);
      });
    }

    public int GetMaxMp(App.Attribute attribute)
    {
      return this.maxMp[(int)attribute];
    }
    public void SetMaxMp(App.Attribute attribute, int mp)
    {
      this.maxMp[(int)attribute] = mp;
    }

    public int GetUseMp(App.Attribute attribute)
    {
      return this.useMp[(int)attribute];
    }
    public void SetUseMp(App.Attribute attribute, int mp)
    {
      this.useMp[(int)attribute] = mp;
    }

  }

#if UNITY_EDITOR

  /// <summary>
  /// PlayerConfigのInspector拡張
  /// </summary>
  [CustomEditor(typeof(PlayerConfig))]
  public class PlayerConfigEditor : Editor 
  {
    private PlayerConfig config;

    public override void OnInspectorGUI()
    {
      config = target as PlayerConfig;

      // 最大HP
      config.MaxHp = EditorGUILayout.IntField("最大HP", config.MaxHp);

      // 最大MP(属性ごと)
      EditorGUILayout.LabelField("最大MP");
      MaxMpField("火", App.Attribute.Fir);
      MaxMpField("水", App.Attribute.Wat);
      MaxMpField("雷", App.Attribute.Thu);
      MaxMpField("氷", App.Attribute.Ice);
      MaxMpField("木", App.Attribute.Tre);
      MaxMpField("聖", App.Attribute.Hol);
      MaxMpField("闇", App.Attribute.Dar);

      EditorGUILayout.LabelField("使用MP(スキル使用に必要なMPの設定)");
      UseMpField("火", App.Attribute.Fir);
      UseMpField("水", App.Attribute.Wat);
      UseMpField("雷", App.Attribute.Thu);
      UseMpField("氷", App.Attribute.Ice);
      UseMpField("木", App.Attribute.Tre);
      UseMpField("聖", App.Attribute.Hol);
      UseMpField("闇", App.Attribute.Dar);

      if (GUILayout.Button("Test")) {
        Debug.Logger.Log(UnityEngine.JsonUtility.ToJson(config));
      }
    }

    /// <summary>
    /// 最大MP
    /// </summary>
    private void MaxMpField(string name, App.Attribute attribute)
    {
      int mp = EditorGUILayout.IntField(name, config.GetMaxMp(attribute));
      config.SetMaxMp(attribute, mp);
    }

    /// <summary>
    /// 使用MP
    /// </summary>
    private void UseMpField(string name, App.Attribute attribute)
    {
      int mp = EditorGUILayout.IntField(name, config.GetUseMp(attribute));
      config.SetUseMp(attribute, mp);
    }
  }
#endif

}
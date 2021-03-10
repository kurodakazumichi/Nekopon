using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyGame.Define;

namespace MyGame
{
  /// <summary>
  /// PlayerConfigインターフェース
  /// </summary>
  public interface IPlayerConfig
  {
    /// <summary>
    /// 最大HP
    /// </summary>
    int MaxHp { get; }

    /// <summary>
    /// 最大MP
    /// </summary>
    int GetMaxMp(App.Attribute attribute);

    /// <summary>
    /// 使用MP
    /// </summary>
    int GetUseMp(App.Attribute attribute);
  }
  /**
   * プレイヤーのHPやMPなどのステータスに関する設定
   */
  [CreateAssetMenu(menuName = "MyGame/Create/PlayerConfig")]
  public class PlayerConfig : ScriptableObject, IPlayerConfig
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 最大HP
    /// </summary>
    [SerializeField]
    private int maxHp = 0;

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
    /// 最大AP
    /// </summary>
    [SerializeField]
    private int maxAp = 0;

    /// <summary>
    /// 使用AP
    /// </summary>
    [SerializeField]
    private int useAp = 0;

    //-------------------------------------------------------------------------
    // ライフサイクル

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

    //-------------------------------------------------------------------------
    // アクセッサ

    /// <summary>
    /// 最大HP
    /// </summary>
    public int MaxHp {
      get { return this.maxHp; }
      set { this.maxHp = value; }
    }

    /// <summary>
    /// 最大AP
    /// </summary>
    public int MaxAp {
      get { return this.maxAp; }
      set { this.maxAp = value; }
    }

    /// <summary>
    /// 使用AP
    /// </summary>
    public int UseAp {
      get { return this.useAp; }
      set { this.useAp = value; }
    }

    /// <summary>
    /// 最大MPを取得
    /// </summary>
    public int GetMaxMp(App.Attribute attribute)
    {
      return this.maxMp[(int)attribute];
    }

    /// <summary>
    /// 最大MPを設定
    /// </summary>
    public void SetMaxMp(App.Attribute attribute, int mp)
    {
      this.maxMp[(int)attribute] = mp;
    }

    /// <summary>
    /// 使用MPを取得
    /// </summary>
    public int GetUseMp(App.Attribute attribute)
    {
      return this.useMp[(int)attribute];
    }

    /// <summary>
    /// 使用MPを設定
    /// </summary>
    public void SetUseMp(App.Attribute attribute, int mp)
    {
      this.useMp[(int)attribute] = mp;
    }

#if _DEBUG

    /// <summary>
    /// デバッグ
    /// </summary>
    public void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) 
      {
        // HP
        GUILayout.Label($"最大HP:{MaxHp}");

        // MP
        GUILayout.Label("MP:Use / Max");
        MyEnum.ForEach<App.Attribute>((attribute) => { 
          GUILayout.Label($"{attribute}:{GetUseMp(attribute)}/{GetUseMp(attribute)}");
        });
      }
    }
#endif
  }

#if UNITY_EDITOR
  //---------------------------------------------------------------------------
  // Inspector拡張

  /// <summary>
  /// PlayerConfigのInspector拡張
  /// </summary>
  [CustomEditor(typeof(PlayerConfig))]
  public class PlayerConfigEditor : ScriptableObjectInspectorEditorBase<PlayerConfig> 
  {
    private bool isOpenedMaxMp = false;
    private bool isOpenedUseMp = false;

    protected override void OnMyInspectorGUI()
    {
      // 最大HP
      config.MaxHp = EditorGUILayout.IntField("最大HP", config.MaxHp);

      // 最大MP(属性別)
      this.isOpenedMaxMp = EditorGUILayout.Foldout(this.isOpenedMaxMp, "最大MP");
      if (this.isOpenedMaxMp) { 
        MaxMpField("火", App.Attribute.Fir);
        MaxMpField("水", App.Attribute.Wat);
        MaxMpField("雷", App.Attribute.Thu);
        MaxMpField("氷", App.Attribute.Ice);
        MaxMpField("木", App.Attribute.Tre);
        MaxMpField("聖", App.Attribute.Hol);
        MaxMpField("闇", App.Attribute.Dar);
      }

      // 使用MP(属性別)
      this.isOpenedUseMp = EditorGUILayout.Foldout(this.isOpenedUseMp, "使用MP(スキル使用に必要なMPの設定)");
      if (this.isOpenedUseMp) {
        UseMpField("火", App.Attribute.Fir);
        UseMpField("水", App.Attribute.Wat);
        UseMpField("雷", App.Attribute.Thu);
        UseMpField("氷", App.Attribute.Ice);
        UseMpField("木", App.Attribute.Tre);
        UseMpField("聖", App.Attribute.Hol);
        UseMpField("闇", App.Attribute.Dar);
      }

      // 最大・使用AP
      config.MaxAp = EditorGUILayout.IntField("最大AP", config.MaxAp);
      config.UseAp = EditorGUILayout.IntField("使用AP", config.UseAp);
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
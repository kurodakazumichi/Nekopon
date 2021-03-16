using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Versus;

namespace MyGame
{
  /// <summary>
  /// 肉球エフェクト管理
  /// </summary>
  public class PawEffectManager : SingletonMonoBehaviour<PawEffectManager>
  {
    /// <summary>
    /// エフェクトの種類
    /// </summary>
    public enum Type
    {
      Freeze,
      Paralysis,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// オブジェクトプール
    /// </summary>
    private ObjectPool<PawEffect> pool = new ObjectPool<PawEffect>();

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 凍結用Spriteリソース
    /// </summary>
    private static readonly List<Sprite> FREEZE_SPRITES = new List<Sprite>();

    /// <summary>
    /// 麻痺用Spriteリソース
    /// </summary>
    private static readonly List<Sprite> NUMB_SPRITES = new List<Sprite>();

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;

      // 凍結エフェクトのスプライトは3枚
      for(int i = 1; i <= 3; ++i) {
        rs.Load<Sprite>($"Effect.Paw.Freeze.0{i}.sprite", pre, done, (res) => { FREEZE_SPRITES.Add(res); });
      }

      // 麻痺エフェクトのスプライトは4枚
      for(int i = 1; i <= 4; ++i) {
        rs.Load<Sprite>($"Effect.Paw.Numb.0{i}.sprite", pre, done, (res) => { NUMB_SPRITES.Add(res); });
      }
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;

      // 凍結エフェクトのスプライトは3枚
      for (int i = 1; i <= 3; ++i) {
        rs.Unload($"Effect.Paw.Freeze.0{i}.sprite");
      }

      // 麻痺エフェクトのスプライトは4枚
      for (int i = 1; i <= 4; ++i) {
        rs.Unload($"Effect.Paw.Numb.0{i}.sprite");
      }
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // デバッグ登録
      DebugSystem.Instance.Regist(this);

      // ObjectPoolの設定
      this.pool.SetGenerator(() => 
      {
        var e = new GameObject("PawEffect").AddComponent<PawEffect>();
        e.SetParent(CacheTransform);
        return e;
      });
    }

    protected override void OnMyDestory()
    {
      // デバッグ解除
      DebugSystem.Instance.Discard(this);
    }

    //-------------------------------------------------------------------------
    // 生成

    /// <summary>
    /// エフェクト生成
    /// </summary>
    public PawEffect Create(Type type)
    {
      var e = this.pool.Create();

      switch (type) {
        case Type.Freeze: SetupFreeze(e); break;
        case Type.Paralysis  : SetupNumb(e); break;
      }

      e.ToUsual();

      return e;
    }

    /// <summary>
    /// エフェクト解除
    /// </summary>
    public void Release(PawEffect e)
    {
      if (e == null) return;

      e.SetParent(CacheTransform);
      this.pool.Release(e);
    }

    /// <summary>
    /// 凍結エフェクトの設定をする
    /// </summary>
    private void SetupFreeze(PawEffect e)
    {
      e.Reset();
      e.Interval = 0.2f;
      e.SortingOrder = Define.Layer.Order.Layer00;
      FREEZE_SPRITES.ForEach((sprite) => { e.AddSprite(sprite); });
    }

    /// <summary>
    /// 麻痺エフェクトの設定をする
    /// </summary>
    private void SetupNumb(PawEffect e)
    {
      e.Reset();
      e.Interval = 0.01f;
      e.SortingOrder = Define.Layer.Order.Layer10;
      NUMB_SPRITES.ForEach((sprite) => { e.AddSprite(sprite); });
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    public override void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) 
      {
        GUILayout.Label("Create Menu");
        OnDebugCreateButtons();

        GUILayout.Label("Release Menu");
        OnDebugReleaseButton();
      }
    }

    private void OnDebugCreateButtons()
    {
      using (new GUILayout.HorizontalScope()) 
      {
        if (GUILayout.Button("Freeze")) {
          Create(Type.Freeze);
        }

        if (GUILayout.Button("Numb")) {
          Create(Type.Paralysis);
        }
      }
    }

    private void OnDebugReleaseButton()
    {
      if (GUILayout.Button("Release")) 
      {
        // 選択中のオブジェクトチェック
        var go = UnityEditor.Selection.activeGameObject;

        if (go == null) { 
          Debug.Logger.Log("PawEffectManageer: オブジェクトが選択されていません。");
          return; 
        }

        // PawEffectチェック
        var e = go.GetComponent<PawEffect>();

        if (e == null) {
          Debug.Logger.Log("PawEffectManager: PawEffectオブジェクトを選択してください。");
          return;
        }

        Release(e);
      }
    }
#endif
  }
}

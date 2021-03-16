using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Versus;

namespace MyGame
{
  /// <summary>
  /// 通常攻撃、及び属性スキルを管理するマネージャー
  /// </summary>
  public class SkillManager : SingletonMonoBehaviour<SkillManager>
  {
    /// <summary>
    /// スキルとしてのインターフェース
    /// </summary>
    public interface ISkill : IPoolable
    {
      /// <summary>
      /// セットアップ可能
      /// </summary>
      void Setup();

      /// <summary>
      /// Idleに出来る
      /// </summary>
      void ToIdle();
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// オブジェクトプール(通常攻撃)
    /// </summary>
    private ObjectPool<ISkill> attacks = new ObjectPool<ISkill>();

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Attack.Load(pre, done);
    }

    public static void Unload()
    {
      Attack.Unload();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // デバッグ登録
      DebugSystem.Instance.Regist(this);

      // ObjectPoolの初期設定
      InitPoolForAttack();
    }

    protected override void OnMyDestory()
    {
      // デバッグ解除
      DebugSystem.Instance.Discard(this);
    }

    //-------------------------------------------------------------------------
    // 通常攻撃

    /// <summary>
    /// Attackユニットプールの初期設定
    /// </summary>
    public void InitPoolForAttack()
    {
      // Generatorを定義
      this.attacks.SetGenerator(() => {
        var attack = new GameObject("Attack").AddComponent<Attack>();
        attack.SetParent(CacheTransform);
        return attack;
      });

      // 2つ予約
      this.attacks.Reserve(2);
    }

    /// <summary>
    /// 攻撃ユニットを生成
    /// </summary>
    public Attack Create(Transform parent)
    {
      var attack = this.attacks.Create();
      attack.SetParent(parent);
      attack.Setup();
      return attack as Attack;
    }

    /// <summary>
    /// 攻撃ユニットを返却
    /// </summary>
    public void Release(ISkill attack)
    {
      attack.ToIdle();
      this.attacks.Release(attack, CacheTransform);
    }
  }
}

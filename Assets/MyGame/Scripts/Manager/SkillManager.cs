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
    /// 攻撃ユニットのInterface
    /// </summary>
    public interface IAttack : IPoolable
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

    /// <summary>
    /// 属性スキルユニットのInterface
    /// </summary>
    public interface ISkill : IPoolable
    {
      void Setup();
      void Fire(Player owner, Player target);
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// オブジェクトプール(通常攻撃)
    /// </summary>
    private ObjectPool<IAttack> attacks = new ObjectPool<IAttack>();

    /// <summary>
    /// 火属性スキルオブジェクトプール
    /// </summary>
    private ObjectPool<ISkill> firs = new ObjectPool<ISkill>();

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Attack.Load(pre, done);
      SkillFir.Load(pre, done);
    }

    public static void Unload()
    {
      Attack.Unload();
      SkillFir.Unload();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // デバッグ登録
      DebugSystem.Instance.Regist(this);

      // ObjectPoolの初期設定
      InitPoolForAttack();
      InitPoolForFir();
    }

    protected override void OnMyDestory()
    {
      // デバッグ解除
      DebugSystem.Instance.Discard(this);
    }

    //-------------------------------------------------------------------------
    // 初期化関連

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
    /// 火属性スキルオブジェクトプールの初期設定
    /// </summary>
    public void InitPoolForFir()
    {
      this.firs.SetGenerator(() => { 
        var unit = new GameObject("SkillFir").AddComponent<SkillFir>();
        unit.SetParent(CacheTransform);
        return unit;
      });

      this.firs.Reserve(2);
    }

    //-------------------------------------------------------------------------
    // 通常攻撃

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

    public ISkill Create(Define.App.Attribute attribute)
    {
      var unit = this.firs.Create();
      unit.Setup();
      return unit;
    }

    /// <summary>
    /// 攻撃ユニットを返却
    /// </summary>
    public void Release(IAttack attack)
    {
      attack.ToIdle();
      this.attacks.Release(attack, CacheTransform);
    }
  }
}

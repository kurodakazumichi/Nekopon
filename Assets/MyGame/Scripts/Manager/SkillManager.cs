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
      /// <summary>
      /// セットアップ可能
      /// </summary>
      void Setup();

      /// <summary>
      /// 発動可能
      /// </summary>
      void Fire(Player owner, Player target);
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// オブジェクトプール(通常攻撃)
    /// </summary>
    private ObjectPool<IAttack> attacks = new ObjectPool<IAttack>();

    /// <summary>
    /// 属性スキル用オブジェクトプール
    /// </summary>
    private Dictionary<int, ObjectPool<ISkill>> skills = new Dictionary<int, ObjectPool<ISkill>>();

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Attack.Load(pre, done);
      SkillFir.Load(pre, done);
      SkillWat.Load(pre, done);
    }

    public static void Unload()
    {
      Attack.Unload();
      SkillFir.Unload();
      SkillWat.Unload();
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // デバッグ登録
      DebugSystem.Instance.Regist(this);

      // ObjectPoolの初期設定
      InitPoolForAttack();
      InitPoolForSkill<SkillFir>(Define.App.Attribute.Fir);
      InitPoolForSkill<SkillWat>(Define.App.Attribute.Wat);
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
    private void InitPoolForAttack()
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
    /// 属性スキル用オブジェクトプールの初期設定
    /// </summary>
    /// <typeparam name="T">Componentであり、ISkillを実装している</typeparam>
    /// <param name="attribute">属性</param>
    private void InitPoolForSkill<T>(Define.App.Attribute attribute) where T : Component, ISkill
    {
      // プール生成
      var pool = new ObjectPool<ISkill>();

      // Generator設定
      pool.SetGenerator(() => { 
        return MyGameObject.Create<T>("Skill", CacheTransform);
      });

      // 2人分予約
      pool.Reserve(2);

      // 登録
      this.skills.Add((int)attribute, pool);
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

    /// <summary>
    /// 攻撃ユニットを返却
    /// </summary>
    public void Release(IAttack attack)
    {
      attack.ToIdle();
      this.attacks.Release(attack, CacheTransform);
    }

    //-------------------------------------------------------------------------
    // 属性スキル

    /// <summary>
    /// 属性スキルユニットを生成
    /// </summary>
    public ISkill Create(Define.App.Attribute attribute)
    {
      var unit = GetPoolForSkill(attribute).Create();
      unit.Setup();
      return unit;
    }

    /// <summary>
    /// 属性スキルユニットを返却
    /// </summary>
    public void Release(Define.App.Attribute attribute, ISkill skill)
    {
      GetPoolForSkill(attribute).Release(skill, CacheTransform);
    }

    /// <summary>
    /// 属性スキルプールを取得
    /// </summary>
    private ObjectPool<ISkill> GetPoolForSkill(Define.App.Attribute attribute)
    {
      return this.skills[(int)attribute];
    }
  }
}

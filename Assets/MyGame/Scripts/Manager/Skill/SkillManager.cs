using MyGame.Unit.Versus;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// 通常攻撃、及び属性スキルを管理するマネージャー
  /// </summary>
  public class SkillManager : SingletonMonoBehaviour<SkillManager>
  {
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
      SkillThu.Load(pre, done);
      SkillIce.Load(pre, done);
      SkillTre.Load(pre, done);
      SkillHol.Load(pre, done);
      SkillDar.Load(pre, done);
    }

    public static void Unload()
    {
      Attack.Unload();
      SkillFir.Unload();
      SkillWat.Unload();
      SkillThu.Unload();
      SkillIce.Unload();
      SkillTre.Unload();
      SkillHol.Unload();
      SkillDar.Unload();
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
      InitPoolForSkill<SkillThu>(Define.App.Attribute.Thu);
      InitPoolForSkill<SkillIce>(Define.App.Attribute.Ice);
      InitPoolForSkill<SkillTre>(Define.App.Attribute.Tre);
      InitPoolForSkill<SkillHol>(Define.App.Attribute.Hol);
      InitPoolForSkill<SkillDar>(Define.App.Attribute.Dar);
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
      pool.SetGenerator(() => 
      { 
        return MyGameObject
          .Create<T>($"{attribute}", CacheTransform)
          .Init(attribute);
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
    public void Release(ISkill skill)
    {
      GetPoolForSkill(skill.Attribute).Release(skill, CacheTransform);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// 属性スキルのベースクラス
  /// </summary>
  public abstract class SkillBase<TState> : Unit<TState>, SkillManager.ISkill where TState : System.Enum
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// ベース位置(汎用的に利用)
    /// </summary>
    protected Vector3 basePosition   = Vector3.zero;

    /// <summary>
    /// 目標地点(汎用的に利用)
    /// </summary>
    protected Vector3 targetPosition = Vector3.zero;

    /// <summary>
    /// スキルのオーナー(発動者)
    /// </summary>
    protected Player owner = null;

    /// <summary>
    /// スキルの対象(被弾者)
    /// </summary>
    protected Player target = null;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// スキルの属性
    /// </summary>
    public Define.App.Attribute Attribute { get; private set; }

    //-------------------------------------------------------------------------
    // ISkill Interfaceの実装

    public virtual SkillManager.ISkill Init(Define.App.Attribute attribute)
    {
      Attribute = attribute;
      return this;
    }

    /// <summary>
    /// スキルのセットアップ(Resourceの設定)
    /// </summary>
    public abstract void Setup();

    /// <summary>
    /// スキルの発動
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="target"></param>
    public virtual void Fire(Player owner, Player target)
    {
      SetActive(true);
      this.owner = owner;
      this.target = target;
    }
  }
}

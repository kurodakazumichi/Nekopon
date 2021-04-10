using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// AI用の脳
  /// </summary>
  public class BrainAI : IBrain
  {
    /// <summary>
    /// BrainAIの生成に必要な物が詰まっている
    /// </summary>
    public class Props
    {
      public Player owner = null;
      public Player target = null;
    }

    /// <summary>
    /// コンストラクタで渡されるAIに必要な小道具
    /// </summary>
    protected readonly Props props = null;

    /// <summary>
    /// 目的の属性
    /// </summary>
    private App.Attribute TargetAttribute = default;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public BrainAI(Props props)
    {
      this.props = props;
    }

    /// <summary>
    /// 思考
    /// </summary>
    public IAction Think()
    {



      return null;
    }

    /// <summary>
    /// 目的の属性を取得
    /// </summary>
    private App.Attribute GetTargetAttribute() {

        // TODO: 欲しい属性を決める要素
        // 1. MPのチャージ状況
        // 2. HPの状況
        // 3. キャラの好み

        return MyEnum.Random<App.Attribute>();
    }
  }
}


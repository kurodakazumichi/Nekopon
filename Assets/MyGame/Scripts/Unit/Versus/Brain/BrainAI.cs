using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
  }
}


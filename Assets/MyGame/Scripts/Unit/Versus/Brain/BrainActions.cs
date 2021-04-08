using MyGame.Define;
using UnityEngine;

namespace MyGame.Unit.Versus.BrainAction
{
  /// <summary>
  /// アクションの基底クラス
  /// </summary>
  public abstract class ActionBase : IAction
  {
    /// <summary>
    /// プレイヤー(Owner)
    /// </summary>
    protected Player owner = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ActionBase(Player owner)
    {
      this.owner = owner;
    }

    /// <summary>
    /// 実行
    /// </summary>
    public abstract void Execute();
  }

  /// <summary>
  /// 肉球カーソルを動かすアクション
  /// </summary>
  public class MoveCursorAction : ActionBase
  {
    /// <summary>
    /// 方向
    /// </summary>
    private Vector3 direction = Vector3.zero;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MoveCursorAction(Player owner, Vector3 direction) : base(owner)
    {
      this.direction = direction;
    }

    /// <summary>
    /// カーソル移動を実行
    /// </summary>
    public override void Execute()
    {
      this.owner.TryMoveCursor(DirectionBy(this.direction));
    }

    /// <summary>
    /// ベクトルを方向に変換
    /// </summary>
    public App.Direction DirectionBy(Vector3 dir)
    {
      // xがマイナスなら左、プラスなら右
      if (dir.x < 0) return App.Direction.L;
      if (0 < dir.x) return App.Direction.R;

      // yがマイナスなら下、プラスなら上
      if (dir.y < 0) return App.Direction.D;
      if (0 < dir.y) return App.Direction.U;

      return App.Direction.N;
    }
  }

  /// <summary>
  /// 肉球を選択するアクション
  /// </summary>
  public class SelectPawAction : ActionBase
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="owner"></param>
    public SelectPawAction(Player owner) : base(owner)
    {
    }

    /// <summary>
    /// 肉球を選択する
    /// </summary>
    public override void Execute()
    {
      this.owner.TrySelectPaw();
    }
  }

  /// <summary>
  /// 肉球選択を解除するアクション
  /// </summary>
  public class ReleasePawAction : ActionBase
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ReleasePawAction(Player owner) : base(owner)
    {
    }

    /// <summary>
    /// 肉球選択を解除する
    /// </summary>
    public override void Execute()
    {
      this.owner.TryReleasePaw();
    }
  }

  /// <summary>
  /// 連鎖するアクション
  /// </summary>
  public class ChainAction : ActionBase
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ChainAction(Player owner) : base(owner)
    {
    }

    /// <summary>
    /// 連鎖する
    /// </summary>
    public override void Execute()
    {
      this.owner.TryChain();
    }
  }

  /// <summary>
  /// スキルガイドアクション
  /// </summary>
  public class SkillGuideAction : ActionBase
  {
    private bool isShow = false;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SkillGuideAction(Player owner, bool isShow) : base(owner)
    {
      this.isShow = isShow;
    }

    /// <summary>
    /// スキルガイドの表示/非表示
    /// </summary>
    public override void Execute()
    {
      Debug.Logger.Log($"ShowSkillGuide:{this.isShow}");
    }
  }

  /// <summary>
  /// 属性スキル発動アクション
  /// </summary>
  public class FireAttributeAction : ActionBase
  {
    /// <summary>
    /// 発動するスキルの属性
    /// </summary>
    private App.Attribute attribute = default;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public FireAttributeAction(Player owner, App.Attribute attribute) : base(owner)
    {
      this.attribute = attribute;
    }

    /// <summary>
    /// スキル発動を試す
    /// </summary>
    public override void Execute()
    {
      this.owner.TryFireAttributeSkill(this.attribute);
    }
  }

  /// <summary>
  /// 固有スキル発動アクション
  /// </summary>
  public class FireUniqueSkillAction : ActionBase
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public FireUniqueSkillAction(Player owner) : base(owner)
    {
    }

    /// <summary>
    /// スキルガイドの表示/非表示
    /// </summary>
    public override void Execute()
    {
      this.owner.TryFireUniqueSkill();
    }
  }

}
using MyGame.Define;

namespace MyGame.Unit.Versus.BrainAction
{
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
    public FireAttributeAction(Player owner, App.Attribute attribute):base(owner)
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
}
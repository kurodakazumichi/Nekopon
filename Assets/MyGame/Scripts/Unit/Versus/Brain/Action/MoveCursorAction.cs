using MyGame.Define;
using UnityEngine;

namespace MyGame.Unit.Versus.BrainAction
{
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
      this.owner.MoveCursor(DirectionBy(this.direction));
    }

    /// <summary>
    /// ベクトルを方向に変換
    /// </summary>
    public App.Direction DirectionBy(Vector3 dir)
    {
      // xがマイナスなら左、プラスなら右
      if (dir.x <= 0) return App.Direction.L;
      if (0 < dir.x)  return App.Direction.R;

      // yがマイナスなら下、プラスなら上
      if (dir.y <= 0) return App.Direction.D;
      if (0 < dir.y)  return App.Direction.U;

      return App.Direction.N;
    }
  }
}
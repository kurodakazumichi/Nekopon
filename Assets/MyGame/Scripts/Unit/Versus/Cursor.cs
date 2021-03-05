using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// Puzzle用のカーソル
  /// 特に複雑な事はなく、ただスプライトを表示するだけのもの
  /// </summary>
  public class Cursor : Unit<Cursor.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State { }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// カーソルのスプライトを表示するため
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// カーソル用のSprite
    /// </summary>
    private static Sprite CursorSprite = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceManager.Instance;
      rm.Load<Sprite>("Cursor.Puzzle.sprite", pre, done, (res) => { CursorSprite = res; });
    }

    public static void Unload()
    {
      var rm = ResourceManager.Instance;
      rm.Unload("Cursor.Puzzle.sprite");
      CursorSprite = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
      this.spriteRenderer.sprite = CursorSprite;
    }
  }

}

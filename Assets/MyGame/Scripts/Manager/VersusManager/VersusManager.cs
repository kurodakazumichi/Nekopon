using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.VersusManagement;

namespace MyGame
{
  public class VersusManager : SingletonMonobehaviour<VersusManager>
  {
    /// <summary>
    /// 対戦画面の各種ロケーション
    /// </summary>
    private Dictionary<Define.App.Player, Location> locations = new Dictionary<Define.App.Player, Location>();

    private Puzzle puzzle1 = null;
    private Puzzle puzzle2 = null;

    //-------------------------------------------------------------------------
    // Load, Unload

    public static void Load(System.Action pre, System.Action done)
    {
      Puzzle.Load(pre, done);
    }

    public static void Unload()
    {
      Puzzle.Unload();
    }

    /// <summary>
    /// ロケーションをセットアップする、ロケーション情報を持ったオブジェクトを受け取る。
    /// </summary>
    public void Setup(GameObject locations)
    {
      this.locations.Add(Define.App.Player.P1, new Location("P1", locations));
      this.locations.Add(Define.App.Player.P2, new Location("P2", locations));

      this.puzzle1 = new Puzzle(CacheTransform, this.locations[Define.App.Player.P1].Paw);
      this.puzzle2 = new Puzzle(CacheTransform, this.locations[Define.App.Player.P2].Paw);

      this.puzzle1.Setup();
      this.puzzle2.Setup();
    }

    protected override void MyUpdate()
    {
      // カーソル移動(上下左右)
      if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        this.puzzle1.MoveCursorL();
      }      
      if (Input.GetKeyDown(KeyCode.RightArrow)) {
        this.puzzle1.MoveCursorR();
      }
      if (Input.GetKeyDown(KeyCode.UpArrow)) {
        this.puzzle1.MoveCursorU();
      }
      if (Input.GetKeyDown(KeyCode.DownArrow)) {
        this.puzzle1.MoveCursorD();
      }

      // 肉球選択 or 入れ替え
      if (Input.GetKeyDown(KeyCode.Z)) 
      {
        // 選択中の肉球があれば入れ替え
        if (this.puzzle1.HasSelectedPaw) {
          this.puzzle1.Swap();
        } 
        
        // 選択中の肉中がなければカーソルのある所の肉球を選択
        else {
          this.puzzle1.SelectPaw();
        }
      }

      // 肉球選択の解除
      if (Input.GetKeyDown(KeyCode.X)) {
        this.puzzle1.ReleasePaw();
      }
    }
  }

}

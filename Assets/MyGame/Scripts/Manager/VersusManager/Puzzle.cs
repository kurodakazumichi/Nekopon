using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Versus;

namespace MyGame.VersusManagement
{
  public class Puzzle
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 親オブジェクト
    /// </summary>
    private Transform parent = null;

    /// <summary>
    /// Puzzle内のゲームオブジェクトを格納しておくｒゲームオブジェクト
    /// </summary>
    private Transform folder = null;

    /// <summary>
    /// パズル配置のベース位置
    /// </summary>
    private Vector3 basePosition = Vector3.zero;

    /// <summary>
    /// カーソルの座標
    /// </summary>
    private Vector2Int cursorCoord = Vector2Int.zero;

    /// <summary>
    /// 肉球リスト
    /// </summary>
    private Paw[] paws = new Paw[Define.Versus.PAW_TOTAL];

    /// <summary>
    /// カーソル
    /// </summary>
    private Unit.Versus.Cursor cursor = null;

    /// <summary>
    /// 選択中の肉球を指すIndex、未選択時は -1
    /// </summary>
    private int selectedIndex = -1;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// 選択された肉球をもっているかどうか
    /// </summary>
    public bool HasSelectedPaw {
      get {
        return (this.selectedIndex != -1);
      }
    }

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 肉球のPrefab
    /// </summary>
    private static GameObject PawPrefab = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceManager.Instance;
      rm.Load<GameObject>("VS.Paw.prefab", pre, done, (res) => { PawPrefab = res; });
      Paw.Load(pre, done);
      Unit.Versus.Cursor.Load(pre, done);
    }

    public static void Unload()
    {
      var rm = ResourceManager.Instance;
      rm.Unload("VS.Paw.prefab");
      Paw.Unload();
      Unit.Versus.Cursor.Unload();
      PawPrefab = null;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Puzzle(Transform parent, Vector3 basePosition)
    {
      this.parent = parent;
      this.basePosition = basePosition;
    }

    /// <summary>
    /// 肉球を生成してプールする
    /// </summary>
    public void Setup()
    {
      // place folderを最初に作っておく
      this.folder = new GameObject("puzzle").transform;
      this.folder.parent = this.parent;

      // カーソル、肉球を用意
      PrepareCursor();
      PreparePaws();
    }

    /// <summary>
    /// カーソルの用意
    /// </summary>
    private void PrepareCursor()
    {
      this.cursor = new GameObject("curosr").AddComponent<Unit.Versus.Cursor>();
      this.cursor.SetParent(this.folder);
      SyncCursorPosition();
    }

    /// <summary>
    /// 肉球のセットアップ
    /// </summary>
    private void PreparePaws()
    {
      for (int i = 0; i < Define.Versus.PAW_TOTAL; ++i) {
        this.paws[i] = Object.Instantiate(PawPrefab).GetComponent<Paw>();

        var pos = PositionBy(CoordBy(i));
        this.paws[i].CacheTransform.position = pos;
        this.paws[i].SetParent(this.folder);
        this.paws[i].RandomAttribute();
      }
    }

    /// <summary>
    /// xy座標をパズル内のワールド座標へ
    /// </summary>
    private Vector3 PositionBy(int x, int y)
    {
      return this.basePosition + new Vector3(
        Define.Versus.PAW_INTERVAL_X * x,
        Define.Versus.PAW_INTERVAL_Y * y,
        0
      );
    }

    /// <summary>
    /// xy座標をパズル内のワールド座標へ
    /// </summary>
    private Vector3 PositionBy(Vector2Int coord)
    {
      return PositionBy(coord.x, coord.y);
    }

    /// <summary>
    /// 肉球を指すIndexをワールド座標へ
    /// </summary>
    private Vector3 PositionBy(int index)
    {
      return PositionBy(CoordBy(index));
    }

    /// <summary>
    /// 整数からパズル内のxy座標に変換する
    /// </summary>
    private Vector2Int CoordBy(int index)
    {
      return new Vector2Int(index % Define.Versus.PAW_COL, index / Define.Versus.PAW_COL);
    }

    private int IndexBy(Vector2Int coord)
    {
      return coord.y * Define.Versus.PAW_COL + coord.x;
    }

    /// <summary>
    /// 現在のカーソル座標に実際のカーソルの位置を同期する
    /// </summary>
    private void SyncCursorPosition()
    {
      this.cursor.CacheTransform.position = PositionBy(this.cursorCoord);
    }

    public void MoveCursorL()
    {
      const int COL = Define.Versus.PAW_COL;
      this.cursorCoord.x = (COL + this.cursorCoord.x - 1) % COL;
      SyncCursorPosition();
    }

    public void MoveCursorR()
    {
      this.cursorCoord.x = (this.cursorCoord.x + 1) % Define.Versus.PAW_COL;
      SyncCursorPosition();
    }

    public void MoveCursorU()
    {
      this.cursorCoord.y = (this.cursorCoord.y + 1) % Define.Versus.PAW_ROW;
      SyncCursorPosition();
    }

    public void MoveCursorD()
    {
      const int ROW = Define.Versus.PAW_ROW;
      this.cursorCoord.y = (ROW + this.cursorCoord.y - 1) % ROW;
      SyncCursorPosition();
    }

    /// <summary>
    /// 肉球を選択する
    /// </summary>
    public void SelectPaw()
    {
      this.selectedIndex = IndexBy(this.cursorCoord);
      this.paws[this.selectedIndex].ToSelected();
    }

    /// <summary>
    /// 肉球を解放する
    /// </summary>
    public void ReleasePaw()
    {
      if (!HasSelectedPaw) return;
      this.paws[this.selectedIndex].ToUsual();
      this.selectedIndex = -1;
    }

    /// <summary>
    /// 肉球を入れ替える
    /// </summary>
    public void Swap()
    {
      // 選択された肉球がなければ何もしない
      if (!HasSelectedPaw) return;

      // 入れ替え元(src)と入れ替え先(dst)のindexを取得
      int srcIndex = this.selectedIndex;
      int dstIndex = IndexBy(this.cursorCoord);

      // 入れ替え元と先が同じであれば何もしない
      if (srcIndex == dstIndex) return;

      // 入れ替える前に選択状態を解除
      ReleasePaw();

      // 入れ替える肉球を取得
      var src = this.paws[srcIndex];
      var dst = this.paws[dstIndex];

      // 入れ替え作業
      src.CacheTransform.position = PositionBy(dstIndex);
      dst.CacheTransform.position = PositionBy(srcIndex);
      this.paws[srcIndex] = dst;
      this.paws[dstIndex] = src;
    }
  }

}

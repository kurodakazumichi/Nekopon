using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Versus;

namespace MyGame.VersusManagement
{
  public class Puzzle
  {
    /// <summary>
    /// 状態
    /// </summary>
    private enum State
    {
      Idle,
      Vanish,
      Refill,
      Finish,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 親オブジェクト
    /// </summary>
    private Transform parent = null;

    /// <summary>
    /// Puzzle内のゲームオブジェクトを格納しておくゲームオブジェクト
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

    /// <summary>
    /// ステートマシン
    /// </summary>
    private StateMachine<State> state = new StateMachine<State>();

    /// <summary>
    /// 連鎖の記録
    /// </summary>
    public ChainInfo ChainScore { get; private set; } = new ChainInfo();

    /// <summary>
    /// 連鎖モード
    /// </summary>
    public Define.Versus.ChainMode ChainMode { get; set; } = Define.Versus.ChainMode.Single;

    /// <summary>
    /// 肉球が消滅し終わったタイミングで呼ばれるコールバック
    /// </summary>
    public System.Action<ChainInfo> OnVanished = null;

    //-------------------------------------------------------------------------
    // プロパティ(導出項目)

    /// <summary>
    /// 選択された肉球をもっているかどうか
    /// </summary>
    public bool HasSelectedPaw {
      get {
        return (this.selectedIndex != -1);
      }
    }

    /// <summary>
    /// 動いてる肉球がいる
    /// </summary>
    public bool HasMovingPaw {
      get {
        bool isMoving = false;
        Util.ForEach(this.paws, (paw, _) => { return isMoving = paw.IsMoving; });
        return isMoving;
      }
    }

    /// <summary>
    /// 消失中の肉球がいる
    /// </summary>
    public bool HasVanishingPaw {
      get {
        bool isVanishing = false;
        Util.ForEach(this.paws, (paw, _) => { return isVanishing = paw.IsVanishing; });
        return isVanishing;
      }
    }

    /// <summary>
    /// 連鎖中かどうか
    /// </summary>
    public bool IsInChain {
      get { return this.state.StateKey != State.Idle; }
    }

    /// <summary>
    /// 連鎖が終了したか
    /// </summary>
    public bool IsFinishedChain => (this.state.StateKey == State.Finish);

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

    //-------------------------------------------------------------------------
    // 生成・準備系

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Puzzle(Transform parent, Vector3 basePosition)
    {
      this.parent = parent;
      this.basePosition = basePosition;

      // 状態の初期化
      this.state.Add(State.Idle);
      this.state.Add(State.Vanish, OnVanishEnter, OnVanishUpdate);
      this.state.Add(State.Refill, OnRefillEnter, OnRefillUpdate);
      this.state.Add(State.Finish);
      this.state.SetState(State.Idle);
    }

    /// <summary>
    /// 肉球、カーソルを生成してプールする
    /// </summary>
    public void Init()
    {
      // place folderを最初に作っておく
      this.folder = new GameObject("puzzle").transform;
      this.folder.parent = this.parent;

      // カーソル、肉球を用意
      InitCursor();
      InitPaws();
    }

    /// <summary>
    /// カーソルの用意
    /// </summary>
    private void InitCursor()
    {
      this.cursor = new GameObject("curosr").AddComponent<Unit.Versus.Cursor>();
      this.cursor.SetParent(this.folder).SetActive(false);
      SyncCursorPosition();
    }

    /// <summary>
    /// 肉球のセットアップ
    /// </summary>
    private void InitPaws()
    {
      // 肉球を生成
      for (int i = 0; i < Define.Versus.PAW_TOTAL; ++i) 
      {
        var paw = Object.Instantiate(PawPrefab).GetComponent<Paw>();
        paw.SetParent(this.folder).SetActive(false);
        this.paws[i] = paw;
      }
    }

    /// <summary>
    /// 初期配置
    /// </summary>
    public void Setup()
    {
      // 肉球を準備する
      Util.ForEach(this.paws, (paw, index) => 
      {
        Vector3 pos = PositionBy(index);
        float time  = Random.Range(Define.Versus.PAW_SETUP_MIN_TIME, Define.Versus.PAW_SETUP_MAX_TIME);

        paw.SetActive(true);
        paw.RandomAttribute();
        paw.CacheTransform.position = new Vector3(pos.x, 1f, 0);
        paw.ToMove(pos, time, Tween.Type.EaseOutBounce);
      });
    }

    //-------------------------------------------------------------------------
    // 変換系

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

    /// <summary>
    /// xy座標を肉球Indexに変換する
    /// </summary>
    private int IndexBy(int x, int y)
    {
      return y * Define.Versus.PAW_COL + x;
    }

    private int IndexBy(Vector2Int coord)
    {
      return IndexBy(coord.x, coord.y);
    }

    //-------------------------------------------------------------------------
    // カーソル関連

    /// <summary>
    /// 現在のカーソル座標に実際のカーソルの位置を同期する
    /// </summary>
    private void SyncCursorPosition()
    {
      this.cursor.CacheTransform.position = PositionBy(this.cursorCoord);
    }

    /// <summary>
    /// カーソルを左に動かす
    /// </summary>
    public void MoveCursorL()
    {
      const int COL = Define.Versus.PAW_COL;
      this.cursorCoord.x = (COL + this.cursorCoord.x - 1) % COL;
      SyncCursorPosition();
    }

    /// <summary>
    /// カーソルを右に動かす
    /// </summary>
    public void MoveCursorR()
    {
      this.cursorCoord.x = (this.cursorCoord.x + 1) % Define.Versus.PAW_COL;
      SyncCursorPosition();
    }

    /// <summary>
    /// カーソルを上に動かす
    /// </summary>
    public void MoveCursorU()
    {
      this.cursorCoord.y = (this.cursorCoord.y + 1) % Define.Versus.PAW_ROW;
      SyncCursorPosition();
    }

    /// <summary>
    /// カーソルを下に動かす
    /// </summary>
    public void MoveCursorD()
    {
      const int ROW = Define.Versus.PAW_ROW;
      this.cursorCoord.y = (ROW + this.cursorCoord.y - 1) % ROW;
      SyncCursorPosition();
    }

    /// <summary>
    /// カーソルの表示
    /// </summary>
    public void ShowCursor()
    {
      this.cursor.SetActive(true);
    }

    /// <summary>
    /// カーソルの非表示
    /// </summary>
    public void HideCursor()
    {
      this.cursor.SetActive(false);
    }

    //-------------------------------------------------------------------------
    // 肉球関連

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

    //-------------------------------------------------------------------------
    // 連鎖関連

    /// <summary>
    /// 連鎖開始(肉球を消失させる)
    /// </summary>
    public void StartChain()
    {
      // 連鎖中に連鎖は開始できない
      if (IsInChain) return;
      ChainScore.Reset();
      this.state.SetState(State.Vanish);
    }

    /// <summary>
    /// 連鎖の更新
    /// </summary>
    public void UpdateChain()
    {
      this.state.Update();
    }

    /// <summary>
    /// 連鎖の終了
    /// </summary>
    public void EndChain()
    {
      ChainScore.Reset();
      this.state.SetState(State.Idle);
    }

    /// <summary>
    /// 連鎖開始
    /// </summary>
    private void OnVanishEnter()
    {
      // 消えてたり、動いてたりするのがあったら連鎖開始しない
      if (HasVanishingPaw) return;
      if (HasMovingPaw) return;

      // 消えた肉球があるか
      bool there_are_vanished_paws = false;

      for (int y = 0; y < Define.Versus.PAW_ROW; ++y) {
        // 連鎖モードがSingleで、消えた肉球がある場合は判定を抜ける
        if (ChainMode == Define.Versus.ChainMode.Single) {
          if (there_are_vanished_paws) break;
        }

        for (int x = 0; x < Define.Versus.PAW_COL; ++x) 
        {
          // 評価対象の肉球
          var paw = this.paws[IndexBy(x, y)];

          // 既に評価済のパネルであればスキップ
          if (paw.IsEvaluated) continue;

          // 肉球のつながりを調べる
          int count = 0;
          LookUpPawConnection(x, y, ref count);

          // 肉球の連結数が連鎖可能な数を超えている場合は肉球を消滅状態にする
          if (Define.Versus.CHAIN_PAW_COUNT <= count) {
            there_are_vanished_paws = true;

            // 消失の記録を更新
            ChainScore.UpdateVanish(paw.Attribute, count);
            Vanish(x, y);

            // 連鎖モードがSingleならこの時点で連鎖判定を抜ける
            if (ChainMode == Define.Versus.ChainMode.Single) break;
          }
        }
      }

      // 評価済フラグを落とす
      Util.ForEach(this.paws, (paw, _) => { paw.IsEvaluated = false; });

      // 消えるのがある場合は連鎖数をUP、ない場合は連鎖終了へ
      if (HasVanishingPaw) {
        ChainScore.UpdateChainCount();
      } else {
        this.state.SetState(State.Finish);
      }
    }

    /// <summary>
    /// 連鎖で肉球が消えている最中
    /// </summary>
    private void OnVanishUpdate()
    {
      if (HasVanishingPaw) return;
      this.state.SetState(State.Refill);
    }

    /// <summary>
    /// 肉球の補充開始
    /// </summary>
    private void OnRefillEnter()
    {
      // 補充開始=消滅直後なのでここで消滅後のコールバックを呼び出す
      OnVanished?.Invoke(ChainScore);

      // 消えてない肉球を詰められるだけ下に詰める、消えてる肉球は必然的に上に集まる
      Util.ForEach(this.paws, (paw, index) => {
        if (!paw.IsIdle) StaffPawDown(index);
      });

      // 上の処理により肉球配置が変わったので、適切な場所に動かす
      Util.ForEach(this.paws, (paw, index) => 
      {
        // 移動に関するパラメータを定義
        var pos  = PositionBy(index);
        var time = Random.Range(Define.Versus.PAW_STAFF_MIN_TIME, Define.Versus.PAW_STAFF_MAX_TIME);
        var type = Tween.Type.EaseOutBounce;

        // 消えてる肉球が画面外の上の方に配置し、ランダムに属性を変更
        if (paw.IsIdle) {
          paw.CacheTransform.position = new Vector3(pos.x, 0.8f, 0);
          paw.RandomAttribute();
        }

        // 移動開始
        paw.ToMove(pos, time, type);
      });
    }

    /// <summary>
    /// 肉球の補充中
    /// </summary>
    private void OnRefillUpdate()
    {
      if (HasMovingPaw) return;
      this.state.SetState(State.Vanish);
    }

    /// <summary>
    /// 指定した肉球、及びその肉球に繋がっている肉球を消滅させる
    /// このメソッドはLookUpPadConnectionによって肉球のつながりを調べた後に使用すること
    /// </summary>
    private void Vanish(int x, int y)
    {
      // 肉球
      Paw curr = this.paws[IndexBy(x, y)];
      Paw next;

      // 評価フラグを落とし、消滅状態へ設定する
      curr.IsEvaluated = false;
      curr.ToVanish();

      // 上方向を消していく
      if (y < Define.Versus.PAW_ROW - 1) {
        next = this.paws[IndexBy(x, y + 1)];
        if (next.IsEvaluated && curr.CanConnect(next)) Vanish(x, y + 1);
      }

      // 下方向を消していく
      if (0 < y) {
        next = this.paws[IndexBy(x, y - 1)];
        if (next.IsEvaluated && curr.CanConnect(next)) Vanish(x, y - 1);
      }

      // 左方向を消していく
      if (0 < x) {
        next = this.paws[IndexBy(x - 1, y)];
        if (next.IsEvaluated && curr.CanConnect(next)) Vanish(x - 1, y);
      }

      // 右方向を消していく
      if (x < Define.Versus.PAW_COL - 1) {
        next = this.paws[IndexBy(x + 1, y)];
        if (next.IsEvaluated && curr.CanConnect(next)) Vanish(x + 1, y);
      }
    }

    /// <summary>
    /// x,yの位置にある肉球に繋がってる肉球の数を調べ上げる
    /// </summary>
    private void LookUpPawConnection(int x, int y, ref int count)
    {
      // 連結数をカウントアップ
      count++;

      // 繋がりをみる肉球をいれておく変数
      Paw curr = this.paws[IndexBy(x, y)];
      Paw next;

      // 評価中の肉球は評価済にしておく
      curr.IsEvaluated = true;

      // 上方向の繋がりをみる
      if (y < Define.Versus.PAW_ROW - 1) 
      {
        next = this.paws[IndexBy(x, y + 1)];
        if (!next.IsEvaluated && curr.CanConnect(next)) LookUpPawConnection(x, y + 1, ref count);
      }

      // 下方向のつながりを見る
      if (0 < y) {
        next = this.paws[IndexBy(x, y - 1)];
        if (!next.IsEvaluated && curr.CanConnect(next)) LookUpPawConnection(x, y - 1, ref count);
      }

      // 左方向のつながりを見る
      if (0 < x) {
        next = this.paws[IndexBy(x - 1, y)];
        if (!next.IsEvaluated && curr.CanConnect(next)) LookUpPawConnection(x - 1, y, ref count);
      }

      // 右方向のつながりを見る
      if (x < Define.Versus.PAW_COL - 1) {
        next = this.paws[IndexBy(x + 1, y)];
        if (!next.IsEvaluated && curr.CanConnect(next)) LookUpPawConnection(x + 1, y, ref count);
      }

    }

    /// <summary>
    /// 連鎖すると肉球が消え場所が空くので、消えてない肉球を下に詰める。
    /// </summary>
    private void StaffPawDown(int srcIndex)
    {
      // 消えてる肉球は対象外
      if (this.paws[srcIndex].IsIdle) return;

      // 肉球の移動先を調べる
      var srcCoord = CoordBy(srcIndex);
      var dstCoord = srcCoord;

      // 現在のY座標から下に向かって消えてない(Idleじゃない)肉球の一個上のY座標を求める
      for (int y = srcCoord.y - 1; 0 <= y; --y) 
      {
        if (!this.paws[IndexBy(srcCoord.x, y)].IsIdle) break;
        dstCoord.y = y;
      }

      // 移動先が同じならなにもしない
      if (srcCoord == dstCoord) return;

      // 肉球の入れ替え
      var dstIndex = IndexBy(dstCoord);
      Paw src = this.paws[srcIndex];
      Paw dst = this.paws[dstIndex];
      this.paws[dstIndex] = src;
      this.paws[srcIndex] = dst;
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ
    public void OnDebug()
    {
      this.ChainScore.OnDebug();
    }
#endif
  }
}


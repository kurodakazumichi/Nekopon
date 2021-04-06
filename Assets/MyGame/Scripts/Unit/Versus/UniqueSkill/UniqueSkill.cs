using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Versus.UniqueSkillExecutors;

namespace MyGame.Unit.Versus
{
  public class UniqueSkill : Unit<UniqueSkill.State>
  {
    /// <summary>
    /// スキル実行者のインターフェース
    /// </summary>
    public interface IExecutor
    {
      /// <summary>
      /// 発動
      /// </summary>
      void Fire(Player owner, Player target);

      /// <summary>
      /// 更新
      /// </summary>
      void Update();
    }

    /// <summary>
    /// 固有スキルの状態
    /// </summary>
    public enum State
    {
      Idle,
      ExpandX,
      ExpandY,
      CutIn,
      Execute,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 拡大速度
    /// </summary>
    private const float EXPAND_TIME = 0.1f;

    /// <summary>
    /// カットイン表示時間
    /// </summary>
    private const float CUTIN_TIME = 1f;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// カットインユニット
    /// </summary>
    private Mover.Glow cutin = null;

    /// <summary>
    /// スキルオーナー
    /// </summary>
    private Player owner = null;

    /// <summary>
    /// 対戦相手
    /// </summary>
    private Player target = null;

    /// <summary>
    /// 開始時のスケール
    /// </summary>
    private Vector3 startScale = Vector3.zero;

    /// <summary>
    /// 目標のスケール
    /// </summary>
    private Vector3 targetScale = Vector3.zero;

    /// <summary>
    /// スキル実行者
    /// </summary>
    private IExecutor executor = null;

    //-------------------------------------------------------------------------
    // プロパティ

    /// <summary>
    /// アイドルかどうか
    /// </summary>
    public bool IsIdle => this.state.StateKey == State.Idle;

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 各キャラクターのカットインスプライト
    /// </summary>
    public static Dictionary<int, Sprite> Sprites = new Dictionary<int, Sprite>();

    public static void Load(System.Action pre, System.Action done)
    {
      var rm = ResourceSystem.Instance;

      MyEnum.ForEach<Define.App.Cat>((type) => { 
        rm.Load<Sprite>($"CutIn.{type}.sprite", pre, done, (res) => { 
          Sprites.Add((int)type, res); 
        });
      });
    }

    public static void Unload()
    {
      var rm = ResourceSystem.Instance;

      MyEnum.ForEach<Define.App.Cat>((type) => { 
        rm.Unload($"CutIn.{type}.sprite");
      });
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // ユニット生成
      this.cutin = MyGameObject.Create<Mover.Glow>("CutIn", CacheTransform);

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.ExpandX, OnExpandXEnter, OnExpandXUpdate);
      this.state.Add(State.ExpandY, OnExpandYEnter, OnExpandYUpdate);
      this.state.Add(State.CutIn, OnCutInEnter, OnCutInUpdate, OnCutInExit);
      this.state.Add(State.Execute, OnExecuteEnter, OnExecuteUpdate, OnExecuteExit);
      this.state.SetState(State.Idle);
    }

    public void Setup(Define.App.UniqueSkill skillType)
    {
      // スキルタイプに合わせてExecutorクラスを生成
      switch(skillType) {
        case Define.App.UniqueSkill.Invincible:
          this.executor = new InvinsibleExecutor(Unlock, Done);
          break;
        case Define.App.UniqueSkill.Reflection:
          this.executor = new ReflectionExecutor(Unlock, Done);
          break;
        case Define.App.UniqueSkill.Recovery:
          this.executor = new RecoveryExecutor(Unlock, Done);
          break;
        case Define.App.UniqueSkill.Swap:
          this.executor = new SwapExecutor(Unlock, Done);
          break;
        default:
          this.executor = new InvinsibleExecutor(Unlock, Done);
          break;
      }
    }

    /// <summary>
    /// スキル使用にロックをかける
    /// </summary>
    private void Lock()
    {
      TimeSystem.Instance.Pause = true;
      SkillManager.Instance.IsLockUniqueSkill = true;
    }

    /// <summary>
    /// スキル使用のロックを解除する
    /// </summary>
    private void Unlock()
    {
      TimeSystem.Instance.Pause = false;
      SkillManager.Instance.IsLockUniqueSkill = false;
    }

    /// <summary>
    /// スキル処理を完了する
    /// </summary>
    private void Done()
    {
      this.state.SetState(State.Idle);
    }

    /// <summary>
    /// スキルを発動する
    /// </summary>
    public void Fire(Player owner, Player target)
    {
      // スキル使用者，対象を保持
      this.owner = owner;
      this.target = target;

      // カットインをセットアップ
      this.cutin.Setup(
        Sprites[(int)this.owner.catType],
        Define.Layer.Sorting.UI
      );

      // 使用したプレイヤーに合わせてテクスチャを反転させ、輝度を高くしておく
      this.cutin.MainFlipX = owner.Type == Define.App.Player.P1;
      this.cutin.Brightness = 1f;

      // スキル発動中は他のスキルを発動できないようにロックする
      Lock();

      // 開始
      this.state.SetState(State.ExpandX);
    }

    //-------------------------------------------------------------------------
    // ExpandX
    // カットイン画像を画面中心から画面幅いっぱいに引き延ばしながら表示

    private void OnExpandXEnter()
    {
      const float SCALE_Y = 0.01f;

      this.startScale  = new Vector3(0, SCALE_Y, 1f);
      this.targetScale = new Vector3(1f, SCALE_Y, 1f);

      this.cutin.CacheTransform.localScale
        = this.startScale;

      this.timer = 0;
    }

    private void OnExpandXUpdate()
    {
      var rate = this.timer / EXPAND_TIME;

      this.cutin.CacheTransform.localScale
        = Vector3.Lerp(this.startScale, this.targetScale, rate);

      UpdateTimer();

      if (EXPAND_TIME < this.timer) {
        this.state.SetState(State.ExpandY);
      }
    }

    //-------------------------------------------------------------------------
    // ExpandY
    // カットイン画像を画面高さいっぱいに引き延ばしながら表示

    private void OnExpandYEnter()
    {
      this.cutin.CacheTransform.localScale = this.targetScale;

      this.startScale = this.targetScale;
      this.targetScale.y = 1f;

      this.timer = 0;
    }

    private void OnExpandYUpdate()
    {
      var rate = this.timer / EXPAND_TIME;

      this.cutin.CacheTransform.localScale
        = Vector3.Lerp(this.startScale, this.targetScale, rate);

      UpdateTimer();

      if (EXPAND_TIME < this.timer) {
        this.state.SetState(State.CutIn);
      }
    }

    //-------------------------------------------------------------------------
    // CutIn
    // カットインを表示

    private void OnCutInEnter()
    {
      this.timer = 0;
      this.cutin.CacheTransform.localScale = this.targetScale;
    }

    private void OnCutInUpdate()
    {
      var rate = this.timer / CUTIN_TIME;

      const float SPEED = 5f;
      this.cutin.Brightness = 1f - Mathf.Min(1f, SPEED * rate);

      UpdateTimer();

      if (CUTIN_TIME < this.timer) {
        this.state.SetState(State.Execute);
      }
    }

    private void OnCutInExit()
    {
      this.cutin.Setup(null, "");
    }

    /// <summary>
    /// Skill用の時間を使用する
    /// </summary>
    protected override void UpdateTimer()
    {
      this.timer += TimeSystem.Instance.SkillDeltaTime;
    }

    //-------------------------------------------------------------------------
    // Execute

    private void OnExecuteEnter()
    {
      // スキル効果発動
      this.executor.Fire(this.owner, this.target);
    }

    private void OnExecuteUpdate()
    {
      // executorの中でスキル終了を呼ぶので、ここではUpdateを呼ぶだけでOK
      this.executor.Update();
    }

    private void OnExecuteExit()
    {

    }
  }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class UniqueSkill : Unit<UniqueSkill.State>
  {
    public enum State
    {
      Idle,
      Phase1,
      Phase2,
      Invoke,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// カットインユニット
    /// </summary>
    private Mover.Glow cutin = null;

    /// <summary>
    /// 固有スキルタイプ
    /// </summary>
    private Define.App.UniqueSkill type = default;

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

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// Spriteリソース
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
      this.state.Add(State.Phase1, OnPhase1Enter, OnPhase1Update);
      this.state.Add(State.Phase2, OnPhase2Enter, OnPhase2Update);
      this.state.Add(State.Invoke, OnInvokeEnter, OnInvokeUpdate, OnInvokeExit);
      this.state.SetState(State.Idle);
    }


    public void Setup(Define.App.UniqueSkill skillType)
    {
      this.type = skillType;
    }

    public void Fire(Player owner, Player target)
    {
      this.owner = owner;
      this.target = target;

      this.cutin.Setup(
        Sprites[(int)this.owner.catType],
        Define.Layer.Sorting.UI
      );

      this.cutin.MainFlipX = owner.Type == Define.App.Player.P1;
      this.cutin.Brightness = 1f;

      // 通常のタイマーを止める
      TimeSystem.Instance.TimeScale = 0f;

      SkillManager.Instance.IsLockUniqueSkill = true;

      this.state.SetState(State.Phase1);
    }

    //-------------------------------------------------------------------------
    // Phase1

    private void OnPhase1Enter()
    {
      const float SCALE_Y = 0.01f;

      this.startScale  = new Vector3(0, SCALE_Y, 1f);
      this.targetScale = new Vector3(1f, SCALE_Y, 1f);

      this.cutin.CacheTransform.localScale
        = this.startScale;

      this.timer = 0;
    }

    private void OnPhase1Update()
    {
      var rate = this.timer / 0.1f;

      this.cutin.CacheTransform.localScale
        = Vector3.Lerp(this.startScale, this.targetScale, rate);

      UpdateTimer();

      if (0.1f < this.timer) {
        this.state.SetState(State.Phase2);
      }
    }

    //-------------------------------------------------------------------------
    // Phase2

    private void OnPhase2Enter()
    {
      this.cutin.CacheTransform.localScale = this.targetScale;

      this.startScale = this.targetScale;
      this.targetScale.y = 1f;

      this.timer = 0;
    }

    private void OnPhase2Update()
    {
      var rate = this.timer / 0.1f;

      this.cutin.CacheTransform.localScale
        = Vector3.Lerp(this.startScale, this.targetScale, rate);

      UpdateTimer();

      if (0.1f < this.timer) {
        this.state.SetState(State.Invoke);
      }
    }

    private void OnInvokeEnter()
    {
      this.timer = 0;
      this.cutin.CacheTransform.localScale = this.targetScale;
    }

    private void OnInvokeUpdate()
    {
      var rate = this.timer / 1f;

      this.cutin.Brightness = Mathf.Max(0, 1f - 5f * rate);

      UpdateTimer();

      if (1f < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnInvokeExit()
    {
      this.cutin.Setup(null, "");
      SkillManager.Instance.IsLockUniqueSkill = false;
      TimeSystem.Instance.TimeScale = 1f;
    }

    /// <summary>
    /// Skill用の時間を使用する
    /// </summary>
    protected override void UpdateTimer()
    {
      this.timer += TimeSystem.Instance.SkillDeltaTime;
    }
  }

}


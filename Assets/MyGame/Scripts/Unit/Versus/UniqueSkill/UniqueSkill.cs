using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public class UniqueSkill : Unit<UniqueSkill.State>, IUniqueSkill
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


    //-------------------------------------------------------------------------
    // IUniqueSkill Interfaceの実装

    public void Setup(Define.App.UniqueSkill skillType, Player owner, Player target)
    {
      this.type = skillType;
      this.owner = owner;
      this.target = target;

      this.cutin.Setup(
        Sprites[(int)this.owner.catType],
        Define.Layer.Sorting.UI
      );
    }

    public void Fire()
    {
      this.state.SetState(State.Phase1);
    }

    //-------------------------------------------------------------------------
    // Phase1

    private void OnPhase1Enter()
    {
      this.cutin.CacheTransform.localScale
        = Vector3.zero;
      this.timer = 0;
    }

    private void OnPhase1Update()
    {
      var rate = this.timer / 0.1f;

      this.cutin.CacheTransform.localScale
        = Vector3.Lerp(Vector3.zero, new Vector3(-1f, 0.01f, 1), rate);

      UpdateTimer();

      if (0.1f < this.timer) {
        this.state.SetState(State.Phase2);
      }
    }

    //-------------------------------------------------------------------------
    // Phase2

    private void OnPhase2Enter()
    {
      this.timer = 0;
      this.cutin.CacheTransform.localScale = new Vector3(-1f, 0.01f, 1);
    }

    private void OnPhase2Update()
    {
      var rate = this.timer / 0.1f;

      this.cutin.CacheTransform.localScale
        = Vector3.Lerp(new Vector3(-1f, 0.01f, 1), Vector3.one, rate);

      UpdateTimer();

      if (0.1f < this.timer) {
        this.state.SetState(State.Invoke);
      }
    }

    private void OnInvokeEnter()
    {
      this.timer = 0;
      this.cutin.CacheTransform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnInvokeUpdate()
    {
      var rate = this.timer / 1f;
      UpdateTimer();

      if (1f < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnInvokeExit()
    {
      SkillManager.Instance.Release(this);
    }
  }

}


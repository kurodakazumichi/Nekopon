using UnityEngine;
using MyGame.Unit.Particle;

namespace MyGame.Unit.Effect
{
  public class Ghost : EffectBase<Ghost.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Haunted, // 幽霊が出る
      Leave,   // 立ち去る
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// 幽霊の数
    /// </summary>
    private const int GHOST_COUNT = 10;

    private const float HAUNTED_TIME = 1f;
    private const float LEAVE_TIME = 1f;

    /// <summary>
    /// 幽霊のパーティクル設定
    /// </summary>
    private static readonly Props GHOST_PROPS = new Props() { 
      IsSelfDestructive = false,
    };

    //-------------------------------------------------------------------------
    // メンバ変数

    private readonly IParticle[] ghosts = new IParticle[GHOST_COUNT];

    //-------------------------------------------------------------------------
    // Load, Unload

    /// <summary>
    /// 雫のテクスチャ
    /// </summary>
    public static Sprite Sprite1 = null;
    public static Sprite Sprite2 = null;

    public static void Load(System.Action pre, System.Action done)
    {
      var rs = ResourceSystem.Instance;
      rs.Load<Sprite>("Skill.Dar.01.sprite", pre, done, (res) => { Sprite1 = res; });
      rs.Load<Sprite>("Skill.Dar.02.sprite", pre, done, (res) => { Sprite2 = res; });
    }

    public static void Unload()
    {
      var rs = ResourceSystem.Instance;
      rs.Unload("Skill.Dar.01.sprite");
      rs.Unload("Skill.Dar.02.sprite");
      Sprite1 = null;
      Sprite2 = null;
    }

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Haunted, OnHauntedEnter, OnHauntedUpdate);
      this.state.Add(State.Leave, OnLeaveEnter, OnLeaveUpdate, OnLeaveExit);
      this.state.SetState(State.Idle);
    }

    //-------------------------------------------------------------------------
    // IEffectの実装

    public override void Setup()
    {
      var pm = ParticleManager.Instance;

      // 幽霊用のパーティクルを生成
      for(int i = 0; i < GHOST_COUNT; ++i) {
        this.ghosts[i] = pm.Create(ParticleManager.Type.Standard);
        this.ghosts[i].Setup(GHOST_PROPS);
        this.ghosts[i].Sprite = Sprite1;
      }
    }

    /// <summary>
    /// 発動
    /// </summary>
    public override void Fire(Vector3 position)
    {
      base.Fire(position);
      this.state.SetState(State.Haunted);
    }

    //-------------------------------------------------------------------------
    // Haunted
    // 幽霊が出現してぐるぐるまわる

    private void OnHauntedEnter() 
    { 
      Util.ForEach(this.ghosts, (ghost, _) => { 
        ghost.Fire(CacheTransform.position);
      });

      this.timer = 0;
    }

    private void OnHauntedUpdate() 
    {
      const float ROTATION_INTERVAL = 360f / GHOST_COUNT;
      const float RADIUS = 0.3f;

      Util.ForEach(this.ghosts, (ghost, index) => 
      { 
        var rotation = Quaternion.AngleAxis(this.timer * 45f + ROTATION_INTERVAL * index, Vector3.forward);

        ghost.CacheTransform.position
          = CacheTransform.position + rotation * Vector3.right * RADIUS;
      });

      UpdateTimer();

      if (HAUNTED_TIME < this.timer) {
        this.state.SetState(State.Leave);
      }
    }

    //-------------------------------------------------------------------------
    // Leave
    // 幽霊が去っていく
    
    private void OnLeaveEnter()
    {
      Util.ForEach(this.ghosts, (ghost, index) => { 
        ghost.Velocity = new Vector3(Random.Range(-1f, 1f), 1f, 0);
        ghost.IsSelfDestructive = true;
        ghost.LifeTime = LEAVE_TIME;
      });

      Action?.Invoke();
      this.timer = 0;
    }

    private void OnLeaveUpdate()
    {
      UpdateTimer();

      if (LEAVE_TIME < this.timer) {
        this.state.SetState(State.Idle);
      }
    }

    private void OnLeaveExit()
    {
      for(int i = 0; i < GHOST_COUNT; ++i) {
        this.ghosts[i] = null;
      }

      EffectManager.Instance.Release(EffectManager.Type.Ghost, this);
    }
  }
}
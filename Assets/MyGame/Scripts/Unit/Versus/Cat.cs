using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Define;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// 対戦中の猫
  /// </summary>
  public class Cat : Unit<Cat.State>
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State { 
      Idle,
      Setup,
      Animation,
    }

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// アニメーション間隔
    /// </summary>
    private const float INTERVAL = 0.5f;

    /// <summary>
    /// Sprite名の定義(定数扱い)
    /// </summary>
    private static readonly Dictionary<App.Cat, string[]> SPRITE_NAMES = new Dictionary<App.Cat, string[]>(){
      { App.Cat.Minchi, new string[] { "Usual01", "Usual02", "Damage01", "Attack01" }},
      { App.Cat.Nick  , new string[] { "Usual01", "Usual02", "Damage01", "Damage02", "Attack01", "Attack02" }},
      { App.Cat.Shiro , new string[] { "Usual01", "Usual02", "Damage01", "Damage02", "Attack01" }},
      { App.Cat.Tii   , new string[] { "Usual01", "Usual02", "Damage01", "Damage02", "Attack01" }},
    };


    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 猫のタイプ
    /// </summary>
    private App.Cat type = default;

    /// <summary>
    /// SpriteRenderer
    /// </summary>
    private SpriteRenderer spriteRenderer = null;

    /// <summary>
    /// アニメーション用スプライト
    /// </summary>
    private List<Sprite> sprites = null;
    private List<Sprite> usualSprites = new List<Sprite>();
    private List<Sprite> damageSprites = new List<Sprite>();
    private List<Sprite> attackSprites = new List<Sprite>();

    //-------------------------------------------------------------------------
    // ライフサイクル

    protected override void MyAwake()
    {
      // SpriteRendererをアタッチ
      this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();

      // 状態の構築
      this.state.Add(State.Idle);
      this.state.Add(State.Setup, OnSetupEnter);
      this.state.Add(State.Animation, OnAnimationEnter, OnAnimationUpdate);
      this.state.SetState(State.Idle);
    }

    public void Init(App.Cat type, bool flip)
    {
      this.type = type;

      // 反転するならスケールのXを-1にする
      CacheTransform.localScale = (flip)
        ? new Vector3(-1, 1, 1)
        : Vector3.one;

      StartCoroutine(Load());
    }

    protected override void OnMyDestory()
    {
      Unload();
    }

    private IEnumerator Load()
    {
      // ロード待機用
      var waitForCount = new WaitForCount();
      System.Action pre  = waitForCount.inc;
      System.Action done = waitForCount.dec;

      LoadCatSprites(pre, done, this.type, SPRITE_NAMES[this.type]);

      yield return waitForCount;
      this.state.SetState(State.Setup);
    }

    private void Unload()
    {
      UnloadCatSprites(this.type, SPRITE_NAMES[this.type]);
    }

    /// <summary>
    /// 猫の種類、スプライト名配列を元にスプライトをロードする
    /// </summary>
    private void LoadCatSprites(System.Action pre, System.Action done, App.Cat type, string[] names)
    {
      var rm = ResourceSystem.Instance;

      Util.ForEach(names, (name, _) => {
        rm.Load<Sprite>($"Cat.{type}.{name}.sprite", pre, done);
      });
    }

    /// <summary>
    /// 猫の種類、スプライト名配列を元にスプライトをアンロードする
    /// </summary>
    private void UnloadCatSprites(App.Cat type, string[] names)
    {
      var rm = ResourceSystem.Instance;

      Util.ForEach(names, (name, _) => {
        rm.Unload($"Cat.{type}.{name}.sprite");
      });
    }

    //-------------------------------------------------------------------------
    // ステートマシン

    private void OnSetupEnter()
    {
      this.usualSprites.Clear();
      this.damageSprites.Clear();
      this.attackSprites.Clear();

      switch(this.type) {
        case App.Cat.Minchi:
          this.usualSprites.Add(GetSpriteBy("Usual01"));
          this.usualSprites.Add(GetSpriteBy("Usual02"));
          this.damageSprites.Add(GetSpriteBy("Damage01"));
          this.damageSprites.Add(GetSpriteBy("Usual01"));
          this.attackSprites.Add(GetSpriteBy("Attack01"));
          this.attackSprites.Add(GetSpriteBy("Usual01"));
          break;
        case App.Cat.Nick:
          this.usualSprites.Add(GetSpriteBy("Usual01"));
          this.usualSprites.Add(GetSpriteBy("Usual02"));
          this.damageSprites.Add(GetSpriteBy("Damage01"));
          this.damageSprites.Add(GetSpriteBy("Damage02"));
          this.attackSprites.Add(GetSpriteBy("Attack01"));
          this.attackSprites.Add(GetSpriteBy("Attack02"));
          break;
        case App.Cat.Shiro:
          this.usualSprites.Add(GetSpriteBy("Usual01"));
          this.usualSprites.Add(GetSpriteBy("Usual02"));
          this.damageSprites.Add(GetSpriteBy("Damage01"));
          this.damageSprites.Add(GetSpriteBy("Damage02"));
          this.attackSprites.Add(GetSpriteBy("Attack01"));
          this.attackSprites.Add(GetSpriteBy("Usual01"));
          break;
        case App.Cat.Tii:
          this.usualSprites.Add(GetSpriteBy("Usual01"));
          this.usualSprites.Add(GetSpriteBy("Usual02"));
          this.damageSprites.Add(GetSpriteBy("Damage01"));
          this.damageSprites.Add(GetSpriteBy("Damage02"));
          this.attackSprites.Add(GetSpriteBy("Attack01"));
          this.attackSprites.Add(GetSpriteBy("Usual01"));
          break;
      }

      this.sprites = this.usualSprites;
      this.state.SetState(State.Animation);
    }

    private void OnAnimationEnter()
    {
      this.timer = 0;
    }

    private void OnAnimationUpdate()
    {
      if (this.sprites == null) return;

      int index = (int)(this.timer / INTERVAL) % this.sprites.Count;
      this.spriteRenderer.sprite = this.sprites[index];

      this.timer += TimeSystem.Instance.DeltaTime;
    }

    //-------------------------------------------------------------------------
    // スプライト関連

    public void ToUsual()
    {
      this.sprites = this.usualSprites;
      this.timer = 0;
    }

    public void ToAttack()
    {
      this.sprites = this.attackSprites;
      this.timer = 0;
    }

    public void ToDamage()
    {
      this.sprites = this.damageSprites;
      this.timer = 0;
    }

    private Sprite GetSpriteBy(string name)
    {
      var rm = ResourceSystem.Instance;
      return rm.GetCache<Sprite>($"Cat.{this.type}.{name}.sprite");
    }

  }
}


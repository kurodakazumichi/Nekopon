using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  /// <summary>
  /// PlayerStatusインターフェース
  /// </summary>
  public interface IPlayerStatus
  {
    ILimitedFloat HP { get; }
    ILimitedFloat MP(Define.App.Attribute attribute);
    ILimitedFloat AP { get; }
    LimitedFloat Attack { get; }
    LimitedFloat Damage { get; }
  }

  /// <summary>
  /// Plyaer Statusクラス
  /// </summary>
  public class PlayerStatus:IPlayerStatus
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// HP
    /// </summary>
    public LimitedFloat Hp { get; private set; } = new LimitedFloat();
    public ILimitedFloat HP => this.Hp;

    /// <summary>
    /// MP
    /// </summary>
    private Dictionary<Define.App.Attribute, LimitedFloat> mp = new Dictionary<Define.App.Attribute, LimitedFloat>();
    public ILimitedFloat MP(Define.App.Attribute attribute) { return this.mp[attribute]; }

    /// <summary>
    /// AP
    /// </summary>
    public LimitedFloat Ap { get; private set; } = new LimitedFloat();
    public ILimitedFloat AP => this.Ap;

    /// <summary>
    /// ダメージゲージ用
    /// </summary>
    public LimitedFloat Dp { get; private set; } = new LimitedFloat();

    /// <summary>
    /// 攻撃(与える予定のダメージ)
    /// </summary>
    public LimitedFloat Attack { get; private set; } = new LimitedFloat();

    /// <summary>
    /// 受ける予定のダメージ
    /// </summary>
    public LimitedFloat Damage { get; private set; } = new LimitedFloat();

    //-------------------------------------------------------------------------
    // publicメソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlayerStatus()
    {
      MyEnum.ForEach<Define.App.Attribute>((attribute) => {
        this.mp.Add(attribute, new LimitedFloat());
      });
    }

    /// <summary>
    /// 設定を元にStatusを構築
    /// </summary>
    public PlayerStatus Init(IPlayerConfig config)
    {
      // HP
      this.Hp.Init(config.MaxHp, config.MaxHp);

      // MP
      MyEnum.ForEach<Define.App.Attribute>((attribute) => 
      {
        Mp(attribute).Init(0, config.GetMaxMp(attribute));
      });

      // DP
      this.Dp.Init(0, config.MaxHp);

      // AP
      this.Ap.Init(0, config.MaxAp);

      // 攻撃とダメージ
      this.Attack.Init(0, Define.Versus.MAX_DAMAGE);
      this.Damage.Init(0, Define.Versus.MAX_DAMAGE);

      return this;
    }

    /// <summary>
    /// MP
    /// </summary>
    public LimitedFloat Mp(Define.App.Attribute attribute)
    {
      return this.mp[attribute];
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
      // ダメージがあるならHPを減らし続ける
      if (Damage.IsEmpty) return;

      float damage = Define.Versus.DAMAGE_PER_SEC * TimeSystem.Instance.DeltaTime;

      Dp.Now = Damage.Now;
      Hp.Now     -= damage;
      Damage.Now -= damage;
    }


#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    public void OnDebug()
    {
      using (new GUILayout.VerticalScope(GUI.skin.box)) 
      {
        GUILayout.Label($"HP:{Hp.Now}");

        Util.ForEach(this.mp, (attr, mp) => {
          GUILayout.Label($"MP_{attr}:{mp.Now}");
        });

        GUILayout.Label($"AP:{Ap.Now}");
        GUILayout.Label($"Attack:{Attack.Now}");
        GUILayout.Label($"Damage:{Damage.Now}");
      }
    }
#endif
  }
}
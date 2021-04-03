using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit.Versus
{
  public partial class Player
  {
    /// <summary>
    /// Plyaer Statusクラス
    /// </summary>
    private class Status
    {
      //-------------------------------------------------------------------------
      // メンバ変数

      /// <summary>
      /// HP
      /// </summary>
      private LimitedFloat hp { get; set; } = new LimitedFloat();
      
      /// <summary>
      /// MP
      /// </summary>
      private Dictionary<Define.App.Attribute, LimitedFloat> mp = new Dictionary<Define.App.Attribute, LimitedFloat>();
      
      /// <summary>
      /// AP
      /// </summary>
      private LimitedFloat ap { get; set; } = new LimitedFloat();

      /// <summary>
      /// ダメージゲージ用
      /// </summary>
      private LimitedFloat dp { get; set; } = new LimitedFloat();

      /// <summary>
      /// 攻撃(与える予定のダメージ)
      /// </summary>
      private LimitedFloat power { get; set; } = new LimitedFloat();

      /// <summary>
      /// 受ける予定のダメージ
      /// </summary>
      private LimitedFloat damage { get; set; } = new LimitedFloat();

      //-------------------------------------------------------------------------
      // プロパティ

      /// <summary>
      /// 現在HPの比率
      /// </summary>
      public float HpRate => hp.Rate;

      /// <summary>
      /// 現在APの比率
      /// </summary>
      public float ApRate => ap.Rate;

      /// <summary>
      /// 現在ダメージの比率
      /// </summary>
      public float DpRate => dp.Rate;

      /// <summary>
      /// 攻撃の強さを表す比率：パワー / 最大ダメージ(1は超えない)
      /// </summary>
      public float PowerRate => Mathf.Min(1f, this.power.Now / dp.Max);

      //-------------------------------------------------------------------------
      // メソッド

      /// <summary>
      /// コンストラクタ
      /// </summary>
      public Status()
      {
        MyEnum.ForEach<Define.App.Attribute>((attribute) => {
          this.mp.Add(attribute, new LimitedFloat());
        });
      }

      /// <summary>
      /// 設定を元にStatusを構築
      /// </summary>
      public Status Init(IPlayerConfig config)
      {
        // HP
        this.hp.Init(config.MaxHp, config.MaxHp);

        // MP
        MyEnum.ForEach<Define.App.Attribute>((attribute) =>
        {
          this.mp[attribute].Init(0, config.GetMaxMp(attribute));
        });

        // DP
        this.dp.Init(0, config.MaxHp);

        // AP
        this.ap.Init(0, config.MaxAp);

        // 攻撃とダメージ
        this.power.Init(0, Define.Versus.MAX_DAMAGE);
        this.damage.Init(0, Define.Versus.MAX_DAMAGE);

        return this;
      }

      /// <summary>
      /// MPを取得
      /// </summary>
      public float GetMp(Define.App.Attribute attribute)
      {
        return this.mp[attribute].Now;
      }

      /// <summary>
      /// MPを加算
      /// </summary>
      public void AddMp(Define.App.Attribute attribute, float mp)
      {
        this.mp[attribute].Now += mp;
      }

      /// <summary>
      /// ステータスの更新、ダメージがある限り徐々に体力を減らしていく
      /// </summary>
      public void Update()
      {
        // ダメージがないなら更新しないで抜ける
        if (this.damage.IsEmpty) 
        {
          if (!this.dp.IsEmpty) this.dp.Empty();
          return;
        }

        // ダメージがあるなら体力を減らし続ける
        float damage = Define.Versus.DAMAGE_PER_SEC * TimeSystem.Instance.DeltaTime;

        dp.Now = this.damage.Now;
        hp.Now -= damage;
        this.damage.Now -= damage;
      }

      public void AddPower(float power)
      {
        this.power.Now += power;
      }

      /// <summary>
      /// パワーを抽出する、このメソッドを呼ぶとstatusのパワーは0になる。
      /// </summary>
      public float ExtractPower()
      {
        var power = this.power.Now;
        this.power.Empty();
        return power;
      }

      /// <summary>
      /// ダメージを受ける
      /// </summary>
      public void TakeDamage(float damage)
      {
        this.damage.Now += damage;
      }

      /// <summary>
      /// 最大HPのrate%の回復をする
      /// </summary>
      public void Recover(float rate)
      {
        // 最大HPと割合から回復量を算出
        var recovery = this.hp.Max * rate;

        // HPの差分(リアルダメージ)
        var real_damage = this.hp.Diff;
        
        // HP回復
        this.hp.Now += recovery;

        // ダメージを回復しても、まだ回復量が余る場合
        if (real_damage < recovery) {
          this.damage.Now -= (recovery - real_damage);
        }
      }

#if _DEBUG
      //-------------------------------------------------------------------------
      // デバッグ

      public void OnDebug()
      {
        using (new GUILayout.VerticalScope(GUI.skin.box)) {
          GUILayout.Label($"HP:{hp.Now}");

          Util.ForEach(this.mp, (attr, mp) => {
            GUILayout.Label($"MP_{attr}:{mp.Now}");
          });

          GUILayout.Label($"AP:{ap.Now}");
          GUILayout.Label($"Attack:{power.Now}");
          GUILayout.Label($"Damage:{damage.Now}");
        }
      }
#endif
    }
  }

}
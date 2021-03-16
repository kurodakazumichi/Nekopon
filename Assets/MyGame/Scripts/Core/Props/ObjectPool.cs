using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace MyGame
{
  /// <summary>
  /// プール可能なインターフェース
  /// </summary>
  public interface IPoolable
  {
    /// <summary>
    /// アクティブを設定可能
    /// </summary>
    void SetActive(bool isActive);

    /// <summary>
    /// アクティブ状態を取得可能
    /// </summary>
    bool IsActiveSelf { get; }
  }

  /// <summary>
  /// Object Pool
  /// ObjectPoolはオブジェクトを生成するためのGenerator関数(System.Func)を必ず設定すること
  /// あまりパフォーマンスを考えてないので後々改良したい
  /// </summary>
  public class ObjectPool<T> where T : class, IPoolable
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// オブジェクト格納プール
    /// </summary>
    private List<T> pool = new List<T>();

    /// <summary>
    /// オブジェクト生成用メソッド(外部からかならず設定すること)
    /// </summary>
    private System.Func<T> Generator = null;

    //-------------------------------------------------------------------------
    // メソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ObjectPool()
    {
    }

    /// <summary>
    /// オブジェクト生成用関数を設定する
    /// </summary>
    public void SetGenerator(System.Func<T> func)
    {
      this.Generator = func;
    }

    /// <summary>
    /// 予約
    /// </summary>
    public void Reserve(int count)
    {
      // 警告ログ
      WarnGeneratorLog();

      for (int i = 0; i < count; ++i) {
        var obj = Generator();
        obj.SetActive(false);
        this.pool.Add(obj);
      }
    }

    /// <summary>
    /// オブジェクトを生成、プールに非アクティブなオブジェクトがあれば再利用する
    /// </summary>
    public T Create()
    {
      // 警告ログ
      WarnGeneratorLog();

      var obj = GetEnactive();

      if (obj == null) 
      {
        obj = Generator();
        this.pool.Add(obj);
      }

      obj.SetActive(true);
      return obj;
    }

    /// <summary>
    /// オブジェクトを非アクティブにする
    /// </summary>
    public void Release(T obj)
    {
      obj.SetActive(false);
    }
    
    /// <summary>
    /// プール内から非アクティブなオブジェクトを探す
    /// </summary>
    /// <returns></returns>
    private T GetEnactive()
    {
      for (int i = 0, count = this.pool.Count; i < count; ++i) {
        if (!this.pool[i].IsActiveSelf) return this.pool[i];
      }
      return null;
    }

    /// <summary>
    /// ObjectPoolはGeneratorを設定することが前提なので、設定されてない状態で
    /// Generatorが必要な処理を呼び出した場合に警告を表示する。
    /// </summary>
    [Conditional("_DEBUG")]
    private void WarnGeneratorLog()
    {
      if (this.Generator != null) return;
      Debug.Logger.Warn($"ObjectPool<{typeof(T).Name}>にGeneratorが設定されていません。");
    }
  }

}
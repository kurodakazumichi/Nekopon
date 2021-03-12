using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// Object Pool
  /// あまりパフォーマンスを考えてないので後々改良する想定
  /// </summary>
  public class ObjectPool<T> where T : MonoBehaviour
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// オブジェクト格納プール
    /// </summary>
    private List<T> pool = new List<T>();

    /// <summary>
    /// オブジェクト生成用メソッド
    /// </summary>
    private System.Func<T> Generator = null;

    //-------------------------------------------------------------------------
    // メソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ObjectPool()
    {
      // nullを指定するとデフォルトのジェネレーターが設定される
      SetGenerator(null);
    }

    /// <summary>
    /// オブジェクト生成用関数を設定する
    /// </summary>
    public void SetGenerator(System.Func<T> func)
    {
      this.Generator = func?? DefaultGenerator;
    }

    /// <summary>
    /// オブジェクトを生成、プールに非アクティブなオブジェクトがあれば再利用する
    /// </summary>
    public T Create()
    {
      var obj = GetEnactive();

      if (obj == null) 
      {
        obj = Generator();
        this.pool.Add(obj);
      }

      obj.gameObject.SetActive(true);
      return obj;
    }

    /// <summary>
    /// オブジェクトを非アクティブにする
    /// </summary>
    public void Release(T obj)
    {
      obj.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// プール内から非アクティブなオブジェクトを探す
    /// </summary>
    /// <returns></returns>
    private T GetEnactive()
    {
      for (int i = 0, count = this.pool.Count; i < count; ++i) {
        if (!this.pool[i].gameObject.activeSelf) return this.pool[i];
      }
      return null;
    }

    /// <summary>
    /// デフォルトのジェネレーター
    /// </summary>
    private T DefaultGenerator()
    {
      var go = new GameObject("Game Object");
      return go.AddComponent<T>();
    }
  }

}
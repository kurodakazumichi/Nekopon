using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using MyGame.Define;

namespace MyGame
{
  public class DebugSystem : SingletonMonoBehaviour<DebugSystem>
  {
    /// <summary>
    /// デバッグマネージャーに対称を登録
    /// </summary>
    /// <param name="mono"></param>
    [Conditional("_DEBUG")]
    public void Regist(MyMonoBehaviour mono)
    {
#if _DEBUG
      this.monos.Add(mono.GetType().Name, mono);
#endif
    }

    /// <summary>
    /// デバッグマネージャーから対象を破棄
    /// </summary>
    [Conditional("_DEBUG")]
    public void Discard(MyMonoBehaviour mono)
    {
#if _DEBUG
      string name = mono.GetType().Name;

      if (this.monos.ContainsKey(mono.GetType().Name)) {
        this.monos.Remove(name);
      }
#endif
    }

#if _DEBUG
    /// <summary>
    /// デバッグ対称
    /// </summary>
    private Dictionary<string, MyMonoBehaviour> monos = new Dictionary<string, MyMonoBehaviour>();

    private MyMonoBehaviour current = null;

    private bool isShow = false;

    protected override void MyUpdate()
    {
      if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D)) {
        this.isShow = !this.isShow;
      }
    }

    private Vector2 scrollPosition = Vector2.zero;

    private void OnGUI()
    {
      if (!isShow) return;
     
      using (var sv = new GUILayout.ScrollViewScope(this.scrollPosition)) {

        using (new GUILayout.HorizontalScope(GUI.skin.box)) {
          this.scrollPosition = sv.scrollPosition;

          using (new GUILayout.VerticalScope(GUILayout.Width(100))) {
            Util.ForEach(this.monos, (name, mono) => {
              if (GUILayout.Button(name)) {
                this.current = mono;
              }
            });
          }
          using (new GUILayout.VerticalScope(GUILayout.Width(Screen.width - 150))) {
            if (this.current) {
              this.current.OnDebug();
            }
          }
        }
      }
    }
#endif
  }

}

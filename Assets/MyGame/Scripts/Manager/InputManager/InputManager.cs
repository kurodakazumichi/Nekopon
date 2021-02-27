using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.InputManagement;

namespace MyGame
{
  
  public class InputManager : SingletonMonobehaviour<InputManager>
  {
    private GamePad[] pads = new GamePad[2];

    protected override void MyStart()
    {
      this.pads[0] = new GamePad(0);
      //this.pads[1] = new GamePad(1);
    }

    
    protected override void MyUpdate()
    {
      this.pads[0].Update();
      for(int i = 0; i < this.pads.Length; ++i) {
        //this.pads[i].Update();
      }
    }

#if _DEBUG
    private void OnGUI()
    {
      this.pads[0].OnGUIDebug();
    }
#endif
  }

}

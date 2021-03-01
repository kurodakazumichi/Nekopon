using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Unit
{
  public class Unit<TState> : MyMonoBehaviour where TState:System.Enum
  {
    protected StateMachine<TState> state = new StateMachine<TState>();

    protected override void Start()
    {
      MyStart();
    }

    protected override void Update()
    {
      MyUpdate();
    }

    protected override void MyUpdate()
    {
      this.state.Update();
    }
  }
}

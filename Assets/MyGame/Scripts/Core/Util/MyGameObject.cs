using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public static class MyGameObject
  {
    public static T Create<T>(string name, Transform parent) where T : Component
    {
      var go = new GameObject(name);
      go.transform.parent = parent;
      return go.AddComponent<T>();
    }
  }
}
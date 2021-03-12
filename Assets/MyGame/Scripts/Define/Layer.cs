using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Define
{
  public static class Layer
  {
    public static class Sorting
    {
      public const string Background = "Background";
      public const string Default = "Default";
      public const string Effect = "Effect";
      public const string UI = "UI";
    }

    public enum Order
    {
      Layer00 = 0,
      Layer10 = 10,
      Layer20 = 20,
      Layer30 = 30,
      Layer40 = 40,
      Layer50 = 50,
      Layer60 = 60,
      Layer70 = 70,
      Layer80 = 80,
      Layer90 = 90,
    }
  }
}
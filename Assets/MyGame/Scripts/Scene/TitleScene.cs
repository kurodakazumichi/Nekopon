using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MyGame.Scene
{
  public class TitleScene : SceneBase
  {
    private GameObject rTitleLogo = null;

    protected override void MyStart()
    {
      Addressables.LoadAssetAsync<GameObject>("TitleLogo").Completed += op => { this.rTitleLogo = op.Result; };
    }

    private bool isCreated = false;
    protected override void MyUpdate()
    {
      if (this.rTitleLogo != null && this.isCreated == false) {
        Instantiate(this.rTitleLogo);
        this.isCreated = true;

      }
    }
  }
}
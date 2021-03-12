using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame
{
  public class SoundSystem : SingletonMonoBehaviour<SoundSystem>
  {
    private AudioSource bgmSource = null;
    private AudioSource seSource = null;

    /// <summary>
    /// BGMを再生
    /// </summary>
    public void PlayBGM(string address, bool loop = true)
    {
      var audio = GetAudio(address);

      if (audio == null) {
        Debug.Logger.Warn($"AudioClip is not loaded. address = {address}");
        return;
      }

      this.bgmSource.clip = audio;
      this.bgmSource.loop = loop;
      this.bgmSource.Play();
    }

    /// <summary>
    /// BGMの再生を止める
    /// </summary>
    public void StopBGM()
    {
      this.bgmSource.Stop();
    }

    /// <summary>
    /// SEを再生する
    /// </summary>
    public void PlaySE(string address)
    {
      var audio = GetAudio(address);

      if (audio == null) {
        Debug.Logger.Warn($"AudioClip is not loaded. address = {address}");
        return;
      }

      this.seSource.PlayOneShot(audio);
    }

    private AudioClip GetAudio(string address)
    {
      return ResourceSystem.Instance.GetCache<AudioClip>(address);
    }

    protected override void MyStart()
    {
      this.bgmSource = this.gameObject.AddComponent<AudioSource>();
      this.bgmSource.playOnAwake = false;
      this.seSource = this.gameObject.AddComponent<AudioSource>();
      this.seSource.playOnAwake = false;
    }
  }
}
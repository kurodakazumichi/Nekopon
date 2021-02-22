using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame
{
  public class SoundManager : SingletonMonobehaviour<SoundManager>
  {
    private AudioSource bgmSource = null;
    private AudioSource seSource = null;

    private Dictionary<string, AudioClip> audios = new Dictionary<string, AudioClip>();

    public void Load(string address, Action pre, Action done) 
    {
      // 既に指定されたリソースがあればロードしない
      if (this.audios.ContainsKey(address)) return;

      ResourceManager.Instance.Load<AudioClip>(
        address, pre, (obj) => { this.audios[address] = obj; }, done
      );
    }

    public void PlayBGM(string address, bool loop = true)
    {
      if (!HasAudioClip(address)) {
#if _DEBUG
        Debug.LogWarning($"AudioClip is not exists. address = {address}");
#endif
        return;
      }

      this.bgmSource.clip = this.audios[address];
      this.bgmSource.loop = loop;
      this.bgmSource.Play();
    }

    public void PlaySE(string address)
    {
      if (!HasAudioClip(address)) {
#if _DEBUG
        Debug.LogWarning($"AudioClip is not exists. address = {address}");
#endif
        return;
      }

      this.seSource.PlayOneShot(this.audios[address]);
    }

    private bool HasAudioClip(string address)
    {
      if (!this.audios.ContainsKey(address)) return false;
      if (this.audios[address] == null) return false;
      return true;
    }

    protected override void MyStart()
    {
      this.bgmSource = this.gameObject.AddComponent<AudioSource>();
      this.bgmSource.playOnAwake = false;
      this.seSource = this.gameObject.AddComponent<AudioSource>();
      this.seSource.playOnAwake = false;
    }

    protected override void MyUpdate()
    {
      if (Input.GetKeyDown(KeyCode.A)) {
        PlayBGM("BGM_001");
      }
      if (Input.GetKeyDown(KeyCode.B)) {
        PlayBGM("BGM_002");
      }

    }
  }
}
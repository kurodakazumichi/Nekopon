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

    /// <summary>
    /// サウンドリソースのロード
    /// </summary>
    /// <param name="address">AddressableAssetsで設定したアドレス</param>
    /// <param name="pre">ロード前に呼ばれるコールバック</param>
    /// <param name="done">ロード後に呼ばれるコールバック</param>
    public void Load(string address, Action pre, Action done) 
    {
      // 既に指定されたリソースがあればロードしない
      if (this.audios.ContainsKey(address)) return;

      ResourceManager.Instance.Load<AudioClip>(
        address, pre, (obj) => { this.audios[address] = obj; }, done
      );
    }

    /// <summary>
    /// BGMを再生
    /// </summary>
    public void PlayBGM(string address, bool loop = true)
    {
      if (!HasAudioClip(address)) {
        Debug.Logger.Warn($"AudioClip is not exists. address = {address}");
        return;
      }

      this.bgmSource.clip = this.audios[address];
      this.bgmSource.loop = loop;
      this.bgmSource.Play();
    }

    public void StopBGM()
    {
      this.bgmSource.Stop();
    }

    public void PlaySE(string address)
    {
      if (!HasAudioClip(address)) {
        Debug.Logger.Warn($"AudioClip is not exists. address = {address}");
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
  }
}
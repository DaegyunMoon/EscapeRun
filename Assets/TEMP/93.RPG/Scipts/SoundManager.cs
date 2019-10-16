using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource bgmSource;
    public List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioClip> audioClips = new List<AudioClip>();

    private void Awake()
    {
        instance = this;
        bgmSource = GetComponent<AudioSource>();
        bgmSource.loop = true;
        AddAudioSource(audioClips.Count);
    }
    private void Update()
    {
        if (GameControl.instance.MyGameState == GameControl.GameState.Playing)
        {
            if(!bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
        }
        else
        {
            bgmSource.Stop();
        }
    }
    //Add audio sources
    public static void AddAudioSource(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject audio = new GameObject("AudioSource", typeof(AudioSource));
            audio.transform.SetParent(instance.transform);
            AudioSource source = audio.GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = instance.audioClips[i];
            source.volume = 0.7f;
            instance.audioSources.Add(source);
        }
    }
    public void PlayEachSound(int index, bool isBool)
    {
        AudioSource source = instance.audioSources[index];
        source.loop = isBool;
        if (instance.audioSources.Count > 0)
        {
            source.Play();
        }
    }
    public void PlaySound(int index, bool isLoop)
    {
        if (instance.audioSources.Count > 0)
        {
            audioSources[index].loop = isLoop;
            if(!audioSources[index].isPlaying)
            {
                audioSources[index].Play();
            }
        }
    }
    public void StopSound(int index)
    {
        if (instance.audioSources.Count > 0)
        {
            if (audioSources[index].isPlaying)
            {
                audioSources[index].Stop();
            }
        }
    }
}

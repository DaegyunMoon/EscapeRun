﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool bgmEnable;
    public bool fxEnable;
    [Range(0, 1)]
    public float fxVolume = 1.0f;

    public GameObject soundOff;
    public GameObject soundOn;
    public GameObject bgmOff;
    public GameObject bgmOn;
    public AudioSource bgmSource;

    [Header("Player")]
    public AudioClip sprint;
    public AudioClip run;
    public AudioClip walk;
    public AudioClip walkWater;
    public AudioClip dive;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip getItem;
    public AudioClip hurt;
    public AudioClip zombieWalk;
    public AudioClip[] zombieScream = new AudioClip[3];
    public AudioClip levelUp;
    public AudioClip pauseSound;
    public AudioClip overSound;

    public static SoundManager instance;

    // Use thias for initialization
    void Awake()
    {
        instance = FindObjectOfType<SoundManager>();

        if(!PlayerPrefs.HasKey("FxCheck"))
        {
            PlayerPrefs.SetFloat("FxCheck", 1);
        }
        if (!PlayerPrefs.HasKey("BgmCheck"))
        {
            PlayerPrefs.SetFloat("BgmCheck", 1);
        }

        if (PlayerPrefs.GetFloat("FxCheck") == 0)
        {
            fxEnable = false;
            Debug.Log("FxCheck == 0");
            soundOff.SetActive(true);
            soundOn.SetActive(false);
        }
        else
        {
            fxEnable = true;
            Debug.Log("FxCheck == 1");
            soundOff.SetActive(false);
            soundOn.SetActive(true);
        }

        if (PlayerPrefs.GetFloat("BgmCheck") == 0)
        {
            bgmEnable = false;
            Debug.Log("BgmCheck == 0");
            bgmOff.SetActive(true);
            bgmOn.SetActive(false);
        }
        else
        {
            bgmEnable = true;
            Debug.Log("BgmCheck == 1");
            bgmOff.SetActive(false);
            bgmOn.SetActive(true);
        }
    }
    void Start()
    {
        PlayBGM();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void PlaySound(AudioClip clip, Vector3 point)
    {
        if (clip && fxEnable)
        {
            AudioSource.PlayClipAtPoint(clip, point);
        }
    }
    public void PlaySound(AudioSource audioSource, AudioClip clip, Vector3 point)
    {
        if (clip && fxEnable)
        {
            audioSource.clip = clip;
            audioSource.transform.position = point;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
    public void StopSound(AudioSource audioSource)
    {
        audioSource.clip = null;
        audioSource.Stop();
    }
    public void PlayBGM()
    {
        if (bgmSource.clip)
        {
            bgmSource.loop = true;
            bgmSource.volume = 0.8f;
            if (!bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
        }
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }
    public void ToggleFxOn()
    {
        fxEnable = true;
        PlayerPrefs.SetFloat("FxCheck", 1);
        Debug.Log("FxCheck = 1");
    }
    public void ToggleFxOff()
    {
        fxEnable = false;
        PlayerPrefs.SetFloat("FxCheck", 0);
        Debug.Log("FxCheck = 0");
    }
    public void ToggleBGMOn()
    {
        bgmEnable = true;
        PlayBGM();
        PlayerPrefs.SetFloat("BgmCheck", 1);
        Debug.Log("BgmCheck = 1");
    }
    public void ToggleBGMOff()
    {
        bgmEnable = false;
        StopBGM();
        PlayerPrefs.SetFloat("BgmCheck", 0);
        Debug.Log("BgmCheck = 0");
    }
}

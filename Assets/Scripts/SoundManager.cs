using System.Collections;
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

    [Header("InGame")]
    public AudioClip Walking;
    public AudioClip WalkingWater;
    public AudioClip Running;
    public AudioClip Jump;
    public AudioClip Falling;
    public AudioClip GetItem;
    public AudioClip LevelUp;
    public AudioClip PauseSound;
    public AudioClip OverSound;
    public AudioClip DeathVoice;

    // Use thias for initialization
    void Awake()
    {
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

    // Update is called once per frame
    void Update()
    {

    }
    public void PlaySound(AudioClip clip)
    {
        if (clip && fxEnable)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        }
    }
    public void PlaySound(AudioClip clip, float volume)
    {
        if (clip && fxEnable)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
            //Mathf.Clamp(this.fxVolume * volMultiplier, 0.05f, 1.0f);
        }
    }
    public void PlayBGM(AudioClip clip)
    {
        if (clip && bgmEnable)
        {
            bgmSource.clip = clip;
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
        if (bgmEnable)
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }
        }
    }
    public void ToggleFxCheck()
    {
        if (fxEnable)
        {
            fxEnable = false;
            PlayerPrefs.SetFloat("FxCheck", 0);
            Debug.Log("FxCheck = 0");
        }
        else
        {
            fxEnable = true;
            PlayerPrefs.SetFloat("FxCheck", 1);
            Debug.Log("FxCheck = 1");
        }
    }
    public void ToggleBGMCheck()
    {
        if (bgmEnable)
        {
            bgmEnable = false;
            PlayerPrefs.SetFloat("BgmCheck", 0);
            Debug.Log("BgmCheck = 0");
        }
        else
        {
            bgmEnable = true;
            PlayerPrefs.SetFloat("BgmCheck", 1);
            Debug.Log("BgmCheck = 1");
        }
    }
}

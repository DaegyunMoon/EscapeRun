using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public enum GameState
    {
        Start,
        Playing,
        Paused,
        Over
    }

    private GameState gameState;
    public static GameControl instance;
    public GameObject StartPanel;
    public GameObject PausePanel;
    public GameObject OverPanel;

    public GameState MyGameState
    {
        get
        {
            return gameState;
        }
        set
        {
            MyGameState = value;
        }
    }
    void Awake()
    {
        instance = this;
        gameState = GameState.Start;
    }
    private void Update()
    {
        OnKeyLisnter();
        CheckState();
    }
    void OnKeyLisnter()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            if(gameState == GameState.Start)
            {
                gameState = GameState.Playing;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(WaitForLoadScene(0.5f, "93.RPG/Scenes/MainScene"));
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (gameState == GameState.Playing)
            {
                SoundManager.instance.PlaySound(13, false); // PauseSound 재생
                TimeManager.instance.Pause();
                gameState = GameState.Paused;
            }
            else if(gameState == GameState.Paused)
            {
                SoundManager.instance.PlaySound(14, false); // ResumeSound 재생
                TimeManager.instance.Resume();
                gameState = GameState.Playing;
            }
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            if(gameState == GameState.Over)
            {
                StartCoroutine(WaitForLoadScene(0.5f, "93.RPG/Scenes/FirstScene"));
            }
        }
    }
    void CheckState()
    {
        switch(gameState)
        {
            case GameState.Start:
                StartPanel.SetActive(true);
                PausePanel.SetActive(false);
                OverPanel.SetActive(false);
                break;
            case GameState.Playing:
                StartPanel.SetActive(false);
                PausePanel.SetActive(false);
                OverPanel.SetActive(false);
                break;
            case GameState.Paused:
                StartPanel.SetActive(false);
                PausePanel.SetActive(true);
                OverPanel.SetActive(false);
                break;
            case GameState.Over:
                StartPanel.SetActive(false);
                PausePanel.SetActive(false);
                OverPanel.SetActive(true);
                break;
        }
    }
    private IEnumerator WaitForLoadScene(float waitTime, string sceneName)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }
    public void SetGameState(int stateNum)
    {
        gameState = (GameState)stateNum;
    }
}

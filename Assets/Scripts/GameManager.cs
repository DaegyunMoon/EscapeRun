using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState { Play, Pause }
public class GameManager : MonoBehaviour
{
    private int level;
    private int score;
    private int acquiredItem;
    private double thresholdTime = 15;
    public float time;
    public GameState gameState;
    public PlayerControl player;
    public Text scoreText;
    public Text countdownText;
    public Text levelText;
    public Slider hpbar;
    public GameObject countDownPanel;
    public GameObject settingPanel;
    private ZombieSpawner zombieSpawner;

    public static GameManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = FindObjectOfType<GameManager>();
        zombieSpawner = FindObjectOfType<ZombieSpawner>();
        time = -3;
        level = 1;
        score = 0;
        acquiredItem = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameState != GameState.Pause)
        {
            time += Time.deltaTime;
        }
        int t = Mathf.FloorToInt(time);

        if (t < 0)
        {
            countDownPanel.SetActive(true);
            scoreText.text = "Score : 0";
            countdownText.text = Mathf.Abs(t).ToString();
        }
        else
        {
            countDownPanel.SetActive(false);
            scoreText.text = "Score : " + (t + score);
            if (Input.GetKeyDown(KeyCode.F1))
            {
                gameState = GameState.Pause;
                settingPanel.SetActive(true);
            }
        }

        PlayerPrefs.SetInt("MyScore", (t + score));

        if (time >= thresholdTime)
        {
            LevelUp();
        }

        hpbar.value = (Mathf.Round(player.GetHP()) > 0.0f) ? Mathf.Round(player.GetHP()) : 0.0f;

        CheckState();
    }

    public void GetScore()
    {
        acquiredItem++;
        float reward = 1.0f + ((time / 2) + (acquiredItem * 10)) / 100.0f;
        score += (int)(50.0f * reward);
        Debug.Log((int)(50.0f * reward) + "점 획득");
    }

    public void LevelUp()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.levelUp, player.transform.position);
        level++;
        levelText.text = level.ToString();
        thresholdTime += 15;
        zombieSpawner.SpawnMaxCount += 5;
    }

    public void Resume()
    {
        gameState = GameState.Play;
    }

    public void CheckState()
    {
        switch (gameState)
        {
            case GameState.Play:
                Time.timeScale = 1.0f;
                break;
            case GameState.Pause:
                Time.timeScale = 0.0f;
                break;
        }
    }
}
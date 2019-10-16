using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private double thresholdTime = 15;
    private bool isMerged = false;
    private int score;
    private int level = 1;

    public static int damageScore;
    public static int killScore;
    public static int itemScore;

    public static float enemySpeed;
    public static double enemyHP;
    public static double enemyDamage;
    public static double tiredness;

    public static int enemySpawnLimit;
    public static int itemSpawnLimit;

    public static Score instance;
    public Text levelText;
    public Text scoreText;
    public Text highScoreText;
    public Text finalScoreText;

    public int MyScore
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
            scoreText.text = score.ToString();
            finalScoreText.text = score.ToString();

            if (PlayerPrefs.GetInt("HighScore") < score)
            {
                PlayerPrefs.SetInt("HighScore", score);
                highScoreText.text = score.ToString();
            }
        }
    }

    void Awake()
    {
        damageScore = 7;
        killScore = 100;
        itemScore = 20;
        enemySpeed = 1.5f;
        enemyHP = 100;
        enemyDamage = 5;
        tiredness = 0.1;
        enemySpawnLimit = 10;
        itemSpawnLimit = 20;

        instance = this;

        if (!PlayerPrefs.HasKey("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", 0);

            scoreText.text = "0";
            highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
        }
    }

    private void Update()
    {
        //관리자 기능 : HighScore 초기화
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PlayerPrefs.DeleteAll();
            highScoreText.text = score.ToString();
        }
        //Level 제어
        levelText.text = level.ToString();
        if (TimeManager.instance.MyPlayTime >= thresholdTime)
        {
            AdjustDifficulty();
            level++;
            thresholdTime += 15;
        }
        if (GameControl.instance.MyGameState == GameControl.GameState.Over && isMerged == false)
        {
            MergeScore();
            isMerged = true;
        }
    }

    void AdjustDifficulty()
    {
        Debug.Log("AdjustDifficulty");
        SoundManager.instance.PlaySound(16, false); // LevelUp 재생
        tiredness *= 1.2;
        enemyHP *= 1.2;
        enemyDamage *= 1.2;
        enemySpeed *= 1.2f;
        enemySpawnLimit += 5;
        itemSpawnLimit += 2;
        damageScore = (int)(damageScore * 1.1);
        killScore = (int)(killScore * 1.1);
        itemScore = (int)(itemScore * 1.1);
    }
    void MergeScore()
    {
        Score.instance.MyScore += (int)TimeManager.instance.MyPlayTime * (Score.instance.MyScore / 200);
    }
}

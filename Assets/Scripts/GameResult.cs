using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameResult : MonoBehaviour
{
    private int result;
    private int highScore;
    public Text resultTime;
    public Text bestTime;
    public GameObject resultUI;
    public string NextToLoad;

    void Start()
    {
        result = PlayerPrefs.GetInt("MyScore");
        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
    }

    void Update()
    {
            resultUI.SetActive(true);
            resultTime.text = "ResultTime : " + result;
            bestTime.text = "BestTime : " + highScore;
    }
    public void Loadnext()
    {
        SceneManager.LoadScene("startScene");
    }  
}
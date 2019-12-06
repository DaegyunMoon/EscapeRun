using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int level;
    private double thresholdTime = 15;
    public static float time;
    public PlayerControl player;
    public Text timeText;
    public Text levelText;
    public Slider hpbar;
    public GameObject gameOverPanel;
    public string NextToLoad;

    private TargetSpawner targetSpawner;

    // Start is called before the first frame update
    void Start()
    {
        targetSpawner = FindObjectOfType<TargetSpawner>();
        time = 0;
        level = 1;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        int t = Mathf.FloorToInt(time);
        timeText.text = "Time: " + t;
        PlayerPrefs.SetInt("MyScore", t);
        if (!PlayerPrefs.HasKey("HighScore") || PlayerPrefs.GetInt("HighScore") < t)
        {
            PlayerPrefs.SetInt("HighScore", t);
        }
        
        if (time >= thresholdTime)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.levelUp, player.transform.position);
            level++;
            levelText.text = level.ToString();
            thresholdTime += 15;
            targetSpawner.SpawnMaxCount += 5;
        }

        hpbar.value = (Mathf.Round(player.GetHP()) > 0.0f) ? Mathf.Round(player.GetHP()) : 0.0f;
    }
}

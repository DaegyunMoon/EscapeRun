using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int level;
    private double thresholdTime = 15;
    public float time;
    public PlayerControl player;
    public Text timeText;
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
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        int t = Mathf.FloorToInt(time);

        if (t < 0)
        {
            countDownPanel.SetActive(true);
            timeText.text = "Time : 0";
            countdownText.text = Mathf.Abs(t).ToString();
        }
        else
        {
            countDownPanel.SetActive(false);
            timeText.text = "Time : " + t;
            if (Input.GetKeyDown(KeyCode.F1))
            {
                settingPanel.SetActive(true);
            }
        }

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
            zombieSpawner.SpawnMaxCount += 5;
        }

        hpbar.value = (Mathf.Round(player.GetHP()) > 0.0f) ? Mathf.Round(player.GetHP()) : 0.0f;
    }
}
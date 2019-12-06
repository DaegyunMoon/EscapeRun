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
    public Text uiText;
    public Slider hpbar;
    public GameObject gameOverPanel;
    public string NextToLoad;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        int t = Mathf.FloorToInt(time);
        uiText.text = "Time: " + t;
        PlayerPrefs.SetInt("MyScore", t);
        if (!PlayerPrefs.HasKey("HighScore") || PlayerPrefs.GetInt("HighScore") < t)
        {
            PlayerPrefs.SetInt("HighScore", t);
        }

        if (time >= thresholdTime)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.levelUp, this.transform.position);
            level++;
            
            thresholdTime += 15;
        }

        hpbar.value = (Mathf.Round(player.GetHP()) > 0.0f) ? Mathf.Round(player.GetHP()) : 0.0f;
    }
}

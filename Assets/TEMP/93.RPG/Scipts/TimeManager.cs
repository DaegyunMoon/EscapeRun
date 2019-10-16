using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    private double startTick;
    private double nowTick;
    private double pauseTick;
    private int intervalTick;
    private double playTime;

    public static TimeManager instance;
    public Text playTimeText;

    public double MyPlayTime
    {
        get
        {
            return playTime;
        }
    }

    public double GetTick()
    {
        return System.DateTime.Now.Ticks;
    }
    public int GetInterval(double startTick, double nowTick)
    {
        return (int)((nowTick - startTick) / 1000000.00);
    }
    public void Pause()
    {
        pauseTick = nowTick - startTick;
    }
    public void Resume()
    {
        startTick = GetTick() - pauseTick;
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameControl.instance.MyGameState == GameControl.GameState.Start)
        {
            startTick = GetTick();
        }
        if(GameControl.instance.MyGameState == GameControl.GameState.Playing)
        {
            nowTick = GetTick();
            intervalTick = GetInterval(startTick, nowTick);
        }
        playTime = (double)intervalTick / 10.0;
        playTimeText.text = playTime.ToString();
    }
}

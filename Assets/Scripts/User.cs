using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    private string stdNum;
    private string name;
    private int score;
    
    public User(string stdNum, string name, int score)
    {
        this.stdNum = stdNum;
        this.name = name;
        this.score = score;
    }

    public string GetStdNum()
    {
        return stdNum;
    }
    public string GetName()
    {
        return name;
    }
    public int GetScore()
    {
        return score;
    }
}

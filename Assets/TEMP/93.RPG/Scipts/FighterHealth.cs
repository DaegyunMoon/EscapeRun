using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterHealth : MonoBehaviour
{
    public Image currentHealth;
    public static FighterControl myChar;

    private double healthPoint;
    private double maxPoint;

    private void Start()
    {
        myChar = GameObject.FindObjectOfType<FighterControl>();
        healthPoint = myChar.HP;
        maxPoint = myChar.HP;
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        float ratio = (float)(healthPoint / maxPoint);
        currentHealth.rectTransform.localScale = new Vector3(1, ratio, 1);
    }

    private void Update()
    {
        healthPoint = myChar.HP;
        UpdateHealth();
        if(healthPoint < 0)
        {
            healthPoint = 0;
        }
    }
}

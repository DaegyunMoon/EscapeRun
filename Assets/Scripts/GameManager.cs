using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayerControl player;
    public Slider hpbar;
    public GameObject gameOverPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hpbar.value = (Mathf.Round(player.GetHP()) > 0.0f) ? Mathf.Round(player.GetHP()) : 0.0f;
    }
}

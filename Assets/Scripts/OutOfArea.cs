using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutOfArea : MonoBehaviour
{
    public PlayerControl playerControl;
    void OnTriggerEnter(Collider col)
    {
        playerControl.playerState = PlayerState.Death;
    }
}
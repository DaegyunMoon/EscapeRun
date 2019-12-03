using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public string LevelToLoad;

    public void Loadnext()
    {
        SceneManager.LoadScene(LevelToLoad);
    }
    public void isExit()
    {
        Application.Quit();
    }
}

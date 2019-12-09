using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string NextToLoad;

    public void Loadnext()
    {
        SceneManager.LoadScene(NextToLoad);
    }
    public void ExitProgram()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine(WaitForLoadScene(0.5f, "93.RPG/Scenes/MainScene"));
        }
    }
    private IEnumerator WaitForLoadScene(float waitTime, string sceneName)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private int selected;
    public string NextToLoad;
    public Text[] menus;

    private void Start()
    {
        selected = 0;
    }
    private void Update()
    {
        SelectMenu();

    }
    void SelectMenu()
    {
        //if()
        //엔터를 누르면 선택된 메뉴가 실행되는 함수를 작성
    }
    public void Loadnext()
    {
        SceneManager.LoadScene(NextToLoad);
    }
    public void isExit()
    {
        Application.Quit();
    }
}

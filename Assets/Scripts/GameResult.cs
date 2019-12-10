using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml;

public class GameResult : MonoBehaviour
{
    private int result;
    private int highScore;
    public Text myScore;
    public Text[] numText = new Text[10];
    public Text[] nameText = new Text[10];
    public Text[] scoreText = new Text[10];

    public List<User> userList = new List<User>();

    void Start()
    {
        result = PlayerPrefs.GetInt("MyScore");
        myScore.text = "Your Score : " + result;
        GetUserListFromXml("Users");
        for (int i = 0; i < 10; i++)
        {
            if (i >= userList.Count) break;
            numText[i].text = userList[i].GetStdNum();
            nameText[i].text = userList[i].GetName();
            scoreText[i].text = userList[i].GetScore().ToString();
        }
    }

    public void Loadnext()
    {
        SceneManager.LoadScene("startScene");
    }  

    void GetUserListFromXml(string fileName)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(fileName);
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textAsset.text);

        XmlNodeList users = xmlDocument.SelectNodes("UserSet/User");

        foreach(XmlNode user in users)
        {
            if (user.SelectSingleNode("Name").InnerText == PlayerPrefs.GetString("MyName"))
            {
                user.SelectSingleNode("Score").InnerText = PlayerPrefs.GetInt("MyScore").ToString();
            }
            User item = new User(user.SelectSingleNode("StdNum").InnerText,
                user.SelectSingleNode("Name").InnerText,
                int.Parse(user.SelectSingleNode("Score").InnerText));
            userList.Add(item);
        }
        xmlDocument.Save("./Assets/Resources/Users.xml");

        if (userList.Count > 1)
        {
            userList.Sort(delegate (User a, User b)
            {
                if (a.GetScore() < b.GetScore()) return 1;
                else if (a.GetScore() > b.GetScore()) return -1;
                else return 0;
            });
        }
    }
}
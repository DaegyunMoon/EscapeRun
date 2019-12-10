using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public string NextToLoad;
    public Text nameText;
    public Text stdNumText;
    public Text phoneText;

    public void Loadnext()
    {
        PlayerPrefs.SetString("MyName", nameText.text);
        SceneManager.LoadScene(NextToLoad);
    }
    public void ExitProgram()
    {
        Application.Quit();
    }
    public void AddUser(string fileName)
    {
        try
        {
            TextAsset textAsset = (TextAsset)Resources.Load(fileName);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);

            XmlNode userSet = xmlDocument.SelectSingleNode("UserSet");
            XmlNode user = xmlDocument.CreateNode(XmlNodeType.Element, "User", string.Empty);
            userSet.AppendChild(user);

            XmlElement stdNum = xmlDocument.CreateElement("StdNum");
            stdNum.InnerText = stdNumText.text;
            user.AppendChild(stdNum);
            XmlElement name = xmlDocument.CreateElement("Name");
            name.InnerText = nameText.text;
            user.AppendChild(name);
            XmlElement score = xmlDocument.CreateElement("Score");
            score.InnerText = "0";
            user.AppendChild(score);
            XmlElement phone = xmlDocument.CreateElement("Phone");
            phone.InnerText = phoneText.text;
            user.AppendChild(phone);

            xmlDocument.Save("./Assets/Resources/Users.xml");
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
}

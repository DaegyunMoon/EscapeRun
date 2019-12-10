using System.IO;
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
        StreamReader sr = new StreamReader("./Assets/Resources/" + fileName);
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(sr.ReadToEnd());
        sr.Close();

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
        xmlDocument.Save("./Assets/Resources/" + fileName);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;
using System.IO;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
public Button Starta;
    public Button Startb;

	public void NewDeck()
{  
        List<int> indexs = new List<int>();
        JsonData CardJson = JsonMapper.ToJson(indexs);
        
        FileInfo fi = new FileInfo(Application.dataPath + "/CardData.json");
        if (!fi.Exists)
        {
            File.Create(Application.dataPath + "/CardData.json").Dispose();
        }
        
        File.WriteAllText(Application.dataPath + "/CardData.json", CardJson.ToString());
        SceneManager.LoadScene ("DeckConstruct");
       
    }
    public void LoadDeck()
    {
        FileInfo fi = new FileInfo(Application.dataPath + "/CardData.json");
        if (!fi.Exists)
        {
            return;
        }
        SceneManager.LoadScene("DeckConstruct");
    }
	void Start()
	{


        Starta.onClick.AddListener (NewDeck);
        Startb.onClick.AddListener(LoadDeck);
       
    }
}
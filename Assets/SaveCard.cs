using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class SaveCard : MonoBehaviour

{
    public List<Card> CardList = new List<Card>();
    public int id;
    public Sprite sprite;
    public string Cardname;
    public string explain;
    public int cost;
    public GameObject effect;
    public CardTarget target;
    // Start is called before the first frame update
    void Start()
    {
        CardList.Add(new Card());
        SaveCards();
        LoadCard();
    }
    public void SaveCards()
    {
        
        JsonData CardJson = JsonMapper.ToJson(CardList);
        File.WriteAllText(Application.dataPath + "/Cards/CardData.json", CardJson.ToString());//save

    }
    public void LoadCard(){
        string json = File.ReadAllText(Application.dataPath + "/Cards/CardData.json");
        JsonData CardData = JsonMapper.ToObject(json);
        for(int i = 0;i < CardData.Count; i++)
        {
            Debug.Log(CardData[i]["ID"].ToString() + CardData[i]["Cardname"].ToString() + CardData[i]["CardCost"].ToString() + CardData[i]["Explain"].ToString());
        }
        }

}

using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckCons : MonoBehaviour
{
    public Button StartA;
    public static int  field1 = 0;
    public static int field2 = 0;
    public static int field3 = 0;
    public List<int> CardList = new List<int>();
    public List<int> DeckList = new List<int>();
    public List<int> CardList2 = new List<int>();
    public List<int> DeckList2 = new List<int>();
    List<GameObject> DeckCards = new List<GameObject>();
    List<GameObject> DeckCardbars = new List<GameObject>();
    public GameObject cardprefab;
    public GameObject deckcardprefab;
    public GameObject deckcardbarprefab;
    private int RandomCard;
    private int Index;
    private int IndexCheck = 0;
    GameObject Content;

    
    // Start is called before the first frame update
    void Start()
    {
        Content = GameObject.Find("Content");
        string json = File.ReadAllText(Application.dataPath + "/CardData.json");
        if (json != null)
        {
            JsonData CardData = JsonMapper.ToObject(json);
            for (int i = 0; i < CardData.Count; i++)
            {
                CardList.Add(int.Parse(CardData[i].ToString()));
            }
        }
        ShowCardList();
        int IndexMax;
        Index = 1;
        if (cardprefab.GetComponent<CardScript>().info.Length % 16 == 0)
            IndexMax = cardprefab.GetComponent<CardScript>().info.Length / 12;
        else
            IndexMax = (cardprefab.GetComponent<CardScript>().info.Length / 12) + 1;

        ShowCards();
        StartA.onClick.AddListener(StartF);
            DontDestroyOnLoad(this);
        
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void StartF()
    {
        int deckn = CardList.Count;
        for (int i = 0; i < deckn; i++)
        {
            RandomCard = Random.Range(0, deckn - i);
            DeckList.Add(CardList[RandomCard]);
            CardList.RemoveAt(RandomCard);
        }
        //deck2
        for (int i = 0; i < 20; i++)
        {
            CardList2.Add(1);
        }
        for (int i = 0; i < 30; i++)
        {
            CardList2.Add(2);
        }
        for (int i = 0; i < 10; i++)
        {
            CardList2.Add(3);
        }
        for (int i = 0; i < 60; i++)
        {
            RandomCard = Random.Range(0, 60 - i);
            DeckList2.Add(CardList2[RandomCard]);
            CardList2.RemoveAt(RandomCard);
        }
        JsonData CardJson = JsonMapper.ToJson(DeckList);
        File.WriteAllText(Application.dataPath + "/CardData.json", CardJson.ToString());
        //SceneManager.LoadScene("SampleScene");
        SceneManager.LoadScene("Server");
    }
    private void AddToList(List<int> A,int Type, int CardNum)
    {
        for (int i = 0; i < CardNum; i++)
        {
            CardList.Add(Type);
        }
    }
    void ShowCards()
    {
        if((Index-1)*16 < cardprefab.GetComponent<CardScript>().info.Length)
        for (int i = 0; i < cardprefab.GetComponent<CardScript>().info.Length - ((Index - 1) * 12); i++)
        {
            DeckCards.Add(Instantiate(deckcardprefab, new Vector3(-4.5f + ((i % 4) * (5f / 3f)), 0.5f - (i / 4 * 2), 0), Quaternion.identity));
            DeckCards[i].GetComponent<DeckCard>().CardInfo = cardprefab.GetComponent<CardScript>().info[(Index - 1) * 12 + i];
        }
    }
    public void ShowCardList()
    {
        for (int i = 0; i < DeckCardbars.Count; i++)
            Destroy(DeckCardbars[i]);

        DeckCardbars.Clear();
        List<int> Cards;
        int Order = 0;
        for (int i = 1; i <= (int)cardprefab.GetComponent<CardScript>().info.Length; i++)
        {
            IndexCheck = i;
            Cards = CardList.FindAll(FindIndex);
            if (Cards.Count != 0)
            {

                Card Cardinfo = SearchCard(i);
                DeckCardbars.Add(Instantiate(deckcardbarprefab, Content.transform.position, Quaternion.identity));
                DeckCardbars[Order].GetComponent<BarScript>().Cardname.text = Cardinfo.Cardname;
                DeckCardbars[Order].GetComponent<BarScript>().Cost.text = "" + Cardinfo.Cost;
                DeckCardbars[Order].GetComponent<BarScript>().CardTotal.text = "" + Cards.Count;
                DeckCardbars[Order].GetComponent<BarScript>().Index = Cardinfo.ID;
                DeckCardbars[Order].transform.SetParent(Content.transform);
                Order++;

            }
        }
    }
    bool FindIndex(int Type)
    {
        if (Type == IndexCheck)
        {
            return true;
        }
        else
            return false;
    }
    public Card SearchCard(int Index)
    {
        for (int i = 0; i < cardprefab.GetComponent<CardScript>().info.Length; i++)
            if (cardprefab.GetComponent<CardScript>().info[i].ID == Index)
                return cardprefab.GetComponent<CardScript>().info[i];
        return null;
    }
}

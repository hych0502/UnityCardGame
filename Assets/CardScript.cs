using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;




public enum CardTarget
{
    None,
    One
};

public class CardScript : MonoBehaviour
{
    //카드 타입 지정

    /*
[Serializable]
public struct Card
{
    public Sprite Sprite;
    public string Cardname;
    public string Explain;
    public int Cost;
    public GameObject Effect;
    public CardType type;
    public PeerType peertype;
    public CardTarget  Target;
};
*/

    public Card[] info;
    public TextMesh Label;
    public SpriteRenderer SR;
    public Sprite front;
    public Sprite back;
    public Sprite ChangeCheck;
    public GameObject toolprefab;
    [HideInInspector]
    public int Index;
    public static Vector3 defpos;


    private GameObject tooltip;
    private Card Cardinfo = null;
    private bool isObject = false;
    [HideInInspector]
    public bool isPlayer;
    private GameObject GM;



    //툴팁
    private void OnMouseOver()
    {
        if (GM.GetComponent<GameManager>().Status == Mode.PlayMode)
        {
            int Value;
            if (!isObject)
            {
                Destroy(GameObject.FindGameObjectWithTag("Tooltip"));
                isObject = true;
                tooltip = Instantiate(toolprefab, new Vector3(0, 0, 0), Quaternion.identity);
                if (Cardinfo is PeerCard)

                {
                    PeerCard PeerInfo = (PeerCard)Cardinfo;
                    Value = PeerInfo.attack;
                    tooltip.GetComponent<Tooltip>().Attack.text = "AT:" + Value;
                    Value = PeerInfo.hp;
                    tooltip.GetComponent<Tooltip>().HP.text = "HP:" + Value;
                }
                else
                {
                    tooltip.GetComponent<Tooltip>().Attack.text = "";
                    tooltip.GetComponent<Tooltip>().HP.text = "";
                }
                tooltip.GetComponent<Tooltip>().title.text = Cardinfo.Cardname;
                tooltip.GetComponent<Tooltip>().effect.text = Cardinfo.Explain;
                tooltip.GetComponent<Tooltip>().costnumber.text = "" + Cardinfo.Cost;
            }
        }

    }
    private void OnMouseExit()
    {
        
        isObject = false;
        Destroy(tooltip);
    }
    //발동

    private void OnMouseDown()
    {
        if(GM.GetComponent<GameManager>().Status == Mode.ChangeCards && !GM.GetComponent<GameManager>().playerChanged)
        {
            if (!transform.GetChild(2).gameObject.activeSelf)
                transform.GetChild(2).gameObject.SetActive(true);
            else
                transform.GetChild(2).gameObject.SetActive(false);
        }
        else 
           defpos = transform.position;

    }

    private void OnMouseDrag()
    {
        if (GM.GetComponent<GameManager>().Status == Mode.PlayMode && GM.GetComponent<GameManager>().GameTurn == Turn.Player)
        {
            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = currentPos;
            transform.Translate(new Vector3(0, 0, -1));
        }

    }
    private void OnMouseUp()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //1차 발동조건
        if (mousePos.x >= -4 && mousePos.x <= 4 && mousePos.y <= 2 && mousePos.y >= -2 && GM.GetComponent<GameManager>().Status == Mode.PlayMode && GM.GetComponent<GameManager>().GameTurn == Turn.Player)
        {
            //2차 발동조건
                if (Cardinfo.Activeable() && ActiveAble())
                {
                    GM.GetComponent<GameManager>().Cards.Remove(gameObject);
                    GM.GetComponent<GameManager>().ActivatingCard = Cardinfo;
                    Destroy(tooltip);
                    Destroy(gameObject);
                    
                }

            GM.GetComponent<GameManager>().SortCard();
        }
        else if(GM.GetComponent<GameManager>().Status == Mode.PlayMode)
        {
            transform.position = defpos;
        }
    }
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GM");
        for(int i = 0;i < info.Length; i++)
        {
            if(info[i].ID == Index)
            {
                Cardinfo = info[i];
                break;
            }
        }
        if (Cardinfo == null)
            Debug.Log("Error");
        SR.sprite = Cardinfo.Sprite;
        Label.text = Cardinfo.Cardname;
    }

    // Update is called once per frame
    void Update()
    {
        if (ActiveAble() && Cardinfo.Activeable() && GM.GetComponent<GameManager>().GameTurn == Turn.Player && GM.GetComponent<GameManager>().Status == Mode.PlayMode)
            gameObject.GetComponent<Outline>().eraseRenderer = false;
        else
            gameObject.GetComponent<Outline>().eraseRenderer = true;

    }
    bool ActiveAble()
    {
        if (isPlayer == true && GM.GetComponent<GameManager>().CurrentAP >= Cardinfo.Cost && GM.GetComponent<GameManager>().ActiveCheck(Index) == true)
        {

                if ((Cardinfo is PeerCard && GM.GetComponent<GameManager>().Peers.Count < 5) || !(Cardinfo is PeerCard))
                {

                    return true;

                }
            return false;

        }
        return false;
    }
    void OpenImage()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = back;
        SR.gameObject.SetActive(false);
    }

    // 앞면 감추기
    void CloseImage()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = front;
        SR.gameObject.SetActive(true);
    }
}





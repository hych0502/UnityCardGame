using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCard : MonoBehaviour
{
    [HideInInspector]
    public Card CardInfo;
    GameObject DeckCons;
    public SpriteRenderer SR;
    public TextMesh Label;
    public GameObject toolprefab;
    private GameObject tooltip;
    public GameObject Peer;
    public GameObject cardprefab;
    Card[] info;
    private Vector3 defpos;
    bool isObject;
    // Start is called before the first frame update
    private void OnMouseOver()
    {
        if (!isObject)
        {
            int Value;
            isObject = true;
            tooltip = (GameObject)Instantiate(toolprefab, new Vector3(0, 0, 0), Quaternion.identity);
            if (CardInfo is PeerCard)

            {
                PeerCard PeerInfo = (PeerCard)CardInfo;
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
            tooltip.GetComponent<Tooltip>().title.text = CardInfo.Cardname;
            tooltip.GetComponent<Tooltip>().effect.text = CardInfo.Explain;
            tooltip.GetComponent<Tooltip>().costnumber.text = "" + CardInfo.Cost;
        }

    }
    private void OnMouseExit()
    {
        isObject = false;
        Destroy(tooltip);
    }
    void Start()
    {
        DeckCons = GameObject.Find("Deck");
        info = cardprefab.GetComponent<CardScript>().info;
        SR.sprite = CardInfo.Sprite;
        Label.text = CardInfo.Cardname;
    }
    private void OnMouseUp()
    {
        
            RaycastHit hit = new RaycastHit();


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
            Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.name == "DeckBoard")
                {
                    DeckCons.GetComponent<DeckCons>().CardList.Add(CardInfo.ID);
                    DeckCons.GetComponent<DeckCons>().ShowCardList();
                }
                    
            }

            transform.position = defpos;

        


    }

    void Update()
    {
        
    }
    private void OnMouseDrag()
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = currentPos;
        transform.Translate(new Vector3(0, 0, -1));

    }
    private void OnMouseDown()
    {
        defpos = transform.position;

    }
}

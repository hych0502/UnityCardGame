using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PeerScript : MonoBehaviour
{



    public TextMesh text1;
    public TextMesh text2;
    public GameObject lineObject;
    public SpriteRenderer SR;
    public PeerCard PeerInfo;
    GameObject targetPeer;
    GameObject GM;
    LineRenderer line;
    Vector3 center = Vector3.zero;
    Vector3 theArc = Vector3.zero;
    public bool isPlayer;
    private int Maxhp;
    private int hp;
    private TTCP Network;
    public int HP
    {
        get { return hp; }
        set { hp = value; text2.text = "" + hp;
            if (hp <= 0)
            {
                if(isPlayer == true)
                GM.GetComponent<GameManager>().Peers.Remove(gameObject);
                else
                GM.GetComponent<GameManager>().PeersE.Remove(gameObject);
                Destroy(gameObject);
                GM.GetComponent<GameManager>().SortPeer();
            }
        }
    }
    private int attack;
    public int Attack
    {
        get { return attack; }
        set { attack = value; text1.text = "" + attack; }
    }
    [HideInInspector]
    public bool isAttackable = false;

    private void OnMouseDrag()
    {
        if (isAttackable && GM.GetComponent<GameManager>().GameTurn == Turn.Player)
        {
            //Debug.Log("drawing");
            Vector2 targetPoint;
            targetPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            center = (transform.position + (Vector3)targetPoint) * 0.5f;
            center.y -= 2.0f;
            Vector3 C1 = transform.position - center;
            Vector3 C2 = (Vector3)targetPoint - center;
            C1.z = 0;
            C2.z = 0;

            for (float index = 0.0f, interval = -0.04f; interval < 1.0f;)
            {
                Vector3 theArc = Vector3.Slerp(C2, C1, interval += 0.04f);
                line.SetPosition((int)index++, theArc + center);
            }
            lineObject.SetActive(true);
        }
        else
        {
            Debug.Log("Already Attacked or Summoned in this turn");
        }
    }
    private void OnMouseUp()
    {
        if (isAttackable&&((isPlayer == false && GM.GetComponent<GameManager>().GameTurn == Turn.Enemy)|| (isPlayer == true && GM.GetComponent<GameManager>().GameTurn == Turn.Player)))
        {
            lineObject.SetActive(false);
            AttackSeq();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(GM == null)
              GM = GameObject.Find("GameManager");
        GameObject Server = GameObject.FindGameObjectWithTag("Server");
        Network = Server.GetComponent<ServerScript>().Network;
        line = lineObject.GetComponent<LineRenderer>();
        line.startColor = Color.blue;
        line.endColor = Color.blue;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 26;
        Maxhp = HP = PeerInfo.hp;
        Attack = PeerInfo.attack;
        SR.sprite = PeerInfo.Peerimg;
        
        //Vector3 targetPoint = circle.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (isAttackable) 
            gameObject.GetComponent<Outline>().eraseRenderer = false;
        else
            gameObject.GetComponent<Outline>().eraseRenderer = true;
    }
    void AttackSeq()
    {
        targetPeer = null;
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);
        if(hit.collider!= null)
        {
            targetPeer = hit.collider.gameObject;
            if (targetPeer.GetComponent<PeerScript>() != null && targetPeer != gameObject && targetPeer.GetComponent<PeerScript>().isPlayer == false)
            {
                int PeerIndex = GM.GetComponent<GameManager>().Peers.FindIndex(SearchThisPeer);
                int EnemyPeerIndex = GM.GetComponent<GameManager>().PeersE.FindIndex(SearchEnemyPeer) + 5;
                targetPeer.GetComponent<PeerScript>().HP -= Attack;
                HP -= targetPeer.GetComponent<PeerScript>().Attack;
                GM.GetComponent<GameManager>().AttackSend(PeerIndex, EnemyPeerIndex);
                isAttackable = false;
            }
            
            else if (targetPeer.name == "Enemy")
            {
                int PeerIndex = GM.GetComponent<GameManager>().Peers.FindIndex(SearchThisPeer);
                GM.GetComponent<GameManager>().AttackSend(PeerIndex, PeerIndex);
                GM.GetComponent<GameManager>().EnemyHP -= Attack;
                isAttackable = false;
            }

        }


    }
    bool SearchThisPeer(GameObject Object)
    {
        if (gameObject == Object)
            return true;
        return false;
    }
    bool SearchEnemyPeer(GameObject Object)
    {
        if (targetPeer == Object)
            return true;
        return false;
    }
    public void InitPeer(PeerCard peerInfo,bool PE)
    {
        GM = GameObject.Find("GameManager");
        PeerInfo = peerInfo;
        isPlayer = PE;
        GM.GetComponent<GameManager>().SortPeer();
        if(isPlayer == true)
            GM.GetComponent<GameManager>().Peers.Add(gameObject);
        else
            GM.GetComponent<GameManager>().PeersE.Add(gameObject);
        
    }
}

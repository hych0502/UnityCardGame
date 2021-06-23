using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum Turn
{
    Player,
    Enemy,
    Unknown
}
public enum Mode {
    Ready,
    ChangeCards,
    PlayMode,
    TargetMode
}

public class GameManager : MonoBehaviour
{
    //네트워크 준비
    public string ServerIP;
    public int ServerPort;
    private TTCP Network;
    private string Data;
    private bool init = false;
    private bool EnemyChanged = false;
    private bool PlayerChanged = false;
    public bool playerChanged {
        get { return PlayerChanged; }
        }



    private GameObject Deck;
    public GameObject CardPrefab;
    private Card[] Cardinfo; 
    public GameObject Peer;
    public GameObject Player;
    public GameObject Enemy;


    List<int> DeckList = new List<int>();
    //List<int> DeckListE = new List<int>();
    List<int> unuseableCards = new List<int>();
    List<int> TargetIndexs = new List<int>();


    public TextMesh text1;
    public TextMesh text2;
    public TextMesh text3;
    public TextMesh text4;
    public TextMesh Ptext;
    public TextMesh Etext;
    public Button TurnEnd;

    private Turn gameTurn = Turn.Unknown;
    GameObject Endbutton;
    [HideInInspector]
    public GameObject TargetObj = null;
    [HideInInspector]
    public Card ActivatingCard = null;
    private int ElementsIndex = 0;
    private bool Activating = false;
    public Turn GameTurn
    {
        get { return gameTurn; }
        set
        {
            gameTurn = value;
            if (gameTurn == Turn.Player)
            {
                text3.text = "Player's Turn";
            }
            else if (gameTurn == Turn.Enemy)
            {
                text3.text = "Enemy's Turn";
            }
            else
                text3.text = "Ready";

        }
    }

    [HideInInspector]
    public List<GameObject> Cards = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> Peers = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> PeersE = new List<GameObject>();
    private int apmax = 1;
    [HideInInspector]
    public int APmax
    {
        get { return apmax; }
        set
        {
            apmax = value;
            text1.text = CurrentAP + "/" + APmax;
        }
    }
    [HideInInspector]
    public int APmaxE = 1;
    private int currentAP = 0;
    //[HideInInspector]
    public int CurrentAP
    {
        get { return currentAP; }
        set
        {
            currentAP = value;
            text1.text = CurrentAP + "/" + APmax;

        }
    }
    [HideInInspector]
    public int CurrentAPE = 0;


    private int playerhp;
    public int PlayerHP
    {
        get { return playerhp; }
        set { playerhp = value; Ptext.text = "" + playerhp;
            if (playerhp <= 0)
            {
                Application.Quit();
            }
        }
    }
    private int enemyhp;
    public int EnemyHP
    {
        get { return enemyhp; }
        set { enemyhp = value; Etext.text = "" + enemyhp;
            if (enemyhp <= 0)
            {
                Application.Quit();
            }
        }
    }
    private Mode status;
    public Mode Status {
        get { return status; }
        set { status = value;
            if (status == Mode.TargetMode)
            {

                Endbutton.SetActive(false);
                text4.text = "target";
            }
            else if (status == Mode.PlayMode)
            {
                Endbutton.transform.GetChild(0).GetComponent<Text>().text = "End";
                Endbutton.SetActive(true);
                text4.text = "";
            }
            else if(status == Mode.Ready)
            {
                text4.text = "준비중입니다.";
                Endbutton.SetActive(false);
            }
            else if(status == Mode.ChangeCards)
            {
                text4.text = "교체할 카드를 선택하세요.";
                Endbutton.transform.GetChild(0).GetComponent<Text>().text = "선택완료";
                Endbutton.SetActive(true);
            }
            
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Cardinfo = CardPrefab.GetComponent<CardScript>().info;
        Endbutton = GameObject.FindGameObjectWithTag("EndButton");
        Status = Mode.Ready;
        GameObject Server = GameObject.FindGameObjectWithTag("Server");
        Network = Server.GetComponent<ServerScript>().Network;
        GameTurn = Turn.Unknown;
        PlayerHP = 30;
        EnemyHP = 30;
        TurnEnd.onClick.AddListener(End);
        //덱 불러오기
        Deck = GameObject.Find("Deck");
        for (int i = 0; i < Deck.GetComponent<DeckCons>().DeckList.Count; i++)
        {
            DeckList.Add(Deck.GetComponent<DeckCons>().DeckList[i]);
        }

        Destroy(Deck);
        //카드 뽑기
        for (int i = 0; i < 5; i++)
            CardDraw();

        SortCard();
        SortPeer();
        //Status = Mode.PlayMode;
        CurrentAP = APmax;

    }

    // 업데이트
    void Update()
    {
        //카드 발동처리
        if(ActivatingCard != null)
        {
            if (Activating == false)
            {
                if (ActivatingCard.element.Length <= ElementsIndex)
                {
                    int targetn = 0;
                    for (int i = 0; i < ActivatingCard.element.Length; i++)
                    {
                        if (ActivatingCard.element[i].Target != TargetSelection.None)
                        {
                            TargetObj = GetTarget(TargetIndexs[targetn]);
                            if (TargetIndexs[i] < 10)
                                Data = Data + "0" + TargetIndexs[i];
                            else
                                Data = Data + TargetIndexs[i];
                        }
                        ActivatingCard.element[i].Active();
                    }
                    TargetIndexs.Clear();
                    Debug.Log("targetdata : " + Data);
                    for (int i = 0; i < 4; i++) { 
                        int times = 1; 
                        for (int j = 0; j < 3 - i; j++)
                            times *= 10;
                        if(ActivatingCard.ID < times)
                        {
                            Data += "0";
                        }
                    }
                    Data = Data + ActivatingCard.ID;
                    CurrentAP = CurrentAP - ActivatingCard.Cost;
                    if (ActivatingCard is PeerCard)
                    {
                        PeerCard Peerinfo = (PeerCard)ActivatingCard;
                        GameObject peerA = Instantiate(Peer, new Vector3(0, 0, 0), Quaternion.identity);
                        peerA.GetComponent<PeerScript>().InitPeer(Peerinfo, true);
                        SortPeer();
                    }
                    TargetObj = null;
                    ElementsIndex = 0;
                    Network.Send(System.Convert.ToInt32(Data));
                    ActivatingCard = null;
                    Data = "";
                }

                else if (ActivatingCard.element[ElementsIndex].Target != TargetSelection.None)
                {
                    
                    Status = Mode.TargetMode;
                    Activating = true;
                }
                else if(ActivatingCard.element[ElementsIndex].Target == TargetSelection.None)
                {
                    Debug.Log("타겟 없음");
                    ElementsIndex++;
                }
            }

        }
        //카드발동처리 끝
        //타겟
        if (Status == Mode.TargetMode && TargetObj != null)
        {
            //ActivatingCard.element[ElementsIndex].Active();
            bool tActived = false;
            if (PeersE.Contains(TargetObj) && (ActivatingCard.element[ElementsIndex].Target == TargetSelection.EnemyP || ActivatingCard.element[ElementsIndex].Target == TargetSelection.EnemyPL))
            {
                TargetIndexs.Add(PeersE.FindIndex(SearchPeerindex) + 5);
                tActived = true;
            }
            else if (Peers.Contains(TargetObj) && (ActivatingCard.element[ElementsIndex].Target == TargetSelection.PlayerP || ActivatingCard.element[ElementsIndex].Target == TargetSelection.PlayerPL))
            {
                TargetIndexs.Add(Peers.FindIndex(SearchPeerindex));
                tActived = true;
            }
            else if (TargetObj.tag == "Player" && (ActivatingCard.element[ElementsIndex].Target == TargetSelection.PlayerPL))
            {
                TargetIndexs.Add(10);
                tActived = true;
            }
            else if (TargetObj.tag == "Enemy" && (ActivatingCard.element[ElementsIndex].Target == TargetSelection.EnemyPL))
            {
                TargetIndexs.Add(11);
                tActived = true;
            }
            if (tActived == true)
            {
                Status = Mode.PlayMode;
                TargetObj = null;
                Activating = false;
                ElementsIndex++;

            }
        }
        //이니시에이트
        if(GameTurn == Turn.Enemy && init && Status == Mode.PlayMode)
        {

            int data = Network.UpRecieve();
            if(data != -1)
            {
                EnemyActive(data);
            }
        }
        
        //서버연결
        if (Status == Mode.Ready && Network.IsConnected && !init && Network.IsServer)
        {
            //승인
            Status = Mode.ChangeCards;
            int first = Random.Range(0, 2);
            if (first == 1)
                GameTurn = Turn.Player;
            else
                GameTurn = Turn.Enemy;
            Network.Send(first);
            init = true;
        }
        //클라연결
        if(!Network.IsServer && Status == Mode.Ready && !init && Network.IsConnected)
        {
            int data = Network.UpRecieve();
            if(data != -1)
            {
                if (data == 1)
                    GameTurn = Turn.Enemy;
                else
                    GameTurn = Turn.Player;
                Status = Mode.ChangeCards;

            }
            init = true;
        }
        //상대 멀리건완료시
        if(Status == Mode.ChangeCards && Network.IsConnected) {
            int data = Network.UpRecieve();
            if(data != -1)
            {
                if (data == 0)
                    EnemyChanged = true;
            }



        }
        if(Status == Mode.ChangeCards && EnemyChanged && PlayerChanged)
        {
            Status = Mode.PlayMode;
        }
        


        text2.text = "덱 : " + DeckList.Count;

    }
    public void SortCard()
    {
        if (Cards.Count > 1)
            for (int i = 0; i < Cards.Count; i++)
                Cards[i].transform.position = new Vector3(-5f + ((10.0f / (Cards.Count - 1)) * i), -4f, (float)10 - i);

        else if (Cards.Count == 1)
            Cards[0].transform.position = new Vector3(-5f, -4f, -1f);


    }
    public void SortPeer()
    {
        if (Peers.Count > 1)
            for (int i = 0; i < Peers.Count; i++)
                Peers[i].transform.position = new Vector3(((-(float)(Peers.Count - 1) / 2) + i) * 1.5f, -1, 1);
        else if (Peers.Count == 1)
            Peers[0].transform.position = new Vector3(0, -1, 1);

        if (PeersE.Count > 1)
            for (int i = 0; i < PeersE.Count; i++)
                PeersE[i].transform.position = new Vector3(((-(float)(PeersE.Count - 1) / 2) + i) * 1.5f, 1, 1);
        else if (PeersE.Count == 1)
            PeersE[0].transform.position = new Vector3(0, 1, 1);
    }
    public void CardIn(int Index)
    {
        List<int> TempDeckList = new List<int>();
        for (int i = 0; i < DeckList.Count; i++)
        {
            TempDeckList.Add(DeckList[i]);
        }
        TempDeckList.Add(Index);
        DeckList.Clear();
        while (TempDeckList.Count > 0)
        {
            int Rannum = Random.Range(0, TempDeckList.Count);
            DeckList.Add(TempDeckList[Rannum]);
            TempDeckList.RemoveAt(Rannum);
        }

    }
    public void Shuffle()
    {
        List<int> TempDeckList = new List<int>();
        for (int i = 0; i < DeckList.Count; i++)
        {
            TempDeckList.Add(DeckList[i]);
        }
        DeckList.Clear();
        while (TempDeckList.Count > 0)
        {
            int Rannum = Random.Range(0, TempDeckList.Count);
            DeckList.Add(TempDeckList[Rannum]);
            TempDeckList.RemoveAt(Rannum);
        }
    }
    //턴 엔드시
    private void End()
    {
        if(Status == Mode.ChangeCards && !PlayerChanged)
        {
            List<GameObject> Deletes = new List<GameObject>();
            int Draws = 0;
            for(int i = 0; i < Cards.Count; i++)
            {
                if(Cards[i].GetComponent<Transform>().GetChild(2).gameObject.activeSelf == true)
                {
                    CardIn(Cards[i].GetComponent<CardScript>().Index);
                    Destroy(Cards[i]);
                    Deletes.Add(Cards[i]);
                    Draws++;
                }
            }
            for(int i = 0; i < Deletes.Count; i++)
            {
                Cards.Remove(Deletes[i]);
            }
            for(int i = 0; i< Draws; i++)
            {
                CardDraw();
            }
            PlayerChanged = true;
            text4.text = "상대방 준비중...";
            Network.Send(0);
        }
        if (GameTurn == Turn.Player && Status == Mode.PlayMode)
        {
            GameTurn = Turn.Enemy;
            Network.Send(0);
        }
    }
    //카드 뽑을 시
    public void CardDraw()
    {
        if (DeckList.Count > 0 && Cards.Count < 10)
        {
            Cards.Add(Instantiate(CardPrefab, Vector3.zero, Quaternion.identity));
            Cards[Cards.Count - 1].GetComponent<CardScript>().Index = DeckList[DeckList.Count - 1];
            Cards[Cards.Count - 1].GetComponent<CardScript>().isPlayer = true;
            SortCard();
        }
        if (DeckList.Count > 0)
        {
            DeckList.RemoveAt(DeckList.Count - 1);
        }
    }

    //카드 턴 제한
    public void CantActive(int T)
    {
        unuseableCards.Add(T);
    }
    //턴 제한 체크
    public bool ActiveCheck(int T)
    {
        for (int i = 0; i < unuseableCards.Count; i++)
            if (unuseableCards[i] == T)
                return false;
        return true;
    }
    public void Attackable()
    {
        int Count = Peers.Count;
        for (int i = 0; i < Count; i++)
        {
            Peers[i].GetComponent<PeerScript>().isAttackable = true;
        }
    }
    public Card SearchCard(int Index)
    {
        for (int i = 0; i < Cardinfo.Length; i++)
            if (Cardinfo[i].ID == Index)
                return Cardinfo[i];
        return null;
    }
    public void ActivateElements(){

        }
    public bool SearchPeerindex(GameObject Object)
    {
        if (TargetObj == Object)
                return true;
        return false;
    }
    public void EnemyActive(int data)
    {
        if(data == 0)
        {
            GameTurn = Turn.Player;
            CurrentAP = APmax;
            CardDraw();
            unuseableCards.Clear();
            Attackable();
            return;
        }
        int Index = data % 10000;
        if(Index == 9999)
        {
            //공격수신
            int AttackPeerIndex = (data % 1000000)/100000;
            int DefPeerIndex = (data % 100000) / 10000;
            GameObject AttackPeer = GetTarget(AttackPeerIndex);
            GameObject DefPeer = GetTarget(DefPeerIndex);
            if(AttackPeerIndex == DefPeerIndex)
            {
                //명치공격
                PlayerHP -= AttackPeer.GetComponent<PeerScript>().Attack;
                Debug.Log( AttackPeer.GetComponent<PeerScript>().PeerInfo.Cardname+"(" + AttackPeerIndex + ") is Attacked You");
            
            }
            else
            {
                AttackPeer.GetComponent<PeerScript>().HP -= DefPeer.GetComponent<PeerScript>().Attack;
                DefPeer.GetComponent<PeerScript>().HP -= AttackPeer.GetComponent<PeerScript>().Attack;
                Debug.Log(AttackPeer.GetComponent<PeerScript>().PeerInfo.Cardname + "(" + AttackPeerIndex + ") Attacked " + DefPeer.GetComponent<PeerScript>().PeerInfo.Cardname + "(" + DefPeerIndex + ")");
                //교환공격
            }
            return;

        }
        Card EnemyCard = SearchCard(Index);
        if(EnemyCard is PeerCard)
        {
            PeerCard pCard = (PeerCard)EnemyCard;
            GameObject peerA = Instantiate(Peer, new Vector3(0, 0, 0), Quaternion.identity);
            peerA.GetComponent<PeerScript>().InitPeer(pCard, false);
            SortPeer();
        }
        for (int i = 0; i < EnemyCard.element.Length; i++)
        {
            if (EnemyCard.element[i].Target != TargetSelection.None)
            {
                int time = 1;
                for (int j = 0; j < (EnemyCard.element.Length * 2 )+ 4 - i*2; j++)
                    time *= 10;
                int TargetN = (data % time) / (time / 100);
                Debug.Log("TargetN: " + TargetN);
                TargetObj = GetTarget(TargetN);
            }
            EnemyCard.element[i].Active();
        }
        TargetObj = null;
    }
    public void AttackSend(int AtkPeerIndex, int DefPeerIndex)
    {
        string data = "";
        Debug.Log(AtkPeerIndex +" " + DefPeerIndex); 
        data = data + AtkPeerIndex;
        data = data + DefPeerIndex;
        data = data + "9999";
        Debug.Log("AttackSend:" + data);
        Network.Send(System.Convert.ToInt32(data));
    }
    GameObject GetTarget(int TargetIndex)
    {
        if (GameTurn == Turn.Enemy)
        {
            if (TargetIndex >= 5 && TargetIndex < 10)
            {
                return Peers[TargetIndex - 5];
            }
            else if (TargetIndex >= 0 && TargetIndex < 5)
            {
                return PeersE[TargetIndex];
            }
            else if(TargetIndex == 10)
            {
                return Enemy;
            }
            else if (TargetIndex == 11)
            {
                return Player;
            }
        }
        else if (GameTurn == Turn.Player)
        {
            if (TargetIndex >= 5 && TargetIndex < 10)
            {
                return PeersE[TargetIndex - 5];
            }
            else if (TargetIndex >= 0 && TargetIndex < 5)
            {
                return Peers[TargetIndex];
            }
        }
        Debug.Log("Error");
        Application.Quit();
        Debug.Break();
        return null;
    }
}

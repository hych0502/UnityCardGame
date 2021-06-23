using UnityEngine;
using System.Collections;


[CreateAssetMenu(menuName = "Elements/Damage")]
public class Damage : Element
{
    public int d;
    GameObject GM;
    public override void Active()
    {
        GM = GameObject.FindGameObjectWithTag("GM");

        if (GM.GetComponent<GameManager>().TargetObj.tag == "Enemy")
            GM.GetComponent<GameManager>().EnemyHP -= d;
        else if (GM.GetComponent<GameManager>().TargetObj.tag == "Player")
            GM.GetComponent<GameManager>().PlayerHP -= d;
            
        else
            GM.GetComponent<GameManager>().TargetObj.GetComponent<PeerScript>().HP -= d;
    }

    public override bool Activeable()
    {
        GM = GameObject.FindGameObjectWithTag("GM");
        if (GM.GetComponent<GameManager>().PeersE.Count > 0)
            return true;
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Elements/AP")]
public class AP : Element
{
    public int MaxAPup;
    public int APup;
    AP()
    {
        target = TargetSelection.None;
    }
    public override void Active()
    {
        GameObject GM = GameObject.FindGameObjectWithTag("GM");
        if (GM.GetComponent<GameManager>().GameTurn == Turn.Player)
        {
            if (GM.GetComponent<GameManager>().APmax < 10)
                GM.GetComponent<GameManager>().APmax += MaxAPup;

            if (GM.GetComponent<GameManager>().APmax < GM.GetComponent<GameManager>().CurrentAP + APup)
                GM.GetComponent<GameManager>().CurrentAP = GM.GetComponent<GameManager>().APmax;
            else
                GM.GetComponent<GameManager>().CurrentAP += APup;
        }

    }

    public override bool Activeable()
    {
        return true;
    }

}

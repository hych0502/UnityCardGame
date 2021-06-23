using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Elements/TurnActivation")]
public class TurnActivation : Element
{
    TurnActivation()
    {
        target = TargetSelection.None;
    }
    public override void Active()
    {
        GameObject GM = GameObject.Find("GameManager");
        if(GM.GetComponent<GameManager>().GameTurn == Turn.Player)
             GM.GetComponent<GameManager>().CantActive(GM.GetComponent<GameManager>().ActivatingCard.ID);
    }

    public override bool Activeable()
    {
        return true;
    }
}
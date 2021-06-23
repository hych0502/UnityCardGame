using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Elements/DrawCards")]
public class DrawCards : Element
{
    public int Cards;
    DrawCards()
    {
        target = TargetSelection.None;
    }
    public override void Active()
    {
        GameObject GM = GameObject.FindGameObjectWithTag("GM");
        if(GM.GetComponent<GameManager>().GameTurn == Turn.Player)
            for (int i = 0; i < Cards; i++)
                GM.GetComponent<GameManager>().CardDraw();
    }

    public override bool Activeable()
    {
        return true;
    }
}


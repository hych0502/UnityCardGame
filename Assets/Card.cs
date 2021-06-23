using UnityEngine;
using System.Collections;


public class Card : ScriptableObject
{
    public int ID;
    public Sprite Sprite;
    public string Cardname;
    public string Explain;
    public int Cost;
    [SerializeField]
    public Element[] element;
    public bool Activeable()
    {
        GameObject GM = GameObject.FindGameObjectWithTag("GM");
        for(int i = 0; i < element.Length; i++)
        {
            if (!element[i].Activeable())
                return false;

        }
        return true;
    }
}

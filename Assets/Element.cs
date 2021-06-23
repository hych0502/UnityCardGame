using UnityEngine;
using System.Collections;
using System;
public enum TargetSelection
{
    None,
    EnemyP,
    EnemyPL,
    PlayerP,
    PlayerPL
}
[Serializable]
public abstract class Element : ScriptableObject
{
    public abstract bool Activeable();
    public abstract void Active();
    public TargetSelection  target;
    public TargetSelection Target
    {
        get { return target; }

    }
}


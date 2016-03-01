using System;
using UnityEngine;

[Serializable]
public struct MonsterMaster
{
    [SerializeField]
    private int health;

    [SerializeField]
    private int attack;

    [SerializeField]
    private int recovery;

    public int Health
    {
        get
        {
            return health;
        }
    }

    public int Attack
    {
        get
        {
            return attack;
        }
    }

    public int Recovery
    {
        get
        {
            return recovery;
        }
    }
}
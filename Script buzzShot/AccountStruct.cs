using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AccountStruct
{
    private string name;
    private int follower;

    public string Name { get { return name; } }
    public int Follower { get { return follower; } }

    public AccountStruct(string name,int follower)
    {
        this.name = name;
        this.follower = follower;
    }

    public override string ToString()
    {
        return name + "," + follower;
    }
}


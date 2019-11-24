using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trainee
{
    public string name;
    public int id;
    public string interchange;

    public Trainee(string name, int id, string interchange)
    {
        this.name = name;
        this.id = id;
        this.interchange = interchange;
    }
}


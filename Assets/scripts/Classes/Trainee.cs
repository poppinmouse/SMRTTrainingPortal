using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Trainee
{
    //public string _id;
    public string name;
    public int id;
    public string interchange;
    public bool absent;

    public Trainee(string name, int id, string interchange, bool absent)
    {
        this.name = name;
        this.id = id;
        this.interchange = interchange;
        this.absent = absent;
    }
}


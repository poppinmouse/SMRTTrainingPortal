﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string username;
    public string password;

    public User(string username, string password)
    {
        this.username = username;
        this.password = password;      
    }
}

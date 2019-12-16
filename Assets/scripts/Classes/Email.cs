using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Email 
{
    public string interchangeAddress;
    public string trainerAddress;
    public string subject;
    public string body;

    public Email(string interchangeAddress, string trainerAddress, string subject, string body)
    {
        this.interchangeAddress = interchangeAddress;
        this.trainerAddress = trainerAddress;
        this.subject = subject;
        this.body = body;
    }

    public Email(string interchangeAddress, string subject, string body)
    {
        this.interchangeAddress = interchangeAddress;
        this.subject = subject;
        this.body = body;
    }
}

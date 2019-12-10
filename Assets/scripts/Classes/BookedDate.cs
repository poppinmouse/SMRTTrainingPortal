using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookedDate
{
    public string proposedDate;
    public bool hasApproved;

    public BookedDate(string proposedDate, bool hasApproved)
    {
        this.proposedDate = proposedDate;
        this.hasApproved = hasApproved;
    }
}

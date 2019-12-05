using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookedDate
{
    public int proposedDate;
    public bool hasApproved;

    public BookedDate(int proposedDate, bool hasApproved)
    {
        this.proposedDate = proposedDate;
        this.hasApproved = hasApproved;
    }
}

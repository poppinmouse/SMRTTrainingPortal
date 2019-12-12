using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booking 
{
    public string _id;
    public List<string> reservedDates;
    public BookedDate bookedDate;
    public int issueCode;
    public List<Trainee> trainees;
}

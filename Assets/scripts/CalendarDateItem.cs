using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CalendarDateItem : MonoBehaviour {

    public void OnDateItemClick()
    {
        CalendarController._calendarInstance.OnDateItemClick(System.Convert.ToInt32(GetComponentInChildren<Text>().text).ToString("00"));
    }
}

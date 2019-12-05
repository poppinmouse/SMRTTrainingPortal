using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CalendarDateItem : MonoBehaviour {

    public Color chosenColor;
    bool hasChosen = false;
    Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
    }

    public void OnDateItemButtonClick()
    {
        if(!hasChosen)
        {
            CalendarController._calendarInstance.OnDateItemAdd(System.Convert.ToInt32(GetComponentInChildren<Text>().text).ToString("00"));
            btn.image.color = chosenColor;
        }
        else
        {
            CalendarController._calendarInstance.OnDateItemRemove(System.Convert.ToInt32(GetComponentInChildren<Text>().text).ToString("00"));
            btn.image.color = btn.colors.normalColor;
        }

        hasChosen = !hasChosen;
    }

}

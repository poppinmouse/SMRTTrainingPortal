using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarController : MonoBehaviour
{
    public GameObject _calendarPanel;
    public Text _yearNumText;
    public Text _monthNumText;

    public GameObject _item;

    public List<GameObject> _dateItems = new List<GameObject>();
    const int _totalDateNum = 42;

    public List<string> reservedDates = new List<string>();
    public int selectedDate;

    private DateTime _dateTime;
    public static CalendarController _calendarInstance;

    public Dictionary<string, GameObject> dateLookUp = new Dictionary<string, GameObject>();

    void Start()
    {
        _calendarInstance = this;
        Vector3 startPos = _item.transform.localPosition;
        _dateItems.Clear();
        _dateItems.Add(_item);

        for (int i = 1; i < _totalDateNum; i++)
        {
            GameObject item = GameObject.Instantiate(_item) as GameObject;
            item.name = "Item" + (i + 1).ToString();
            item.transform.SetParent(_item.transform.parent);
            item.transform.localScale = Vector3.one;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localPosition = new Vector3((i % 7) * 31 + startPos.x, startPos.y - (i / 7) * 25, startPos.z);

            _dateItems.Add(item);            
        }

        _dateTime = DateTime.Now;

        Debug.Log(DateTime.Today.Month);

        CreateCalendar();

        _calendarPanel.SetActive(false);
    }

    void CreateCalendar()
    {
        //CheckPreviousYearMonthButtonStatus();
        //clear datelookup dictionary
        dateLookUp.Clear();
        DateTime firstDay = _dateTime.AddDays(-(_dateTime.Day - 1));
        int index = GetDays(firstDay.DayOfWeek);

        int date = 0;
        for (int i = 0; i < _totalDateNum; i++)
        {
            _dateItems[i].GetComponent<Button>().interactable = true; //clear out previous disable ones
            Text label = _dateItems[i].GetComponentInChildren<Text>();
            _dateItems[i].SetActive(false);

            if (i >= index)
            {
                DateTime thatDay = firstDay.AddDays(date);
                if (thatDay.Month == firstDay.Month)
                {
                    _dateItems[i].SetActive(true);

                    label.text = (date + 1).ToString();
                    date++;

                    if (_dateTime.Year < DateTime.Today.Year)
                    {
                        _dateItems[i].GetComponent<Button>().interactable = false;
                    }
                    else if (_dateTime.Year == DateTime.Today.Year)
                    {
                        if (_dateTime.Month < DateTime.Today.Month)
                        {
                            _dateItems[i].GetComponent<Button>().interactable = false;
                        }
                        else if (_dateTime.Month == DateTime.Today.Month)
                        {
                            if (date < DateTime.Today.Day)
                            {
                                _dateItems[i].GetComponent<Button>().interactable = false;
                            }
                        }
                        else
                        {
                            _dateItems[i].GetComponent<Button>().interactable = true;
                        }
                    }
                    else
                    {
                        _dateItems[i].GetComponent<Button>().interactable = true;
                    }

                    //add to datelookup dictionary
                    dateLookUp.Add(_dateTime.Month.ToString("00") + "-" + int.Parse(label.text).ToString("00") + "-" + _dateTime.Year.ToString(), _dateItems[i]);
                }
            }

        }
        _yearNumText.text = _dateTime.Year.ToString();
        _monthNumText.text = _dateTime.Month.ToString("00");

        for (int i = 0; i < GetController.getControllerInstance.dateList.Count; i++)
        { 
            if (dateLookUp.TryGetValue(GetController.getControllerInstance.dateList[i].ToString(), out GameObject dateObject))
            {
                dateLookUp[GetController.getControllerInstance.dateList[i].ToString()].GetComponent<Button>().interactable = false;
            }
            //else
            //{
            //    Debug.Log("cant find");
            //}
        }
    }

    int GetDays(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 1;
            case DayOfWeek.Tuesday: return 2;
            case DayOfWeek.Wednesday: return 3;
            case DayOfWeek.Thursday: return 4;
            case DayOfWeek.Friday: return 5;
            case DayOfWeek.Saturday: return 6;
            case DayOfWeek.Sunday: return 0;
        }

        return 0;
    }
    public void YearPrev()
    {
        _dateTime = _dateTime.AddYears(-1);
        CreateCalendar();
    }

    public void YearNext()
    {
        _dateTime = _dateTime.AddYears(1);
        CreateCalendar();
    }

    public void MonthPrev()
    {
        _dateTime = _dateTime.AddMonths(-1);
        CreateCalendar();
    }

    public void MonthNext()
    {
        _dateTime = _dateTime.AddMonths(1);
        CreateCalendar();
    }

    public void ShowCalendar(Text target)
    {
        _calendarPanel.SetActive(true);
        _target = target;
        //_calendarPanel.transform.position = new Vector3(965, 475, 0);//Input.mousePosition-new Vector3(0,120,0);
    }

    Text _target;
    public void OnDateItemAdd(string day)
    {
        reservedDates.Add(_calendarInstance._monthNumText.text + "-" + day + "-" + _calendarInstance._yearNumText.text);
        //_target.text = _yearNumText.text + "Year" + _monthNumText.text + "Month" + day+"Day";
        //if (int.TryParse(day + _calendarInstance._monthNumText.text + _calendarInstance._yearNumText.text, out int result))
        //{
        //    //selectedDate = result;
        //    reservedDates.Add(result);
        //}
        //_calendarPanel.SetActive(false);
    }
    public void OnDateItemRemove(string day)
    {
        reservedDates.Remove(_calendarInstance._monthNumText.text + "-" + day + "-" + _calendarInstance._yearNumText.text);
        //if (int.TryParse(day + _calendarInstance._monthNumText.text + _calendarInstance._yearNumText.text, out int result))
        //{
        //    reservedDates.Remove(result);
        //}
    }
}

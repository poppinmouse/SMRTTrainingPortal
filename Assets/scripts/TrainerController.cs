using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class TrainerController : MonoBehaviour
{
    public string url;
    //public List<Booking> bookingList = new List<Booking>();
    public GameObject button;
    //public GameObject toConfirmPopUp;
    public RootObject theBookings;
    public float btnOffset;
    public Button attendanceButton;

    void Start()
    {
        //theBookings = new RootObject();
        //StartCoroutine(GetRequest(url + "/bookings/"));
        StartCoroutine(Generate());
        attendanceButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Attendance");
        });
    }

    IEnumerator Generate()
    {
        yield return GetBookingsManager.Instance.GetRequest();

        int btnCount = 0;
        for (int i = 0; i < GetBookingsManager.Instance.theBookings.bookings.Count; i++)
        {
            if (GetBookingsManager.Instance.theBookings.bookings[i].issueCode != 0)
            {
                GameObject btn = Instantiate(button);//instantiate the button
                btn.transform.SetParent(button.transform.parent);           
                btn.transform.position = new Vector2(button.transform.position.x, button.transform.position.y - (btnOffset * btnCount));
                btnCount++;
                btn.SetActive(true);

                //issueCode reference
                //0 = no issue
                //1 = to confirm
                //2 = to remind
                //3 = to rebook
                switch (GetBookingsManager.Instance.theBookings.bookings[i].issueCode)
                {
                    case 1:
                        btn.GetComponentInChildren<Text>().text = "To Confirm";
                        btn.GetComponent<ButtonIndex>().index = i;
                        btn.GetComponent<Button>().onClick.AddListener(() => {
                            GetBookingsManager.Instance.selectedIndex = btn.GetComponent<ButtonIndex>().index;
                            SceneManager.LoadScene("Confirm");
                        });
                        break;
                    case 2:
                        btn.GetComponentInChildren<Text>().text = "To Remind";
                        btn.GetComponent<ButtonIndex>().index = i;
                        btn.GetComponent<Button>().onClick.AddListener(() => {
                            GetBookingsManager.Instance.selectedIndex = btn.GetComponent<ButtonIndex>().index;
                            SceneManager.LoadScene("Remind");
                        });
                        break;
                    case 3:
                        btn.GetComponentInChildren<Text>().text = "To Rebook";
                        btn.GetComponent<ButtonIndex>().index = i;
                        btn.GetComponent<Button>().onClick.AddListener(() => {
                            GetBookingsManager.Instance.selectedIndex = btn.GetComponent<ButtonIndex>().index;
                            SceneManager.LoadScene("Rebook");
                        });
                        break;
                    default:
                        break;
                }
            }
        }
    }

}


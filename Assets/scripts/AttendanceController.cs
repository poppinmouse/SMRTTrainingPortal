using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class AttendanceController : MonoBehaviour
{   
    public Toggle toggle;
    //public string url;
    public Booking booking;
    public string bookingId;

    public GameObject button;
    public RootObject theBookings;

    public GameObject bg;
    public GameObject submitBtn;
    public int btnOffset;

    private int clickedIndex;

    public Transform toggles;

    void Start()
    {
        toggle = FindObjectOfType<Toggle>();
        toggle.gameObject.SetActive(false);
        Generate();
    }

    void Generate()
    {
        int btnCount = 0;

        for (int i = 0; i < GetBookingsManager.Instance.theBookings.bookings.Count; i++)
        {
            if (GetBookingsManager.Instance.theBookings.bookings[i].bookedDate.hasApproved)
            {
                GameObject btn = Instantiate(button);//instantiate the button
                btn.transform.SetParent(button.transform.parent);
                btn.transform.position = new Vector2(button.transform.position.x, button.transform.position.y - btnOffset * btnCount);
                btnCount++;
                btn.SetActive(true);
                btn.GetComponentInChildren<Text>().text = GetBookingsManager.Instance.theBookings.bookings[i].bookedDate.proposedDate;
                btn.GetComponent<ButtonIndex>().index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    clickedIndex = btn.GetComponent<ButtonIndex>().index;
                    bookingId = GetBookingsManager.Instance.theBookings.bookings[clickedIndex]._id;
                    bg.SetActive(true);
                    PopulateTrainees();
                    submitBtn.SetActive(true);
                    submitBtn.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        StartCoroutine(Upload());
                    });
                });
            }
        }
    }

    void PopulateTrainees()
    { 
        for (int i = 0; i < GetBookingsManager.Instance.theBookings.bookings[clickedIndex].trainees.Count; i++)
        {
            GameObject toggleBtn = Instantiate(toggle.gameObject, toggles.gameObject.transform);
            toggleBtn.transform.position = new Vector2(toggle.transform.position.x, toggle.transform.position.y - 50 * i);
            toggleBtn.GetComponentInChildren<Text>().text = GetBookingsManager.Instance.theBookings.bookings[clickedIndex].trainees[i].name + "     ";
            toggleBtn.GetComponentInChildren<Text>().text += GetBookingsManager.Instance.theBookings.bookings[clickedIndex].trainees[i].id + "     ";
            toggleBtn.GetComponentInChildren<Text>().text += GetBookingsManager.Instance.theBookings.bookings[clickedIndex].trainees[i].interchange;
            toggleBtn.GetComponent<ToggleIndex>().index = i;
            toggleBtn.SetActive(true);
        }     
    }

    IEnumerator Upload()
    {
        List<Trainee> trainees = new List<Trainee>();
        bool fullAttendance = true;
        foreach (Transform toggle in toggles)
        {
            if (!toggle.GetComponent<Toggle>().isOn)
            {
                if (fullAttendance)
                {
                    fullAttendance = false;
                }

                GetBookingsManager.Instance.theBookings.bookings[clickedIndex].trainees[toggle.GetComponent<ToggleIndex>().index].absent = true;
              
            }

            trainees.Add(GetBookingsManager.Instance.theBookings.bookings[clickedIndex].trainees[toggle.GetComponent<ToggleIndex>().index]);
        }

        if(fullAttendance)
        {
            yield return null;
        }
        else
        {
            WWWForm form = new WWWForm();

            string absenteesJson = JsonConvert.SerializeObject(trainees);
            form.AddField("Absentees", absenteesJson);

            UnityWebRequest www = UnityWebRequest.Post(NetworkManager.Instance.url + "/bookings/" + bookingId + "/absentees", form);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
      
    }

}

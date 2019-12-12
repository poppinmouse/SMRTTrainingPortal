using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InterchangeBookingController : MonoBehaviour
{
    public ToggleGroup toggleGroup;
    public Toggle toggle;
    public string url;
    public Booking booking ;
    public string bookingId;

    public GameObject button;  
    public RootObject theBookings;

    public GameObject bg;
    public GameObject submitBtn;

    void Start()
    {
        toggleGroup = FindObjectOfType<ToggleGroup>();
        toggle = FindObjectOfType<Toggle>();
        toggle.gameObject.SetActive(false);
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        yield return GetBookingsManager.Instance.GetRequest();

        for (int i = 0; i < GetBookingsManager.Instance.theBookings.bookings.Count; i++)
        {
            if (GetBookingsManager.Instance.theBookings.bookings[i].bookedDate.proposedDate == "")
            {
                GameObject btn = Instantiate(button);//instantiate the button
                btn.transform.SetParent(button.transform.parent);
                btn.transform.position = new Vector2(button.transform.position.x, button.transform.position.y - 100 * i);
                btn.SetActive(true);
                btn.GetComponentInChildren<Text>().text = GetBookingsManager.Instance.theBookings.bookings[i]._id;
                btn.GetComponent<Button>().onClick.AddListener(() => {
                    bookingId = btn.GetComponentInChildren<Text>().text;
                    bg.SetActive(true);
                    StartCoroutine(GetRequest(url + "/bookings/" + bookingId));
                    submitBtn.SetActive(true);
                    submitBtn.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        StartCoroutine(Upload());
                    });
                });
            }
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                booking = JsonUtility.FromJson<Booking>(webRequest.downloadHandler.text);
                for (int i = 0; i < booking.reservedDates.Count; i++)
                {
                    GameObject radioBtn = Instantiate(toggle.gameObject);
                    radioBtn.transform.SetParent(toggle.transform.parent);
                    radioBtn.transform.position = new Vector2(toggle.transform.position.x, toggle.transform.position.y - 50*i);
                    var dt = DateTime.ParseExact(booking.reservedDates[i], "MM-dd-yyyy", CultureInfo.InvariantCulture);
                    radioBtn.GetComponentInChildren<Text>().text = dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    radioBtn.SetActive(true);
                }
            }
        }   
    }

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();

        var dt = DateTime.ParseExact(toggleGroup.GetActive().GetComponentInChildren<Text>().text, "dd-MMM-yyyy", CultureInfo.InvariantCulture);

        BookedDate bookedDate = new BookedDate(dt.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture), false);
        //BookedDate bookedDate = new BookedDate(toggleGroup.GetActive().GetComponentInChildren<Text>().text, true);

        string bookedDateJson = JsonUtility.ToJson(bookedDate);
        form.AddField("BookedDate", bookedDateJson);

        UnityWebRequest www = UnityWebRequest.Post(url + "/bookings/" + bookingId + "/bookeddate", form);

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

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Upload());
        }
    }
}

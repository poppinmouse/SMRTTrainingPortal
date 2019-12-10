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

    void Start()
    {
        toggleGroup = FindObjectOfType<ToggleGroup>();
        toggle = FindObjectOfType<Toggle>();
        toggle.gameObject.SetActive(false);
        StartCoroutine(GetRequest(url + "/bookings/" + bookingId));
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
                booking = JsonUtility.FromJson<Booking>(webRequest.downloadHandler.text);
                for (int i = 0; i < booking.reservedDates.Count; i++)
                {
                    GameObject radioBtn = Instantiate(toggle.gameObject);
                    radioBtn.transform.SetParent(toggle.transform.parent);
                    radioBtn.transform.position = new Vector2(toggle.transform.position.x, toggle.transform.position.y - 50*i);
                    //var dt = DateTime.ParseExact(booking.reservedDates[i].ToString("D8"), "ddMMyyyy", CultureInfo.InvariantCulture);
                    //radioBtn.GetComponentInChildren<Text>().text = dt.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                    radioBtn.GetComponentInChildren<Text>().text = booking.reservedDates[i];
                    radioBtn.SetActive(true);
                }
            }
        }   
    }

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();

        //var dt = DateTime.ParseExact(toggleGroup.GetActive().GetComponentInChildren<Text>().text, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        //BookedDate bookedDate = new BookedDate(int.Parse(dt.ToString("ddMMyyyy", CultureInfo.InvariantCulture)), true);
        BookedDate bookedDate = new BookedDate(toggleGroup.GetActive().GetComponentInChildren<Text>().text, true);

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

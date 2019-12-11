using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class TrainerController : MonoBehaviour
{
    public string url;
    public List<Booking> bookingList = new List<Booking>();
    public GameObject button;

    void Start()
    {
        StartCoroutine(GetRequest(url + "/bookings/"));
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
                RootObject theBookings = new RootObject();
                JsonConvert.PopulateObject(webRequest.downloadHandler.text, theBookings);
                for (int i = 0; i < theBookings.bookings.Count; i++)
                {
                    if (theBookings.bookings[i].issueCode != 0)
                    {
                        GameObject btn = Instantiate(button);//instantiate the button
                        btn.transform.SetParent(button.transform.parent);
                        btn.transform.position = new Vector2(button.transform.position.x, button.transform.position.y - 100 * i);
                        btn.SetActive(true);

                        switch (theBookings.bookings[i].issueCode)
                        {
                            case 1:
                                btn.GetComponentInChildren<Text>().text = "To Remind";
                                break;
                            case 2:
                                btn.GetComponentInChildren<Text>().text = "To Confirm";
                                break;
                            case 3:
                                btn.GetComponentInChildren<Text>().text = "To Rebook";
                                break;
                            default:
                                break;
                        }
                    }
                }
                
            }
        }
    }
}


public class RootObject
{
    public List<Booking> bookings { get; set; }
}
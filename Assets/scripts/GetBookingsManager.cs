using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class GetBookingsManager : MonoBehaviour
{
    public static GetBookingsManager Instance { get; private set; }

    public string url;
    public RootObject theBookings;
    public int selectedIndex;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }    
    }

    void Start()
    {
        theBookings = new RootObject();
        //StartCoroutine(GetRequest());
    }

    public IEnumerator GetRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url + "/bookings/"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                JsonConvert.PopulateObject(webRequest.downloadHandler.text, theBookings);
                Debug.Log(theBookings.bookings[0].trainees[0].id);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetController : MonoBehaviour
{
    //public string url;
    public List<string> dateList;
    public static GetController getControllerInstance;

    void Start()
    {
        getControllerInstance = this;
        StartCoroutine(GetRequest(NetworkManager.Instance.url + "/Dates"));
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
                dateList = JsonUtility.FromJson<BlockedDates>(webRequest.downloadHandler.text).dates;
                for(int i = 0; i < dateList.Count; i++)
                {
                    if (CalendarController._calendarInstance.dateLookUp.TryGetValue(dateList[i], out GameObject dateObject))
                    {
                        CalendarController._calendarInstance.dateLookUp[dateList[i]].GetComponent<Button>().interactable = false;
                    }
                    //char[] charArray = dateList[i].ToString("D8").ToCharArray();
                    //for (int j = 0; j < 2; j++)
                    //{
                    //    Debug.Log(charArray[j]);
                    //}
                    //Debug.Log(dateList[i]);
                    //if (CalendarController._calendarInstance.dateLookUp.TryGetValue(dateList[i].ToString(), out GameObject dateObject))
                    //{
                    //    CalendarController._calendarInstance.dateLookUp[dateList[i].ToString()].GetComponent<Button>().interactable = false;
                    //}
                    //else
                    //{
                    //    Debug.Log("cant find");
                    //}
                   
                }
               
            }
        }
    }

}

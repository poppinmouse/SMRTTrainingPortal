using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetController : MonoBehaviour
{
    public List<int> dateList;
    public static GetController getControllerInstance;

    void Start()
    {
        getControllerInstance = this;
        StartCoroutine(GetRequest("http://localhost:3000/Dates"));
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
                dateList = JsonUtility.FromJson<AppointedDates>(webRequest.downloadHandler.text).dates;
                for(int i = 0; i < dateList.Count; i++)
                {
                    //char[] charArray = dateList[i].ToString("D8").ToCharArray();
                    //for (int j = 0; j < 2; j++)
                    //{
                    //    Debug.Log(charArray[j]);
                    //}
                    //Debug.Log(dateList[i]);
                    if(CalendarController._calendarInstance.dateLookUp.TryGetValue(dateList[i].ToString(), out GameObject dateObject))
                    {
                        CalendarController._calendarInstance.dateLookUp[dateList[i].ToString()].GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        Debug.Log("cant find");
                    }
                    

                }
               
            }
        }
    }

}

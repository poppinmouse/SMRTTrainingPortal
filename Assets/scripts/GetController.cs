using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetController : MonoBehaviour
{
    void Start()
    {
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
                List<int> dateList = JsonUtility.FromJson<AppointedDates>(webRequest.downloadHandler.text).dates;
                for(int i = 0; i < dateList.Count; i++)
                {
        
                    char[] charArray = dateList[i].ToString("D8").ToCharArray();
                    for (int j = 0; j < 2; j++)
                    {
                        Debug.Log(charArray[j]);
                    }
                }
               
            }
        }
    }

}

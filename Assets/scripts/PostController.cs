using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PostController : MonoBehaviour
{
    public string url;
    public Dropdown trainingTypeDD;

    public InputField[] nameFields;
    public InputField[] iDFields;
    public InputField[] interchangeFields;

    [SerializeField]
    public Trainee[] trainees;

    private string route;

    private int numberOfTrainees;

    private void Start()
    {
        numberOfTrainees = nameFields.Length;
        trainees = new Trainee[numberOfTrainees];
    }

    public void Submit()
    {
        StartCoroutine(Upload());
        //locally disable the selected date for this session
        //CalendarController._calendarInstance.dateLookUp[CalendarController._calendarInstance.selectedDate.ToString()].GetComponent<Button>().interactable = false;
    }

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();

        int[] arr = CalendarController._calendarInstance.reservedDates.ToArray();
        string arrJson = JsonHelper.ToJson(arr);
        form.AddField("ReservedDates", arrJson);

        form.AddField("BookedDate", CalendarController._calendarInstance.selectedDate);

        for (int i = 0; i < 2; i++)
        {
            string name = nameFields[i].text;
            int iD = 0;
            if (int.TryParse(iDFields[i].text, out int result))
            {
                iD = result;
            }
            string interchange = interchangeFields[i].text;

            trainees[i] = new Trainee(name, iD, interchange);

            //form.AddField("Name" + i, name);
            //form.AddField("Id" + i, iD);
            //form.AddField("Interchange" + i, interchange);
        }

        string traineesJson = JsonHelper.ToJson(trainees);
        Debug.Log(traineesJson);

        form.AddField("Trainees", traineesJson);

        switch (trainingTypeDD.value)
        {
            case 0:
                route = trainingTypeDD.options[0].text;
                break;

            case 1:
                route = trainingTypeDD.options[1].text;
                break;

            case 2:
                route = trainingTypeDD.options[2].text;
                break;

            default:
                break;
        }

        UnityWebRequest www = UnityWebRequest.Post(url + "/" + route, form);
        
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

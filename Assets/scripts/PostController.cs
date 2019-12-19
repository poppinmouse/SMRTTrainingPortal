using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PostController : MonoBehaviour
{
    public string url;
    public Dropdown trainingTypeDD;
    public static PostController postControllerInstance;

    public InputField[] nameFields;
    public InputField[] iDFields;
    public InputField[] interchangeFields;

    [SerializeField]
    public List<Trainee> traineeList;

    public Dropdown[] addressDDs;
    public InputField subject;
    public InputField body;

    public Email email;
    public GameObject EmailTemplate;
    public GameObject CompleteNoti;

    private string route;


    private void Awake()
    {
        postControllerInstance = this;
    }
    private void Start()
    {        
        traineeList = new List<Trainee>();
    }

    public void Submit()
    {
        StartCoroutine(Upload());
        GenerateEmail();
        EmailTemplate.SetActive(true);
        //locally disable the selected date for this session
        //CalendarController._calendarInstance.dateLookUp[CalendarController._calendarInstance.selectedDate.ToString()].GetComponent<Button>().interactable = false;
    }

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();

        string[] arr = CalendarController._calendarInstance.reservedDates.ToArray();
        string arrJson = JsonHelper.ToJson(arr);
        form.AddField("ReservedDates", arrJson);

        for (int i = 0; i < nameFields.Length; i++)
        {
            if(nameFields[i].text == "")
            {
                Debug.Log("not count");
            }
            else
            {
                string name = nameFields[i].text;
                int iD = 0;
                if (int.TryParse(iDFields[i].text, out int result))
                {
                    iD = result;
                }
                string interchange = interchangeFields[i].text;
                bool absent = false;

                traineeList.Add(new Trainee(name, iD, interchange, absent));
            }
        }

        string traineesJson = JsonHelper.ToJson(traineeList.ToArray());
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

    void GenerateEmail()
    {
        string emailSubject;
        string emailBody;
        string trainingType = trainingTypeDD.options[trainingTypeDD.value].text;

        emailSubject = string.Format("Booking for {0} Training", trainingType);

        string appointedDates = "";
        for (int i = 0; i < CalendarController._calendarInstance.reservedDates.Count; i++)
        {
            var dt = DateTime.ParseExact(CalendarController._calendarInstance.reservedDates[i], "MM-dd-yyyy", CultureInfo.InvariantCulture);

            if (i == 0)
            {
                appointedDates += dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }
            else if(i == CalendarController._calendarInstance.reservedDates.Count-1)
            {
                appointedDates += " and " + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                appointedDates += ", " + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }
        }

        emailBody = string.Format("Hi Interchange Personal,\n\n " +
            "Here are the Bus Captains that need to attend the {0} Training.\n\n " +
            "{1}\n\n" +
            "We are avaliable for {2}. Please kindly choose a date that suitable and sent email to {3}\n\n" +
            "Thanks and Regards,\n" +
            "Adele",
            trainingType,
            TraineesToString(traineeList),
            appointedDates,
            addressDDs[1].options[addressDDs[1].value].text        
            );

        subject.text = emailSubject;
        body.text = emailBody;
    }

    IEnumerator SentEmailCR()
    {
        WWWForm form = new WWWForm();

        Email email = new Email(addressDDs[0].options[addressDDs[0].value].text, addressDDs[1].options[addressDDs[1].value].text, subject.text, body.text);

        string emailJson = JsonUtility.ToJson(email);

        form.AddField("Email", emailJson);

        UnityWebRequest www = UnityWebRequest.Post(url + "/" + "email", form);

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

    public void SendEmail()
    {
        StartCoroutine(SentEmailCR());
        CompleteNoti.SetActive(true);
    }

    string TraineesToString(List<Trainee> traineeList)
    {
        string traineesString = "";
        for (int i = 0; i < traineeList.Count; i++)
        {
            traineesString += traineeList[i].name + "     " + traineeList[i].id + "     " + traineeList[i].interchange + "\n";
        }
        return traineesString;
    }
}

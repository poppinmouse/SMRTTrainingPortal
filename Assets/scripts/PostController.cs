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
    public Trainee[] trainees;

    public Dropdown[] addressDDs;
    public InputField subject;
    public InputField body;

    public Email email;
    public GameObject EmailTemplate;
    public GameObject CompleteNoti;

    private string route;

    private int numberOfTrainees;

    private void Awake()
    {
        postControllerInstance = this;
    }
    private void Start()
    {        
        numberOfTrainees = nameFields.Length;
        trainees = new Trainee[numberOfTrainees];
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

        int[] arr = CalendarController._calendarInstance.reservedDates.ToArray();
        string arrJson = JsonHelper.ToJson(arr);
        form.AddField("ReservedDates", arrJson);

        BookedDate bookedDate = new BookedDate(13122019, true);
        string bookedDateJson = JsonUtility.ToJson(bookedDate);
        form.AddField("BookedDate", bookedDateJson);
        //form.AddField("BookedDate", CalendarController._calendarInstance.selectedDate);

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

    void GenerateEmail()
    {
        string emailSubject;
        string emailBody;
        string trainingType = trainingTypeDD.options[trainingTypeDD.value].text;

        emailSubject = string.Format("Booking for {0} Training", trainingType);

        string appointedDates = "";
        for (int i = 0; i < CalendarController._calendarInstance.reservedDates.Count; i++)
        {
            var dt = DateTime.ParseExact(CalendarController._calendarInstance.reservedDates[i].ToString("D8"), "ddMMyyyy", CultureInfo.InvariantCulture);
            if(i == 0)
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
            "{1}  {2}   {3}\n\n" +
            "{4}  {5}   {6}\n\n" +
            "{7}  {8}   {9}\n\n" +
            "We are avaliable for {10}. Please kindly choose a date that suitable and sent email to {11}\n\n" +
            "Thanks and Regards,\n" +
            "Adele",
            trainingType, 
            nameFields[0].text, iDFields[0].text, interchangeFields[0].text,
            nameFields[1].text, iDFields[1].text, interchangeFields[1].text,
            nameFields[2].text, iDFields[2].text, interchangeFields[2].text,
            appointedDates,
            addressDDs[1].options[addressDDs[1].value].text);

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
}

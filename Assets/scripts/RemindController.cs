using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RemindController : MonoBehaviour
{
    public GameObject calendarBtn;
    public InputField subject;
    public InputField body;
    public Dropdown[] addressDDs;
    public Button sendButton;
    public Button resolveAnotherIssueButton;
    public Button logOutButton;
    public GameObject PopUp;

    private void Awake()
    {
        CalendarDateItem.DateChosenEvent += OnDateChosen;
    }

    private void OnDestroy()
    {
        CalendarDateItem.DateChosenEvent -= OnDateChosen;
    }

    private void OnDateChosen()
    {
        GenerateEmail();
    }

    void Start()
    {
        GenerateEmail();

        sendButton.onClick.AddListener(() => {
            StartCoroutine(SentEmailCR());
            StartCoroutine(IssueSolvedCR());
            PopUp.SetActive(true);
        });

        resolveAnotherIssueButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Trainer");
        });
    }


    void GenerateEmail()
    {
        string emailSubject;
        string emailBody;
          
        emailSubject = string.Format("Please Rebook the Training");
      
        emailBody = string.Format("Hi Interchange Personal,\n\n " +
            "This is the reminder email regarding about the {0} Training.\n\n " +
            "You have missed the training and need to rebook again. \n\n" +
            "We are avaliable for {1}. Please kindly choose a date that suitable and reply.\n\n" +
            "Thanks and Regards,\n" +
            "Janson",
             DateConverter(GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex].reservedDates),
             DateConverter(CalendarController._calendarInstance.reservedDates)
            );

        subject.text = emailSubject;
        body.text = emailBody;
    }

    private string DateConverter(List<string> dates)
    {
        string datesString = "";

        for (int i = 0; i < dates.Count ; i++)
        {
            var dt = DateTime.ParseExact(dates[i], "MM-dd-yyyy", CultureInfo.InvariantCulture);

            if (i == 0)
            {
                datesString += dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }
            else if (i == dates.Count - 1)
            {
                datesString += " and " + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                datesString += ", " + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }
        }
        return datesString;
    }

    IEnumerator SentEmailCR()
    {
        WWWForm form = new WWWForm();

        Email email = new Email(addressDDs[0].options[addressDDs[0].value].text, subject.text, body.text);

        string emailJson = JsonUtility.ToJson(email);

        form.AddField("Email", emailJson);

        UnityWebRequest www = UnityWebRequest.Post("localhost:3000" + "/" + "email", form);

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

    IEnumerator IssueSolvedCR()
    {
        WWWForm form = new WWWForm();

        form.AddField("Code", "remind");

        UnityWebRequest www = UnityWebRequest.Post("localhost:3000" + "/bookings/" + GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex]._id + "/issue", form);

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

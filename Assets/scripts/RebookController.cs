using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RebookController : MonoBehaviour
{
    public GameObject calendarBtn;
    public InputField subject;
    public InputField body;
    public Dropdown[] addressDDs;
    public Button sendButton;
    public Button resolveAnotherIssueButton;
    public Button logOutButton;
    public GameObject PopUp;
    private Booking thisBooking;
    Trainee[] traineeArr;

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
        thisBooking = GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex];
        GenerateEmail();

        sendButton.onClick.AddListener(() => {
            StartCoroutine(SentEmailCR());
            StartCoroutine(UploadRebooking());
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
            "The below Bus Captains were absent on {0} Training.\n\n " +
            "{1}\n" +
            "Please kindly give us the reason and rebook the training. \n\n" +
            "We are avaliable for {2}. Please kindly choose a date that suitable and reply.\n\n" +
            "Thanks and Regards,\n" +
            "Janson",
             thisBooking.bookedDate.proposedDate,
             TraineesToString(GetAbsentTrainees()),
             DateConverter(CalendarController._calendarInstance.reservedDates)
            );

        subject.text = emailSubject;
        body.text = emailBody;
    }

    private string DateConverter(List<string> dates)
    {
        string datesString = "";

        for (int i = 0; i < dates.Count; i++)
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

    List<Trainee> GetAbsentTrainees()
    {
        List<Trainee> absentTrainee = new List<Trainee>();
        for (int i = 0; i < thisBooking.trainees.Count; i++)
        {
           if(thisBooking.trainees[i].absent)
           {
                absentTrainee.Add(thisBooking.trainees[i]);
           }
        }
        return absentTrainee;
    }

    string TraineesToString(List<Trainee> traineeList)
    {
        string traineesString = "";
        traineeArr = traineeList.ToArray();
        for (int i = 0; i < traineeArr.Length; i++)
        {
            traineesString += traineeArr[i].name + "     " + traineeArr[i].id + "     " + traineeArr[i].interchange + "\n";
        }
        return traineesString;
    }

    IEnumerator SentEmailCR()
    {
        WWWForm form = new WWWForm();

        Email email = new Email(addressDDs[0].options[addressDDs[0].value].text, subject.text, body.text);

        string emailJson = JsonUtility.ToJson(email);

        form.AddField("Email", emailJson);

        UnityWebRequest www = UnityWebRequest.Post(NetworkManager.Instance.url + "/" + "email", form);

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

    IEnumerator UploadRebooking()
    {
        WWWForm form = new WWWForm();

        string[] arr = CalendarController._calendarInstance.reservedDates.ToArray();
        string arrJson = JsonHelper.ToJson(arr);
        form.AddField("ReservedDates", arrJson);

        traineeArr = GetAbsentTrainees().ToArray();

        for (int i = 0; i < traineeArr.Length; i++)
        {
            traineeArr[i].absent = false; 
        }

        string traineesJson = JsonHelper.ToJson(traineeArr);

        form.AddField("Trainees", traineesJson);

        UnityWebRequest www = UnityWebRequest.Post(NetworkManager.Instance.url + "/" + "ODVL", form);

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

        form.AddField("Code", "rebook");

        UnityWebRequest www = UnityWebRequest.Post(NetworkManager.Instance.url + "/bookings/" + GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex]._id + "/issue", form);

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

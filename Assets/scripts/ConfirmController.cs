using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmController : MonoBehaviour
{
    public Text message;
    public Button confirm;
    public Button reject;
    public GameObject emailBG;
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
        GenerateEmail(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        message.text = string.Format("Interchange requested for ODVL training on {0}. What would you like to do?", GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex].bookedDate.proposedDate);

        confirm.onClick.AddListener(() => {
            StartCoroutine(UpdateConfirmCR());
            emailBG.SetActive(true);
            calendarBtn.SetActive(false);
            GenerateEmail(true);
        });

        reject.onClick.AddListener(() => {
            emailBG.SetActive(true);
            calendarBtn.SetActive(true);
            GenerateEmail(false);
        });

        sendButton.onClick.AddListener(() => {
            StartCoroutine(SentEmailCR());
            PopUp.SetActive(true);
        });

        resolveAnotherIssueButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Trainer");
        });
    }

    IEnumerator UpdateConfirmCR()
    {
        WWWForm form = new WWWForm();
        BookedDate bookedDate = new BookedDate(GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex].bookedDate.proposedDate, true);

        string bookedDateJson = JsonUtility.ToJson(bookedDate);
        form.AddField("BookedDate", bookedDateJson);

        UnityWebRequest www = UnityWebRequest.Post(NetworkManager.Instance.url + "/bookings/" + GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex]._id + "/bookeddate", form);

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

    void GenerateEmail(bool isComfirmation)
    {
        string emailSubject;
        string emailBody;
        if(isComfirmation)
        {
            emailSubject = string.Format("Confirmation for Training");
            emailBody = string.Format("Hi Interchange Personal,\n\n " +
                "Your booking for {0} for Training is confirm.\n\n " +
                "Please kindly inform the Bus Captains to attend\n\n" +
                "Thanks and Regards,\n" +
                "Janson",
                GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex].bookedDate.proposedDate
               );
        }
        else
        {
            emailSubject = string.Format("Please Rebook the Training");

            string appointedDates = "";
            for (int i = 0; i < CalendarController._calendarInstance.reservedDates.Count; i++)
            {
                //var dt = DateTime.ParseExact(CalendarController._calendarInstance.reservedDates[i].ToString("D8"), "ddMMyyyy", CultureInfo.InvariantCulture);
                var dt = DateTime.ParseExact(CalendarController._calendarInstance.reservedDates[i], "MM-dd-yyyy", CultureInfo.InvariantCulture);

                if (i == 0)
                {
                    appointedDates += dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                }
                else if (i == CalendarController._calendarInstance.reservedDates.Count - 1)
                {
                    appointedDates += " and " + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    appointedDates += ", " + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                }
            }

            emailBody = string.Format("Hi Interchange Personal,\n\n " +
                "We are sorry that {0} for Training is unavaliable.\n\n " +
                "We are avaliable for {1}. Please kindly choose a date that suitable and reply.\n\n" +
                "Thanks and Regards,\n" +
                "Janson",
                GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex].bookedDate.proposedDate,
                appointedDates
               );
        }

        subject.text = emailSubject;
        body.text = emailBody;
    }

    IEnumerator SentEmailCR()
    {
        WWWForm form = new WWWForm();

        Email email = new Email(addressDDs[0].options[addressDDs[0].value].text, subject.text, body.text);

        string emailJson = JsonUtility.ToJson(email);

        form.AddField("Email", emailJson);

        UnityWebRequest www = UnityWebRequest.Post(NetworkManager.Instance.url + " /" + "email", form);

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

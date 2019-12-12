using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
    }

    IEnumerator UpdateConfirmCR()
    {
        WWWForm form = new WWWForm();
        BookedDate bookedDate = new BookedDate(GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex].bookedDate.proposedDate, true);

        string bookedDateJson = JsonUtility.ToJson(bookedDate);
        form.AddField("BookedDate", bookedDateJson);

        UnityWebRequest www = UnityWebRequest.Post("localhost:3000" + "/bookings/" + GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex]._id + "/bookeddate", form);

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
            emailBody = string.Format("Hi Interchange Personal,\n\n " +
                "We are sorry that {0} for Training is unavaliable.\n\n " +
                "Please rebook.\n\n" +
                "Thanks and Regards,\n" +
                "Janson",
                GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex].bookedDate.proposedDate
               );
        }

        subject.text = emailSubject;
        body.text = emailBody;
    }
}

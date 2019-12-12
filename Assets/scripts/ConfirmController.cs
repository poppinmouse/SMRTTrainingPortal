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

    // Start is called before the first frame update
    void Start()
    {
        message.text = string.Format("Interchange requested for ODVL training on {0}. What would you like to do?", GetBookingsManager.Instance.theBookings.bookings[GetBookingsManager.Instance.selectedIndex].bookedDate.proposedDate);
        confirm.onClick.AddListener(() => {
            StartCoroutine(UpdateConfirmCR());
            emailBG.SetActive(true);
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
}

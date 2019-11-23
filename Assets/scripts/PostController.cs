using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PostController : MonoBehaviour
{
    public Dropdown trainingTypeDD;

    public InputField nameField;
    public InputField iDField;
    public InputField interchangeField;

    private string route;

    public void Submit()
    {
        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        string name = nameField.text;
        int iD = 0;
        if (int.TryParse(iDField.text, out int result))
        {
            iD = result;
        }
        string interchange = interchangeField.text;

        WWWForm form = new WWWForm();
        form.AddField("Name", name);
        form.AddField("Id", iD);
        form.AddField("Interchange", interchange);

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

        UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000" + "/" +route, form);
        
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

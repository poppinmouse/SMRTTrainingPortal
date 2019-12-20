using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    public string url;
    public InputField username;
    public InputField password;
    public Button login;

    private void Start()
    {
        login.onClick.AddListener(() =>
        {
            StartCoroutine(PostLogin());
        });
    }

    IEnumerator PostLogin()
    {
        WWWForm form = new WWWForm();

        User user = new User(username.text, password.text);

        string userJson = JsonConvert.SerializeObject(user);

        form.AddField("User", userJson);

        UnityWebRequest www = UnityWebRequest.Post(url + "/" + "login", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            switch (www.downloadHandler.text)
            {
                case "admin":
                    SceneManager.LoadScene("Admin");
                        break;
                case "trainer":
                    SceneManager.LoadScene("Trainer");
                    break;
                case "interchange":
                    SceneManager.LoadScene("Interchange");
                    break;
                default:
                    break;
            }
        }
    }

}

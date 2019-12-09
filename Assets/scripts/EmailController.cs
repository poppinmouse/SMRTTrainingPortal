using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailController : MonoBehaviour
{
    public InputField subject;
    public InputField body;
    public string emailBody;
    // Start is called before the first frame update
    void Start()
    {
        subject.text = "hello";
        int dropdownValue = PostController.postControllerInstance.trainingTypeDD.value;
        string trainingType = PostController.postControllerInstance.trainingTypeDD.options[dropdownValue].text;
        emailBody = string.Format("Hi Interchange Personal,\n\n Here are the Bus Captains that need to attend the {0} Training.\n\n {1}", trainingType, PostController.postControllerInstance.nameFields[0].text);
        GenerateEmail();
    }

    void GenerateEmail()
    {
        body.text = emailBody;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

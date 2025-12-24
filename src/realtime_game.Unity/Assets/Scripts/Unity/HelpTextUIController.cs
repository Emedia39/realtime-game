using UnityEngine;
using UnityEngine.UI;

public class HelpTextUIController : MonoBehaviour
{
    [SerializeField] Text helpText;

    bool helpTextDisplay = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //gameObject.SetActive(true);
        helpTextDisplay = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if(helpTextDisplay == true)
            {
                helpText.enabled = false;
                helpTextDisplay = false;
            }
            else if(helpTextDisplay == false)
            {
                helpText.enabled = true;
                helpTextDisplay = true;
            }
            //Debug.Log("H!");
        }

    }
}

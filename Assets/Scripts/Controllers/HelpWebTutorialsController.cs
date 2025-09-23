using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpWebTutorials : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OpenTutorialInterface()
    {
        var webView1 = gameObject.GetComponent<UniWebView>();
        if (webView1 == null)
        {
            webView1 = gameObject.AddComponent<UniWebView>();
        }

        //Mostrar el aviso de privacidad en el UniWebView
        //string urlTutorials = "https://arturosuarezdev.github.io/Proyectoreal.html";
        string urlTutorials = config.sharedInstance.urlArviSpaceForms("help",config.localFiles.yes);
        WebViewController.sharedInstance.ShowWebView(webView1, urlTutorials, 1);
    }
}

//using Gpm.WebView.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TerminosController : MonoBehaviour
{
    #region Variables privadas serializadas
    [SerializeField]
    private GameObject blackScreen;
    [SerializeField] private WebViewController m_webViewController;   
    #endregion

    #region Variables privadas
    private Animator intro;
    private  bool isAcceptedAgreements;
    private float time = 0;
    #endregion

    void Start()
    {
        //Obtener el intro
        foreach (var item in config.sharedInstance.GetIntros())
        {
            if (item.gameObject.activeSelf)
            {
                intro = item.GetComponent<Animator>();
            }
        }

    }

    void Update()
    {
        //Si ya terminó la intro...
        if (intro.runtimeAnimatorController.animationClips[0].length <= time)
        {
            
            //Checar si los términos están en pantalla
            if (!m_webViewController.GetIsShown())
            {
                //Si no es asi, mostrarlos
                if (!IsAgreementAccepted())
                {
                    blackScreen.SetActive(true);
                    OpenTerminates();
                }
                else
                {
                    blackScreen.SetActive(false);
                }
            }
        }
        else
        {
            //Tomar el tiempo transcurrido de la aplicación para checar si la intro ya terminó
            time += Time.deltaTime;
        }
        
    }
    #region Public Methods 
    /// <summary>
    /// Inicia el proceso de visualización de la vista web referente al aviso de privacidad
    /// </summary>
    public void OpenTerminates() {
        //Obtener o crear el componente UniWebView del gameObject
        var webView1 = gameObject.GetComponent<UniWebView>();
        if (webView1 == null)
        {
            webView1 = gameObject.AddComponent<UniWebView>();
        }

        //Mostrar el aviso de privacidad en el UniWebView
        string urlPrivacyPolicy = config.sharedInstance.urlArviSpaceForms("agreements", config.localFiles.yes);
        WebViewController.sharedInstance.ShowWebView(webView1, urlPrivacyPolicy, 1);
    }

    /// <summary>
    /// Verifica si el usuario aceptó el aviso de privacidad
    /// </summary>
    /// <returns></returns>
    public static bool IsAgreementAccepted()
    {
// Comprueba si estás en el editor
#if UNITY_EDITOR
        return true;
#endif
        if (PlayerPrefs.GetString("agreementsAccepted") == null ||
            PlayerPrefs.GetString("agreementsAccepted") != "true")
        {
            //Debug.Log("Politica de privacidad rechazada");
            return false;
        }
        Debug.Log("Politica de privacidad aceptada");
        return true;

    }
}
#endregion

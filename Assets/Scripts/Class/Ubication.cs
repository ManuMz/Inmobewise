using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

[Serializable]
public class TUbication
{
    public string cp;
}


[System.Serializable]
public class Ubication : MonoBehaviour
{
    public static Ubication sharedInstance;

    #region Variables privadas 
    [Header("Visualizacion")]
    public string pPostalCode;
    public string pPostalCodeDemo;
    public bool pValidateCP;

    private Animator intro;
    #endregion
    private void Awake()
    {
        if (!sharedInstance)
            sharedInstance = this;
        else
            Destroy(this);
    }

    void Start()
    {
#if UNITY_EDITOR
        /// establece por defecto un codigo postal si esta en el editor
        UpdateLocation(
            /*"72014"*/"72100"
            //"90500"
            );
#endif
        pPostalCodeDemo = "72100";
        //pPostalCodeDemo = "90500";

        foreach (var item in config.sharedInstance.GetIntros())
        {
            if (item.gameObject.activeSelf)
            {
                intro = item.GetComponent<Animator>();
            }
        }
        pPostalCode = PlayerPrefs.GetString("_postal_code", "") == "" ? "" : PlayerPrefs.GetString("_postal_code");//recupera el codigo postal guardado en PlayerPrefs
        pValidateCP = PlayerPrefs.GetString("_postal_code", "") != "";

    }

    /// <summary>
    /// metodo para obtener el codigo postal
    /// </summary>
    /// <returns></returns>
    public string GetPostalCode()
    {
        if (config.sharedInstance.GetmUseMode() == config.useMode.testing)
        {
            return pPostalCode;
        }
        else
        {
            return pPostalCode;
        }
    }
    private float time = 0;

    // Update is called once per frame
    void Update()
    {
        if (time == -1)
        {
            return;
        }
        //Si el intro se encuentra a la mitad se mete a este if para pausar y verificar que todo vaya bien
        if (intro.runtimeAnimatorController.animationClips[0].length / 2 < time)
        {
            if (PersonalizableController.sharedInstance.GetIsDone())
            {
                intro.speed = 1;
                if (time != -1)
                {
                    //Debug.Log("entro en el tiempo");
                    time += Time.deltaTime;
                }
                if (TerminosController.IsAgreementAccepted())
                //if (Geolocation.sharedInstance.isAsked)
                {
                    if (intro.runtimeAnimatorController.animationClips[0].length - .5f < time)
                    {
                        if (pValidateCP)
                        {
#if !UNITY_EDITOR
                                UpdateLocation(pPostalCode.ToString());
#endif
                            // opentutorial();
                        }
                        else
                        {
                            //Debug.Log("no entro en el tiempo");
                            OpenMap();
                            // opentutorial();
                        }

                        time = -1;
                    }

                }
            }
            else
            {
                //Debug.Log("no entro en el segundo intro");
                intro.speed = 0;
            }


        }
        else
        {
            //Debug.Log("no entro en el intro");
            if (time != -1)
            {
                time += Time.deltaTime;
            }
        }

    }
    /// <summary>
    /// metodo para eliminar el cp guardado
    /// </summary>
    public void DeleteCP()
    {
        PlayerPrefs.SetString("_postal_code", "");
    }
    /// <summary>
    /// metodo para guardar el ultimo cp
    /// </summary>
    public void SaveLastCP()
    {
        Debug.Log("EL CP QUE VOY A GUARDAR ES: " + pPostalCode);
        PlayerPrefs.SetString("_postal_code", pPostalCode);
    }
    /// <summary>
    /// metodo para verificar si la ubicacion se ha actualizado
    /// </summary>
    /// <returns></returns>
    public static bool IsLocationUpdated()
    {
        string savedPostalCode = PlayerPrefs.GetString("_postal_code");
        return !string.IsNullOrEmpty(savedPostalCode);
    }
    /// <summary>
    /// metodo para actualizar la ubicacion con un codigo postal
    /// </summary>
    /// <param name="postalCode"></param>
    public void UpdateLocation(string postalCode)
    {
        pPostalCode = postalCode;
        if (FindObjectOfType<ConfigurationController>().configuration.cp == 1)
        {
            PlayerPrefs.SetString("_postal_code", pPostalCode);
        }
        FindObjectOfType<DownloadComponentController>().StartDownload();
        Debug.Log("ENTRE A LA UBICASION...");
    }

    /// <summary>
    /// metodo para abrir el mapa de navegacion
    /// </summary>
    public void OpenMap()
    {
        if (true)//if (Geolocation.sharedInstance.isLocate)
        {
            var webView1 = gameObject.AddComponent<UniWebView>();
            //  WebViewController.sharedInstance.initnewview(webView1);
            WebViewController.sharedInstance.ShowWebView(webView1, config.sharedInstance.urlArviSpaceForms("map", config.localFiles.yes), 1);
        }
    }
    /// <summary>
    /// metodo para abrir un tutorial usando UniWebView
    /// </summary>
    public void OpenTutorial()
    {
        var webView1 = gameObject.AddComponent<UniWebView>();//Abre el tutorial
        WebViewController.sharedInstance.ShowWebView(webView1, config.sharedInstance.urlArviSpaceForms("tutorial", config.localFiles.yes), 2);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// clase que representa la configuracion del usuario
[Serializable]
public class Configuration {
    public int m_menu;
    public int sesion;
    public int cp;
    public int tutorial;
}
public class ConfigurationController : MonoBehaviour
{
    public static ConfigurationController sharedInstance;
    // 1 = true
    //0 = false
    [Header("Visualizacion de la clase")]
    public Configuration configuration;// referencia a la clase configuración

    #region Obtener valores de variables privadas
    /*
    public Configuration GetConfiguration()
    {
        return configuration;
    }
    */
    #endregion
    void Awake()
    {
        if (sharedInstance)
            Destroy(this);
        else
            sharedInstance = this;

        configuration = new Configuration();
        //If not exists variable in application
        if (PlayerPrefs.GetInt("m_menu", -1) == -1)
        {
            PlayerPrefs.SetInt("m_menu", 1);
            configuration.m_menu = PlayerPrefs.GetInt("m_menu");
        }
        //Just in case the variable is saved sign to m_menu 

        //Por si la variable is guardada inicia sesión en m_menu
        else
        {
            configuration.m_menu = PlayerPrefs.GetInt("m_menu");
        }
        //initialize variables
        //Inicializar variables
        InitConfigurationWebView();


        //If not exists variable in application
        //Si no existe la variable en la aplicación
        if (PlayerPrefs.GetInt("_sesion", -1) == -1)
        {
            PlayerPrefs.SetInt("_sesion", 1);
            configuration.sesion = PlayerPrefs.GetInt("_sesion");
        }
        //Just in case the variable is saved sign to m_menu 

        //Por si la variable is guardada inicia sesión en m_menu
        else
        {
            configuration.sesion = PlayerPrefs.GetInt("_sesion");
        }


        //If not exists variable in application
        //Si no existe la variable en la aplicación
        if (PlayerPrefs.GetInt("_cp", -1) == -1)
        {
            PlayerPrefs.SetInt("_cp", 1);
            configuration.cp = PlayerPrefs.GetInt("_cp");
        }
        //Just in case the variable is saved sign to cp 

        //Por si la variable is guardada inicia sesión en codigo postal
        else
        {
            configuration.cp = PlayerPrefs.GetInt("_cp");
        }


        //If not exists variable in application
        //Si no existe la variable en la aplicación
        if (PlayerPrefs.GetInt("_tutorial", -1) == -1)
        {
            PlayerPrefs.SetInt("_tutorial", 1);
            configuration.tutorial = PlayerPrefs.GetInt("_tutorial");
        }
        //Just in case the variable is saved sign to cp 

        //Por si la variable is guardada inicia sesión en codigo postal
        else
        {
            configuration.tutorial = PlayerPrefs.GetInt("_tutorial");
        }
    }

    private void Start()
    {
    }
    void Update()
    {

    }

    #region PrivateMethods
    /// <summary>
    /// inicializa la configuracion en el webview
    /// </summary>
    private void InitConfigurationWebView()
    {
        if (configuration.m_menu == 1)
        {
            //FindObjectOfType<config>().m_menu = config.menu.ar;------------------------------Determinar si se puede eliminar------------------------------------------------[1]
        }
        else
        {
            //FindObjectOfType<config>().m_menu = config.menu.web;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// cambia la configuracion del menu del webview
    /// </summary>
    /// <param name="value"></param>
    public void ChangeConfigurationWebView(int value)
    {
        configuration.m_menu = value;
        if (configuration.m_menu == 0)
        {
            PlayerPrefs.SetInt("m_menu", configuration.m_menu);
        }
        else
        {
            PlayerPrefs.SetInt("m_menu", configuration.m_menu);
          
        }
    }
    /// <summary>
    /// Cambia la configuración de la sesión
    /// </summary>
    /// <param name="value"></param>
    public void ChangeConfigurationSession(int value)
    {
        configuration.sesion = value;
        if (configuration.sesion == 0)
        {
            PlayerPrefs.SetInt("_sesion", configuration.sesion);
            //FindObjectOfType<Usuario>().DeleteLastSesion();
        }
        else
        {
            //FindObjectOfType<Usuario>().SaveLastSession();
            PlayerPrefs.SetInt("_sesion", configuration.sesion);
        }
    }

    /// <summary>
    /// Cambia la configuración del codigo postal
    /// </summary>
    /// <param name="value"></param>
    public void ChangeConfigurationPostalCode(int value)
    {
        configuration.cp = value;
        if (configuration.cp == 0)
        {
            PlayerPrefs.SetInt("_cp", configuration.cp);
            FindObjectOfType<Ubication>().DeleteCP();
        }
        else
        {
            FindObjectOfType<Ubication>().SaveLastCP();
            PlayerPrefs.SetInt("_cp", configuration.cp);
        }
    }
    /// <summary>
    /// Alterna la configuración del tutorial
    /// </summary>
    public void ToggleConfigurationTutorial()
    {
        if (configuration.tutorial == 1)
        {
            configuration.tutorial = 0;
            PlayerPrefs.SetInt("_tutorial", configuration.cp);
        }
        else
        {
            configuration.tutorial = 1;
            PlayerPrefs.SetInt("_tutorial", configuration.tutorial);
        }
    }
    /// <summary>
    /// No regresa si el tutorial esta habilitado
    /// </summary>
    /// <returns></returns>
    public bool GetConfigurationTutorial() {
        bool flag = false;
        if (configuration.tutorial == 1) {
            flag = true;
        }
        return flag;
    }
    #endregion
}

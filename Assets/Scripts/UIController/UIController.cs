using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
public enum PanelType
{
    login,
    register,
    resetPassword,
    productMenu,
    buildingMenu,
    favoriteBuildingsMenu,
    defaultPanel,
}

public enum InputFieldType
{
    //Login Panel
    emailLogin,
    passwordLogin,
    //Register Panel
    fullNameRegister,
    userNameRegister,
    emailRegister,
    phoneNumberRegister,
    passwordRegister,
}

public class UIController : MonoBehaviour
{
    public static UIController sharedInstance;

    [SerializeField]
    private List<GameObject> panels; //paneles de la app
    [SerializeField]
    private GameObject currentPanel; //panel actual
    private bool switchMenu;
    private PanelType currentPanelType;

    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
            SetStart();
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
  
    }
    private void SetSetters()
    {

    }

    private void SetStart()
    {
        ShowPanel(PanelType.login);
    }

    /// <summary>
    /// Activa o Desactiva el Menu de los productos
    /// </summary>
    public void SwitchMenu()//Se utiliza el el boton del ring menu(Menu).
    {

    }

    public bool ExistPanel(PanelType panelType, out GameObject panel)
    {
        panel = null; //valor de salida de la funcion

        foreach (GameObject p in panels)
        {
            var panelIndex = p.GetComponent<PanelController>();

            if (panelIndex != null && panelIndex.panelType == panelType)
            {
                panel = p;
                return true;
            }
        }

        return false;
    }

    public void ShowPanel(PanelType panelType)
    {
        HidePanel();

        if (ExistPanel(panelType, out GameObject panelToShow))
        {
            currentPanel = panelToShow;
            currentPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"no se encontró el panel: {panelType}");
        }
    }

    public void HidePanel()
    {
        foreach(GameObject p in panels)
        {
            p.SetActive(false);
        } 
    }

    public InputField GetInputField(InputFieldType type)
    {
        InputFieldController[] inputFields =
            currentPanel.GetComponentsInChildren<InputFieldController>();

        foreach (InputFieldController inputField in inputFields)
        {
            if (inputField.InputFieldType == type)
            {
                //return inputField.GetInputValue();//el texto/valor del inputField
                return inputField.GetInputField();//obtener el objeto
            }
        }
       
        return null;
    }

    public void SetInputField(InputFieldType type, string textValue)
    {
        InputFieldController[] inputFields =
            currentPanel.GetComponentsInChildren<InputFieldController>();

        foreach (InputFieldController inputField in inputFields)
        {
            if (inputField.InputFieldType == type)
            {
                inputField.SetInputValue(textValue);
                return;
            }
        }
    }

    public void SetInputFieldMessage(InputFieldType type, string message,
        string animState)
    {
        InputFieldController[] inputFields =
            currentPanel.GetComponentsInChildren<InputFieldController>();
        foreach (InputFieldController inputField in inputFields)
        {
            if(inputField.InputFieldType == type)
            {
                inputField.SetMessage(message);
                inputField.SetAnimationState(animState);
                return;
            }
        }
    }

    public void SendUITrigger(int index)
    {
   
        switch (currentPanelType)
        {
            case PanelType.login:
                break;
            case PanelType.register:
                break;
            case PanelType.resetPassword:
                break;
            default:
                Debug.LogError("Error al recibir el disparador de acuerdo" +
                    "al tipo de panel");
                break; 
        }
    }

    /// <summary>
    /// Oculta el Menu de productos
    /// </summary>
    public void HideMenu()
    {

    }
    /// <summary>
    /// Cerrar el menu principal de los immuebles
    /// </summary>
    public void HideMenuBuildings()//Buton X del menu de inmuebles
    {
       
    }

    public void ShowFavoriteBuildingsMenu()
    {
       
    }
    public void HideFavoriteBuildingsMenu()
    {
       
    }

    /// <summary>
    /// Cierra la app.
    /// </summary>
    public void ExitApp()
    {
        Application.Quit();
    }
}

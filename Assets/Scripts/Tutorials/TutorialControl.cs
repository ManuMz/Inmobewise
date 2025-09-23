using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
#region Enum
public enum tutorialItems
{
    touchFloor = 1, 
    moveObject = 2,
    rotateObject = 3, 
    touchTexture = 4,
    welcome = 5,
    notAvailablePackage= 6,
    availablePackage =7,
    
    popupNotification =8,
    infoNotification= 9,
    questionNotification=10,    
}


public enum PopupsNotificationsIcons
{
    successIcon=0,
    errorIcon=1,
    warningIcon=2,  
}

#endregion
public class TutorialControl : MonoBehaviour
{
    #region Miembro est?tico del patr?n de dise?o Singleton
    public static TutorialControl sharedInstance;
    #endregion

    #region Variables privadas serializadas
    [SerializeField]
    private GameObject panel; // panel en el cual se instanciar? cada tutorial 
    [SerializeField]
    private bool isPlaying; //booleano que indica si el tutorial se encuentra en ejecucion
    [SerializeField]
    private tutorialItems currentTutorial; // variable que hace referencia al enum tutorialItem, indica el tutorial a utilizar
    [SerializeField]
    private bool isActiveByUser = true; // booleano que indica la interacci?n del tutorial por parte del usuario 

    #endregion

    #region Variables privadas 
    private int limitToPlay = 3;// numero de intentos permitidos para el usuario 
    #endregion

    #region Obtener valores de variables privadas
    /// <summary>
    /// devuelve el GameObject panel
    /// </summary>
    /// <returns></returns>
    public GameObject Panel()
    {                                                             
        return panel;   
    }
    /// <summary>
    /// devuelve el valor del booleano isPlaying
    /// </summary>
    /// <returns></returns>
    public bool IsPlaying()
    {
        return isPlaying;
    }
    /// <summary>
    /// devuelve el valor de la var privada que hace referencia al enum tutorialItem en ejecuci?n
    /// </summary>
    /// <returns></returns>
    public tutorialItems CurrentTutorial()
    {
        return currentTutorial; 
    }
    #endregion

    private void Awake()
    {
        if (!sharedInstance)
            sharedInstance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        EmptyPanel();
        DefaultConfig();
        //Si no existe esto en "player prefs" lo creamos
        if (PlayerPrefs.GetInt("isActiveByUser", -1) == -1)
        {
            //inicializa en 1
            PlayerPrefs.SetInt("isActiveByUser", 1);//1 = true
        }
        //Inicializa la variable booleana
        isActiveByUser = PlayerPrefs.GetInt("isActiveByUser") == 1 ? true : false;
    }
  
    #region Private Methods
    /// <summary>
    /// Reset times to play Item tutorial
    /// Restablece el momento para ejecutar item tutorial
    /// </summary>
    private void ResetTimesToPlay() 
    {
        foreach (var item in Enum.GetNames(typeof(tutorialItems)))
        {
            PlayerPrefs.SetInt(item, limitToPlay);
        }
    }
    /// <summary>
    /// Valida e inicializa variables en memoria segun cada TutorialItem
    /// </summary>
    /// <param name="tutorialItem">Enum</param>
    /// <returns>bool</returns>
    private bool Validate(tutorialItems tutorialItem)
    {
        bool res = PlayerPrefs.GetInt(tutorialItem.ToString(), limitToPlay) > 0 ? true : false;

        if (res)
            PlayerPrefs.SetInt(
                tutorialItem.ToString(),
                PlayerPrefs.GetInt(tutorialItem.ToString(), -1) == -1 ?
                    limitToPlay :
                    PlayerPrefs.GetInt(tutorialItem.ToString()) - 1);
        return res;

    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Muestra / instancia el tutorial seleccionado
    /// </summary>
    /// <param name="tutorialSelected"></param>
    /// <param name="beforeStart"></param>
    /// <param name="afterStart"></param>
    public void InstatiateTutorial(tutorialItems tutorialSelected, UnityAction beforeStart = null, UnityAction afterStart = null)
    {
#if UNITY_EDITOR
        ResetTimesToPlay();
#endif
        if (!isActiveByUser) return;
        //Vaciamos el panel para evitar que se encimen tutoriales
        EmptyPanel();
        DefaultConfig();

        GameObject item;
        item = Resources.Load<GameObject>("Tutorial/" + tutorialSelected.ToString() + "/" + tutorialSelected.ToString());

        //we validate it havent play a lot of time than limitToPlay Variable
        if (Validate(tutorialSelected) || item.GetComponent<TutorialItem>().isEverPlayable)
        {
            item.GetComponent<TutorialItem>().afterStart = afterStart;
            item.GetComponent<TutorialItem>().beforeStart = beforeStart;
            Instantiate(item, panel.transform);
            isPlaying = true;
            currentTutorial = tutorialSelected;
        }
    }
    /// <summary>
    /// Muestra el tutorial seleccionado al inicio de la aplicacion
    /// </summary>
    /// <param name="tutorialSelected"></param>
    public void InstatiateTutorialWelcome(tutorialItems tutorialSelected)
    {
        EmptyPanel();
        DefaultConfig();
        GameObject item;

        item = Resources.Load<GameObject>("Tutorial/" + tutorialSelected.ToString() + "/" + tutorialSelected.ToString());

        Instantiate(item, panel.transform);
        isPlaying = true;
        currentTutorial = tutorialSelected;
    }


    public void InstatiateNotAvailablePackage(tutorialItems tutorialSelected, string productName)
    {
        EmptyPanel();
        DefaultConfig();
        GameObject item;

        item = Resources.Load<GameObject>("Tutorial/" + tutorialSelected.ToString() + "/" + tutorialSelected.ToString());

        Instantiate(item, panel.transform);
        isPlaying = true;
        currentTutorial = tutorialSelected;

        panel.transform.Find("notAvailablePackage(Clone)/Contorno/Panel/ProductName").GetComponent<Text>().text = productName;

        panel.transform.Find("notAvailablePackage(Clone)/Contorno/Panel/Close").GetComponent<Button>().onClick.RemoveAllListeners();
        panel.transform.Find("notAvailablePackage(Clone)/Contorno/Panel/Close").GetComponent<Button>().onClick.AddListener(() =>
        {
            if(IsPlaying() && CurrentTutorial() == tutorialItems.notAvailablePackage)
            {
                //item.GetComponent<TutorialItem>().toDestroy = true;
                panel.transform.Find("notAvailablePackage(Clone)").GetComponent<TutorialItem>().toDestroy = true;
            }

            //CartController.sharedInstance.RecalculatePhysicDimensionsTotales(idProBodPre);
            //CartController.sharedInstance.DeleteLastProductInCart(idProBodPre);

        });
    }

    public void InstatiateAvailablePackage(tutorialItems tutorialSelected)
    {
        EmptyPanel();
        DefaultConfig();
        GameObject item;

        item = Resources.Load<GameObject>("Tutorial/" + tutorialSelected.ToString() + "/" + tutorialSelected.ToString());

        Instantiate(item, panel.transform);
        isPlaying = true;
        currentTutorial = tutorialSelected;

        panel.transform.Find("availablePackage(Clone)/Contorno/Panel/Close").GetComponent<Button>().onClick.RemoveAllListeners();
        panel.transform.Find("availablePackage(Clone)/Contorno/Panel/Close").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (IsPlaying() && CurrentTutorial() == tutorialItems.availablePackage)
            {
                //FindObjectOfType<TutorialItem>().toDestroy = true;
                panel.transform.Find("availablePackage(Clone)").GetComponent<TutorialItem>().toDestroy = true;
                //item.GetComponent<TutorialItem>().toDestroy = true;
            }
        });
            
    }


    public void CreatePopupNotification(tutorialItems tutorialSelected,
        PopupsNotificationsIcons popupNotificationIconSelected, string title, string message, float delay)
    {
        EmptyPanel();
        DefaultConfig();
        GameObject prefab = Resources.Load<GameObject>("Tutorial/" + tutorialSelected.ToString() + "/" + tutorialSelected.ToString());

        Sprite icon = Resources.Load<Sprite>("PopupsNotificationsIcons" + popupNotificationIconSelected.ToString());

        GameObject instance = Instantiate(prefab, panel.transform);
        isPlaying = true;
        currentTutorial = tutorialSelected;

        instance.transform.Find("Contorno/Panel/Icon").GetComponent<Image>().sprite =icon;
        instance.transform.Find("Contorno/Panel/Title").GetComponent<Text>().text = title;
        instance.transform.Find("Contorno/Panel/Message").GetComponent<Text>().text = message;

        StartCoroutine(WaitSeconds(delay, ()=>Destroy(instance)));
        //panel.transform.Find("popupNotification(Clone)/Contorno/Panel/Close").GetComponent<Button>().onClick.RemoveAllListeners();
        //panel.transform.Find("popupNotification(Clone)/Contorno/Panel/Close").GetComponent<Button>().onClick.AddListener(()=>
        //{
        //    if (IsPlaying() && CurrentTutorial() == tutorialItems.popupNotification)
        //    {
        //        panel.transform.Find("popupNotification(Clone)").GetComponent<TutorialItem>().toDestroy = true;
        //    }
        //});

    }

    public void CreateInfoNotification(tutorialItems tutorialSelected, string title, string message,string buttonText, Action buttonAction)
    {
        GameObject prefab = Resources.Load<GameObject>("Tutorial/" + tutorialSelected.ToString() + "/" + tutorialSelected.ToString());

        GameObject instance = Instantiate(prefab, panel.transform);
       
        instance.transform.Find("Panel/Title").GetComponent<Text>().text = title;
        instance.transform.Find("Panel/Message").GetComponent<Text>().text = message;
        instance.transform.Find("Panel/ActionButton").GetChild(0).GetChild(0).GetComponent<Text>().text = buttonText;

        instance.transform.Find("Panel/ActionButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(instance);
            buttonAction?.Invoke();
        });
    }

    public void CreateQuestionNotification(tutorialItems tutorialSelected)
    {

    }

    private void DestroyNotification()
    {
        if (IsPlaying() && CurrentTutorial() == tutorialItems.popupNotification)
        {
            panel.transform.Find("popupNotification(Clone)").GetComponent<TutorialItem>().toDestroy = true;
        }
    }

    private IEnumerator WaitSeconds(float delay, UnityAction action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    /// <summary>
    /// Vacia el panel en el cual se instancia un tutorial para colocar otro
    /// </summary>
    public void EmptyPanel()
    {
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            Destroy(panel.transform.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// Este metodo desactiva el Raycast y puede funcionar de la misma manera
    /// </summary>
    public void DefaultConfig()
    {
        isPlaying = false;
        panel.GetComponent<Image>().raycastTarget = false;
    }
    #endregion
}

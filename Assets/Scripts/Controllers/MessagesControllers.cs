using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEditor;
using TMPro;

public class MessagesControllers : MonoBehaviour
{
    #region Enum
    public enum iconsNotifications
    {
        success = 0,
        error = 1,
        warning = 2
    }
    #endregion

    public static MessagesControllers sharedInstance;

    #region Variables privadas serializadas
    [SerializeField]
    private GameObject answer;
    
    [SerializeField]
    private GameObject notification;
    
    [SerializeField]
    private List<Sprite> spritesNotifications;
    
    [SerializeField]
    private GameObject info;
    
    [SerializeField]
    private Ease animationType;
    
    [SerializeField]
    private Image background;
    #endregion

    private void Awake()
    {
        if (!sharedInstance)
            sharedInstance = this;
        else
            Destroy(this);
    }

    #region Private Methods
    /// <summary>
    ///desaparición de animación de notificaciones 
    /// </summary>
    private void CloseNotification()
    {
        notification.GetComponent<RectTransform>().DOAnchorPos(new Vector2(750, -100), .8f);//Oculta la notificación
    }

    /// <summary>
    /// corrutina de tiempo de espera
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator WaitSeconds(float delay, UnityAction action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    /// <summary>
    /// cambia la transparencia del panel
    /// </summary>
    /// <param name="transparency"></param>
    private void ChangeBackground(float transparency)
    {
        background.DOFade(transparency, .4f);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// animación de aparición de notificaciones 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="body"></param>
    /// <param name="delay"></param>
    /// <param name="iconSelected"></param>
    public void CreateNotification(string title, string body, float delay, iconsNotifications iconSelected)
    {
        CloseNotification();
        notification.transform.Find("contentIcon/icon").GetComponent<Image>().sprite = spritesNotifications[(int)iconSelected];
        notification.transform.Find("Body").GetComponent<Text>().text = body;
        notification.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-485, -100), .8f);//Muestra la notificación
        StartCoroutine(WaitSeconds(delay, CloseNotification));
    }

    /// <summary>
    /// Animación de aparición de información
    /// </summary>
    /// <param name="text"></param>
    /// <param name="time"></param>
    /// <param name="acceptText"></param>
    public void CreateInfo(string text, float time = 1f, string acceptText = "Aceptar")
    {
        CloseInfo(0.1f);//oculta rapido el panel antes de entrar
        ChangeBackground(.9f);
        info.transform.Find("ScrollView/Viewport/Body").GetComponent<TextMeshProUGUI>().text = text;
        info.transform.Find("Buttons/Ok").GetComponent<Button>().onClick.RemoveAllListeners();
        info.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0 , 152.6692f), time).SetEase(animationType);//0,0
    }

    /// <summary>
    /// Creación de la respuesta 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="body"></param>
    /// <param name="cancelCall"></param>
    /// <param name="successCall"></param>
    /// <param name="time"></param>
    /// <param name="cancelText"></param>
    /// <param name="acceptText"></param>
    public void CreateAnswer(string title, string body, UnityAction cancelCall = null, UnityAction successCall = null, float time = .5f, string cancelText = "Cancelar", string acceptText = "Aceptar")
    {
        ChangeBackground(0.9f);
        answer.transform.Find("Header/Text").GetComponent<Text>().text = title;
        answer.transform.Find("Body").GetComponent<Text>().text = body;

        answer.transform.Find("Buttons/Ok").GetComponent<Button>().onClick.RemoveAllListeners();
        answer.transform.Find("Buttons/Cancel").GetComponent<Button>().onClick.RemoveAllListeners();

        answer.transform.Find("Buttons/Ok").GetComponent<Button>().onClick.AddListener(successCall);
        answer.transform.Find("Buttons/Cancel").GetComponent<Button>().onClick.AddListener(cancelCall);

        answer.transform.Find("Buttons/Ok/Text").GetComponent<Text>().text = acceptText;
        answer.transform.Find("Buttons/Cancel/Text").GetComponent<Text>().text = cancelText;

        answer.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), time).SetEase(animationType);
    }

    /// <summary>
    /// animación de cierre de info
    /// </summary>
    /// <param name="time"></param>
    public void CloseInfo(float time = .5f)
    {
        ChangeBackground(0);
        info.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -385), time).SetEase(animationType); //0, -3000
    }
    /// <summary>
    /// Cierre de la respuesta
    /// </summary>
    /// <param name="time"></param>
    public void CloseAnswer(float time = .5f)
    {
        ChangeBackground(0);
        answer.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -700), time).SetEase(animationType);
    }
    #endregion
    
}
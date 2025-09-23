using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using System;
using System.Runtime.CompilerServices;

public class UILoginButtons : MonoBehaviour, ISelectHandler, IDeselectHandler
{


    [SerializeField]
    private string selectHexImg = "#FF0000";
    [SerializeField]
    private string unselectHexImg;
    [SerializeField]
    private GameObject firstButton;
    [SerializeField]
    private Image selectImg;

    public void OnSelect(BaseEventData eventData)
    {
        //if (!enabled)
        //    return;

        //OnSelectImg(selectImg);
        Debug.Log("Seleccionado: " + gameObject.name);

        OnSelectImg(selectImg);
        //throw new System.NotImplementedException();

    }

    public void OnDeselect(BaseEventData eventData)
    {
        //throw new System.NotImplementedException();
        OnDeselectImg(selectImg);

    }

    private void OnEnable()
    {
        if (firstButton != null)
        {
            // Clean Actual Selection
            EventSystem.current.SetSelectedGameObject(null);
            // Assign New Button
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    private void OnSelectImg(Image img)
    {
        //Debug.Log("ingrese a OnSelectImg");
        if (ColorUtility.TryParseHtmlString(selectHexImg, out Color imgColorHex))
        {
            Debug.Log("Color válido: " + selectHexImg);

            if (img != null && img.enabled)
            {
                Debug.Log("cambio de color Seleccionado");
                
                img.color = imgColorHex;
            }
        }
        else
        {
            Debug.LogError("Formato de color inválido: " + selectHexImg);
        }
    }
    private void OnDeselectImg(Image img)
    {
        //Debug.Log("ingrese a OnDeselectImg");

        if (ColorUtility.TryParseHtmlString(selectHexImg, out Color imgColorHex))
        {
            Debug.Log("Color válido: " + selectHexImg);
            if (img != null && img.enabled)
            {
                Debug.Log("cambio de color NO seleccionado");
                img.color = imgColorHex;
            }
        }
    }
    public void OnButtonPressed()
    {
        print("Button " + gameObject.name + " Pressed");
    }
}

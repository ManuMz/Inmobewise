using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WithoutProductsController : MonoBehaviour
{
    #region mimebro estático del patrón de diseño Singleton 
    public static WithoutProductsController sharedInstance;
    #endregion

    #region Variables privadas seriallizadas
    [SerializeField]
    private List<GameObject> components;
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Activa el canvas que indica la disponibilidad de los productos de acuerdo al código postal
    /// </summary>
    public void OpenWithoutProductsCanvas()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<Canvas>().sortingOrder = 13;
        components[3].SetActive(true);
    }
   
    /// <summary>
    /// Desactiva el canvas que indica la disponibilidad de los productos de acuerdo al código postal
    /// </summary>
    public void CloseWithoutProductsCanvas()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
        components[3].SetActive(false);
    }
    #endregion
}
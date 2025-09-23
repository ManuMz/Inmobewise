using System.Collections.Generic;
using UnityEngine;

public class WithoutBuildingController : MonoBehaviour
{
    #region miembro estático del patrón de diseño Singleton 
    public static WithoutBuildingController sharedInstance;
    #endregion

    #region Variables privadas seriallizadas
    [SerializeField]
    private List<GameObject> components;
    #endregion

    #region Public Methods
    /// <summary>
    /// Activa el canvas que indica la disponibilidad de los productos de acuerdo al código postal
    /// </summary>
    public void OpenWithoutBuildingCanvas()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<Canvas>().sortingOrder = 13;
        components[0].SetActive(true);
    }

    /// <summary>
    /// Desactiva el canvas que indica la disponibilidad de los productos de acuerdo al código postal
    /// </summary>
    public void CloseWithoutBuildingCanvas()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
        components[0].SetActive(false);
    }
    #endregion
}


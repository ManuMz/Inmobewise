using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingWifiController : MonoBehaviour
{
    #region mimebro estático del patrón de diseño Singleton 
    public static LoadingWifiController sharedInstance;
    #endregion

    #region Variables privadas seriallizadas
    [SerializeField]
    private List<GameObject> components;
    #endregion

    private void Awake()
    {
        /*if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(this);
        }*/
    }
    private void Start()
    {

    }

    #region Public Methods
    /// <summary>
    /// Habilita/abre la animacion de indicador de carga de n componente de la app
    /// </summary>
    public void OpenWiFi()
    {
        gameObject.GetComponent<Canvas>().sortingOrder = 13;// gameObject.GetComponent<Canvas>().sortingOrder = 4;
        //GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().Play("initiLoadWeb");
    }


    private void Update()
    {

    }
    /// <summary>
    /// Cierre la animación de indicador de carga de n componente de la app 
    /// </summary>
    public void CloseWiFi()
    {
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
        GetComponent<Animator>().Play("idle");
        /*if (gameObject.activeSelf)

        {
            //Debug.Log("Canvas activo");
            GetComponent<Animator>().enabled = true;
            GetComponent<Animator>().Play("idle");
        }
        else
        {
            //Debug.Log("Canvas no Activo");
            GetComponent<Animator>().enabled = false;
        }*/
    }
    #endregion
}

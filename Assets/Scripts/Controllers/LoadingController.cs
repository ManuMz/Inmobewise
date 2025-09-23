using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    #region mimebro estático del patrón de diseño Singleton 
    public static LoadingController sharedInstance;
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
    public void Open()
    {
        //gameObject.SetActive(true);
        gameObject.GetComponent<Canvas>().sortingOrder = 13;// gameObject.GetComponent<Canvas>().sortingOrder = 4;
        //GetComponent<Animator>().enabled = true;
        components[0].SetActive(true);
        components[1].SetActive(true);
        GetComponent<Animator>().Play("initiLoadWeb");
    }

    /// <summary>
    /// Activa el canvas que acompaña al indicador de descarga de AssetBundle
    /// </summary>
    public void OpenAssetBundle()
    {
        gameObject.GetComponent<Canvas>().sortingOrder = 13;// gameObject.GetComponent<Canvas>().sortingOrder = 4;
        //components[1].SetActive(false);
        //GetComponent<Animator>().Play("initiLoadWeb");
        components[0].SetActive(true);
        GetComponent<Animator>().Play("loadWeb");
        components[1].SetActive(false);

    }

    private void Update()
    { 

    }
    /// <summary>
    /// Cierre la animación de indicador de carga de n componente de la app 
    /// </summary>
    public void Close()
    {
        //gameObject.SetActive(false);
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
        components[0].SetActive(false);
        components[1].SetActive(false);
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
    /// <summary>
    /// Desactiva el canvas que acompaña al indicador de descarga de AssetBundle
    /// </summary>
    public void CloseAssetBundle()
    {
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
        components[0].SetActive(false);
        GetComponent<Animator>().Play("idle");
        components[1].SetActive(false);

    }
    #endregion
}
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
//using UnityEngine.UIElements;

public class ContainerSubCategoriaInmueble : ScrollItem
{
    #region PUBLIC VARIABLES
    [HideInInspector]
    public Tipo_Inmueble tipoinmueble;//(Subcategoria)
    #endregion

    #region PRIVATE VARIABLES
    [SerializeField] private Text text;
    private MenuBuildings m_menuInmuebleManager;
    #endregion

    public override void onStart()
    {
        //Cambiamos el nombre del objeto por el id de nuestra subcategoria
        gameObject.name = tipoinmueble.Id_Tipo_Inmueble.ToString();
        // buscamos e inicalizamos nuestro MenuInmueble
        m_menuInmuebleManager = FindObjectOfType<MenuBuildings>();

        gameObject.GetComponent<Button>().onClick.AddListener(() => { m_menuInmuebleManager.InstatiateProductsBuildinigs(tipoinmueble); FindObjectOfType<MessagesControllers>().CloseInfo();});

        text.text = tipoinmueble.tipo_Inmueble; //asigna el nombre de cada producto
    }
}
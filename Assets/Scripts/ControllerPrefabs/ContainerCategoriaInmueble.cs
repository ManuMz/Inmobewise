using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using UnityEngine.Networking;
using DG.Tweening;
using UnityEngine.Events;
using JetBrains.Annotations;

public class ContainerCategoriaInmueble : ScrollItem
{
    #region PUBLIC VARIABLES
    [HideInInspector]
    public CategoriaInmueble categoryBuilding;
    #endregion

    #region PRIVATE VARIABLES
    [SerializeField] private Text text;
    private MenuBuildings m_menuManagerInmueble;
    #endregion

    #region SCROLL ITEMS METHODS
    public override void onStart()
    {
        //Objeto que al descargar el nombre se cambia por su id
        gameObject.name = categoryBuilding.Id_Tipo_Publicacion.ToString();

        //Buscamos e inicalizamos nuestro MenuInmueble
        m_menuManagerInmueble = FindObjectOfType<MenuBuildings>();
        gameObject.GetComponent<Button>().onClick.AddListener(() => { m_menuManagerInmueble.InstatiateSubCategoriesBuildings(categoryBuilding); FindObjectOfType<MessagesControllers>().CloseInfo(); });

        //obtenemos el componente texto para poder mostrar el id de categoria
        text = transform.Find("Text").GetComponent<Text>();

        //Le pasamos el nombre de la categoria que se esta mostrando
        text.text = categoryBuilding.Tipo_Publicacion;

    }
    #endregion

}

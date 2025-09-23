using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
//using static UnityEditor.Progress;
using System.Linq;
using Unity.VisualScripting;

public class ContainerInmuebleVisualizer : ScrollItem
{
    #region PUBLIC VARIABLES
    [HideInInspector]
    public Inmuebles building;
    //private DownloadModel downloadFunction;
    [HideInInspector]
    public QueriesController queriesController;
    [HideInInspector]
    public DownloadComponentController m_downLoadComponentController;
    #endregion

    #region PRIVATE VARIABLES
    [SerializeField] private GameObject scrollCaracteristica, caracteristicaContainer, offer, info, description, buttons, card, descriptionContainer, descriptionText, addCart, buyProducto;
    [SerializeField] private Text mCategory, mSubCategory;
    [SerializeField] private Button viewButton, moreInfo;
    [SerializeField] private RawImage imagen;
    [SerializeField] private Text title;
    private bool hasImage;

    private SaveComponentsController m_saveComponentsController;
    #endregion

    #region Static Variables
    public static bool changeObj;
    #endregion

    #region SCROLL ITEM METHODS
    public override void onVisible()
    {
        if (!hasImage)
        {
            hasImage = true;
            DownloadImageBuilders();//Descarga el arreglo de imagenes en base 64
        }
    }

    public override void onStart()
    {
        hasImage = false;
        gameObject.name = building.Id_Inmueble.ToString();//Nombre del contenedor en unity.
        m_saveComponentsController = FindObjectOfType<SaveComponentsController>();
        queriesController = FindObjectOfType<QueriesController>();
        m_downLoadComponentController = FindObjectOfType<DownloadComponentController>();  
        title.text = building.Nombre_Inmueble;//Titulo del contenedor
        
        imagen.DOFade(0, 0);

        viewButton.onClick.RemoveAllListeners();
        viewButton.onClick.AddListener(() =>
        {
            m_saveComponentsController.StartToLoadDefaultData(int.Parse(building.Id_Inmueble),building.Codigo_Inmueble);
            m_saveComponentsController.GetBuildingCodeUseToSave(building.Codigo_Inmueble);
        });

        moreInfo.onClick.RemoveAllListeners();
        moreInfo.onClick.AddListener(() =>
        {
            FindObjectOfType<MessagesControllers>().CreateInfo(building.Descripcion_Inmueble);//activa la animaci?n de informaci?n de producto con los elementos
        });

        description.GetComponent<Text>().text = building.Descripcion_Inmueble;

        GetCategoryAndSubcategoryByBuilding(building.Codigo_Inmueble);
    }

    public override void onCleanCache()
    {
        if (hasImage)
        {
            imagen.texture = null;
            imagen.DOFade(0, 0);

            for (int i = 1; i < caracteristicaContainer.transform.childCount; i++)
            {
                Destroy(caracteristicaContainer.transform.GetChild(i).gameObject);
            }
            hasImage = false;
        }
    }
    /// <summary>
    /// Obtener categoria y subcategoria a partir del codigo del inmueble
    /// </summary>
    /// <param name="buidingCode"></param>
    public void GetCategoryAndSubcategoryByBuilding(string buidingCode)
    {
        if (m_downLoadComponentController.componentsBuildings.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.componentsBuildings)
            {
                foreach (var subCategory in category.Tipo_Inmueble)
                {
                    foreach (var item in subCategory.Inmuebles)
                    {
                        if (item.Codigo_Inmueble.Equals(buidingCode))
                        {
                            mSubCategory.text = subCategory.tipo_Inmueble;
                            mCategory.text = category.Tipo_Publicacion;
                        }
                    }
                }
            }
        }
    }

    public override void onDestroy()
    {
        imagen.texture = null;
    }
    #endregion

    #region DESCARGA DE IMAGEN
    /// <summary>
    /// Descarga un arreglo de imagenes a base 64
    /// </summary>
    /// <returns></returns>
    private void DownloadImageBuilders()
    {
        // Recorriendo la lista con un bucle foreach
        foreach (Pictures picture in building.Pictures)
        {
            string base64String = picture.Picture.Substring(picture.Picture.IndexOf(',') + 1);
            // Decodificar la cadena Base64
            byte[] bytes = System.Convert.FromBase64String(base64String);
            GameObject temp = caracteristicaContainer.transform.GetChild(0).gameObject;
            temp = Instantiate(temp, caracteristicaContainer.transform);
            // Create a new texture
            Texture2D texture = new Texture2D(1, 1);
            // Load image bytes into the texture
            texture.LoadImage(bytes);
            // Asignar la textura al componente RawImage
            temp.GetComponent<RawImage>().texture = texture;
            temp.SetActive(true);
        }
    }
    #endregion
}

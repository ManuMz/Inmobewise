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

public class ContainerInmueble : ScrollItem
{
    #region PUBLIC VARIABLES
    [HideInInspector]
    public Inmuebles buildings;
    [HideInInspector]
    public DownloadModel downloadFunction;
    #endregion

    #region PRIVATE VARIABLES
    [SerializeField] private GameObject scrollCaracteristica, caracteristicaContainer, offer, info, description, buttons, card, descriptionContainer, descriptionText, addCart, buyProducto;
    [SerializeField] private Button viewButton;
    [SerializeField] private RawImage imagen;
    [SerializeField] private Text title;
    private bool hasImage;
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

        gameObject.name = buildings.Id_Inmueble.ToString();//Nombre del contenedor en unity.
        downloadFunction = FindObjectOfType<DownloadModel>();
        title.text = buildings.Nombre_Inmueble;//Titulo del contenedor
        
        imagen.DOFade(0, 0);

        viewButton.onClick.RemoveAllListeners();
        viewButton.onClick.AddListener(() =>
        {
            UIController.sharedInstance.HideMenuBuildings();

            RelocatePlayer.sharedInstance.PosInitial();//Reestablece la posicion del jugador en la pos inicial
            MenuBuildings.Clear(DownloadModel.sharedInstance.GetFatherBuildings());
            MenuBuildings.Clear(DownloadModel.sharedInstance.GetFatherProducts());

            downloadFunction.StartDownloadBuildings(buildings);
        });

        description.GetComponent<Text>().text = buildings.Descripcion_Inmueble;
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
        foreach (Pictures picture in buildings.Pictures)
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

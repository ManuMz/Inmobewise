using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using static SaveComponentsController;
using System;

public class ContainerFavoriteBuilding : ScrollItem
{
    public InmuebleData favoriteBuilding;
    private bool hasImage;

    #region Variables Serializadas Privadas
    [SerializeField] private GameObject scrollCaracteristica, caracteristicaContainer, offer, info, description, buttons, card, descriptionContainer, descriptionText, addCart, buyProducto;
    [SerializeField] private Button viewButton, moreInfo;
    [SerializeField] private RawImage imagen;
    [SerializeField] private Text title;

    private SaveComponentsController m_saveComponentsController;
    #endregion

    #region Variables estáticas
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
        m_saveComponentsController = FindObjectOfType<SaveComponentsController>();
        hasImage = false;

        gameObject.name = favoriteBuilding.Id_Inmueble.ToString();//Nombre del contenedor en unity.
        title.text = favoriteBuilding.Nombre_Inmueble;//Titulo del contenedor

        imagen.DOFade(0, 0);

        viewButton.onClick.RemoveAllListeners();
        viewButton.onClick.AddListener(() =>
        {
            m_saveComponentsController.StartToLoadData(favoriteBuilding.Id_Inmueble,favoriteBuilding.Codigo_Inmueble);
            m_saveComponentsController.GetBuildingCodeUseToSave(favoriteBuilding.Codigo_Inmueble);
        });

        moreInfo.onClick.RemoveAllListeners();
        moreInfo.onClick.AddListener(() =>
        {
            FindObjectOfType<MessagesControllers>().CreateInfo(favoriteBuilding.Descripcion_Inmueble);//activa la animaci?n de informaci?n de producto con los elementos
        });

        description.GetComponent<Text>().text = favoriteBuilding.Descripcion_Inmueble;
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
    #region Descarga de Imagenes
    /// <summary>
    /// Descarga un arreglo de imagenes a base 64
    /// </summary>
    /// <returns></returns>
    private void DownloadImageBuilders()
    {
        //Lista de Pictures
        var favoriteBuildingPictures = new List<Pictures> {

            new Pictures{Picture = favoriteBuilding.Picture1},
            new Pictures{Picture = favoriteBuilding.Picture2},
            new Pictures{Picture = favoriteBuilding.Picture3},
            new Pictures{Picture = favoriteBuilding.Picture4},
            new Pictures{Picture = favoriteBuilding.Picture5}
       };

        Texture2D defaultTexture = Resources.Load<Texture2D>("404");
        GameObject template = caracteristicaContainer.transform.GetChild(0).gameObject;
        // Recorriendo la lista con un bucle foreach
        foreach (Pictures picture in favoriteBuildingPictures)
        {
            //Debug.Log($"Valor de picture:{picture.Picture} del inmueble {favoriteBuilding.Id_Inmueble}");
            try
            {

                if (string.IsNullOrEmpty(picture.Picture))
                {
                    Debug.LogError($"la imagen del inmueble {favoriteBuilding.Id_Inmueble} está vacia o es nula");
                    CreateImageInstance(defaultTexture, template);
                    continue;
                }
                
                //string base64String = picture.Picture.Substring(picture.Picture.IndexOf(',') + 1);
                string base64String = CleanBase64(picture.Picture);

                if (!IsBase64String(base64String))
                {
                    Debug.LogError($"Imagen inválida en inmueble {favoriteBuilding.Id_Inmueble}. Base64 corrupta.");
                    CreateImageInstance(defaultTexture, template);
                    continue;
                }

                // Decodificar la cadena Base64
                byte[] bytes = Convert.FromBase64String(base64String);
                // Create a new texture
                Texture2D imageTexture = new Texture2D(1, 1);
                // Load image bytes into the texture
                imageTexture.LoadImage(bytes);

                // Asignar la textura al componente RawImage
                CreateImageInstance(imageTexture, template);
            }
            catch(Exception ex) 
            {
                Debug.LogError($"Error al cargar las imagenes del inmueble {favoriteBuilding.Id_Inmueble}:{ex.Message}");    
            }
        }
    }

    private void CreateImageInstance(Texture2D texture, GameObject template)
    {
        var instance = Instantiate(template, caracteristicaContainer.transform);
        instance.GetComponent<RawImage>().texture = texture;
        instance.SetActive(true);
    }

    private string CleanBase64(string base64)
    {
        if (base64.Contains(","))
            base64 = base64.Substring(base64.IndexOf(',') + 1);
        return base64.Trim().Replace("\r", "").Replace("\n", "");
    }

    private bool IsBase64String(string base64)
    {
        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }

    #endregion

}

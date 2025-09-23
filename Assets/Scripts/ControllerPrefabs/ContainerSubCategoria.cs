using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class ContainerSubCategoria : ScrollItem
{
    #region Variables privadas serializadas
    [SerializeField]
    private RawImage rawImage;// componente RawImage que hace referencia a la imagen de la subcategoria
    private Menu m_menuManager;
    [SerializeField]
    private Text text; 
    #endregion

    #region Variables privadas
    private bool hasImage;// booleano que indica si se cuenta con la imagen 
    #endregion

    #region Variables publicas 
    public SubCategoria subCategory; //referencia a la clase Subcategoria
    #endregion

    #region SCROLL ITEM METHODS
    public override void onStart()
    {
        //Cambiamos el nombre del objeto por el id de nuestra subcategoria
        gameObject.name = subCategory.idProducto.ToString();

        //buscamos e inicalizamos nuestro Menu 
        m_menuManager = FindObjectOfType<Menu>();

        //Se encarga de cargar los productos disponibles en una subcategoria al seleccionarla
        gameObject.GetComponent<Button>().onClick.AddListener(() => {m_menuManager.InstatiateProducts(subCategory); FindObjectOfType<MessagesControllers>().CloseInfo();});

        //Obtenemos el componente image de la imagen
        hasImage = false;

        //Obtenemos el componente text del texto
        text.text = subCategory.nombreProducto;//asigna el nombre de cada producto

        rawImage.DOFade(0, 0);
    }
    public override void onVisible()
    {
        if (!hasImage)
        {
            hasImage = true;
            _ = dowloadTexture();
        }
    }
    public override void onCleanCache()
    {
        if (hasImage)
        {
            rawImage.texture = null;
            subCategory.FotoTexture2DSubCategoria = null;
            hasImage = false;
            rawImage.DOFade(0, 0);

        }
    }
    public override void onDestroy()
    {
        rawImage.texture = null;
        subCategory.FotoTexture2DSubCategoria = null;
    }
    #endregion

    #region Private Methods / Coroutines
    /// <summary>
    /// Descarga la textura correspondiente a las subcategorias
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid dowloadTexture() 
    {
        if (subCategory.FotoTexture2DSubCategoria == null)
        {
            hasImage = true;
            {
                Texture2D texture = await Downloads.DownloadTexture(subCategory.foto);
                int size = (int)rawImage.GetComponent<RectTransform>().rect.width;
                subCategory.FotoTexture2DSubCategoria = Texture2DHelper.scaleTexture(texture, size);
                rawImage.texture = subCategory.FotoTexture2DSubCategoria;
                rawImage.DOFade(1, .5f);
            }
        }
    }
    }
    #endregion

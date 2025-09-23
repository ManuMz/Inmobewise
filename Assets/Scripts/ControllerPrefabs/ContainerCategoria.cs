using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class ContainerCategoria : ScrollItem
{
    #region Variables publicas 
    public Categoria category; // referencia a la clase categorias 
    #endregion

    #region Variables privadas
    [SerializeField] private RawImage rawImage; // componente RawImage que hace referencia a la imagen de la categoria 
    private Text text; // componente de texto
    private bool hasImage; // booleano que indica si se cuenta con la imagen 
    private Menu m_menuManager; // referencia al script Menu.cs
    #endregion

    #region SCROLL ITEMS METHODS
    public override void onStart()
    {
        rawImage.DOFade(0, 0);
        hasImage = false;
        //objeto que al descargar se lo cambiamos por su id
        gameObject.name = category.idCategoria.ToString();
        //buscamos e inicalizamos nuestro queries controller
        m_menuManager = FindObjectOfType<Menu>();

        gameObject.GetComponent<Button>().onClick.AddListener(() => { m_menuManager.InstatiateSubCategories(category); FindObjectOfType<MessagesControllers>().CloseInfo(); });

        //Debug.Log("Valor de categoria: " + category.idCategoria.ToString());
        //Obtener el componente image para poder cambiar la foto
        text = transform.Find("Text").GetComponent<Text>();

        //Le pasamos el nombre de la categoria que se esta mostrando
        text.text = category.nombreCategoria;

        //escondemos la imagen para que al momento de cargar de el efecto de que aparece
        rawImage.DOFade(0, 0);
    }
    public override void onVisible()
    {
        if (!hasImage)
        {
            hasImage = true;
            _ = DownloadTexture();
        }
    }

    public override void onCleanCache()
    {
        rawImage.texture = null;
        category.FotoTexture2DCategoria = null;
        hasImage = false;
        rawImage.DOFade(0, 0);
    }
    public override void onDestroy() 
    {
        category.FotoTexture2DCategoria = null;
        hasImage = false;
    }
    #endregion

    #region Private Methods/Coroutines
    /// <summary>
    /// Descarga de la textura de acuerdo a la categoria
    /// </summary>
    /// <returns></returns>
    private async UniTask DownloadTexture()
    {
        if (category.FotoTexture2DCategoria == null)
        {
            hasImage = true;

            int size = (int)rawImage.GetComponent<RectTransform>().rect.width;

            category.FotoTexture2DCategoria = Texture2DHelper.scaleTexture(await Downloads.DownloadTexture(category.foto), size);
            rawImage.texture = category.FotoTexture2DCategoria;
            rawImage.DOFade(1, .75f);

        }
    }
    #endregion
}

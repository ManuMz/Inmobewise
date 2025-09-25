using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
//using static cartObject;

public class ContainerProducto : ScrollItem
{
    #region Variables publicas
    [HideInInspector]
    public Producto product;
    public static bool changeObj;
    public static int indexCharacteristic;
    #endregion

    #region Variables Privadas Serializadas 
    [SerializeField] private GameObject scrollCharacteristic, characteristicContainer, offer, info, buttons, card, descriptionContainer, descriptionText;
    [SerializeField] private GameObject addCart;
    [SerializeField] private GameObject buyProducto;
    [SerializeField] private RawImage rawiImage;
    [SerializeField] private Text title; 
    [SerializeField] private Text price, brand; 
    [SerializeField] private Button preview;
    [SerializeField] private GameObject loadingIndicator;

    //ContainerAddCart
    [SerializeField] private GameObject containerAddCart;
    [SerializeField] private AnimationCurve animationCurve;
    #endregion

    #region Variables Privadas
    private MenuRemplace menuRemplace;
    private DownloadModel downloadModel;
    private Caracteristica currentCharacteristic;
    private GameObject father;
    private bool hasImage;
    private int idCharacteristic;
    //ContainerAddCart
    private float duration=0.3f, delay=0;
    [SerializeField] private Vector2 startPoint, finalPoint;
    private RectTransform target;

    #endregion

    #region Events
    private UnityEvent containerAddCartOn, containerAddCartOff;
    #endregion


    #region Coroutines
    private IEnumerator ShowContainerAddCart()
    {
       
        float elapsed = 0; // tiempo transcurrido 
        while (elapsed<=delay)
        { 
            elapsed+= Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        while (elapsed <= duration) 
        {
            float percentage = elapsed / duration;// punto actual de interpolacion
            float curvePercentange = animationCurve.Evaluate(percentage);//evaluacion de curva
            elapsed += Time.deltaTime;
            Vector2 CurrentPosition = Vector2.Lerp(startPoint, finalPoint, curvePercentange);
            target= containerAddCart.GetComponent<RectTransform>();  
            target.anchoredPosition = CurrentPosition;
            yield return null;
        }

        target.anchoredPosition = finalPoint;

    }

    private IEnumerator HideContainerAddCart()
    {
        
        yield return null;  
    }
    #endregion

    #region SCROLL ITEM METHODS

    public override void onVisible()
    {
        if (!hasImage)
        {
            hasImage = true;
            LoadingIndicator(hasImage);
            _ = DownloadTexture(product.Caracteristicas[indexCharacteristic].imagenes[0].nombreImagen);//Llama al metodo que descargara la imagen de cada caracteristica
            _ = DownloadColors();
        }
    }
    public override void onStart()
    {
        menuRemplace = FindObjectOfType<MenuRemplace>();
        downloadModel = FindObjectOfType<DownloadModel>();

        father = transform.parent.gameObject;

        hasImage = false;
        LoadingIndicator(hasImage);
        gameObject.name = product.idProBodPre.ToString();
        
        title.text = product.descripcion;
        brand.text = product.nombreMarca;
        indexCharacteristic = 0;
        price.text = VerifyPrice();

        rawiImage.DOFade(0, 0);
        idCharacteristic = product.Caracteristicas[indexCharacteristic].idCaracteristica;

        preview.onClick.RemoveAllListeners();

        //Se encarga de inicar la descarga del producto a colocar, realiza una accion de vibracion y cierra el menuAR
        preview.onClick.AddListener(() =>
        {
            changeObj = IsContainerToChangeProdct(this.gameObject);
            if (changeObj == false)
            {
                UIController.Instance.SwitchMenu();//OCULTAR EL MENU
            }
            else
            {
                menuRemplace.HideMenu();
            }
            downloadModel.StartDownload(product, idCharacteristic);
        });

        if (product.Caracteristicas.Count <= 1)
        {
            scrollCharacteristic.SetActive(false);
            characteristicContainer.SetActive(false);
        }

        info.GetComponent<Button>().onClick.AddListener(() =>
        {
            FindObjectOfType<MessagesControllers>().CreateInfo(product.descripcionProducto/*descripcionLarga*/);//activa la animación de información de producto con los elementos
        });

        currentCharacteristic = product.Caracteristicas[0];

        //Se va a compra directa del producto
        buyProducto.GetComponent<Button>().onClick.AddListener(() => {
                bool thereIsAnExistenceProduc = true;
                foreach (var caracteristica in product.Caracteristicas)
                {
                    Debug.Log("La existencia de tu producto es de: " + caracteristica.existencia);
                    if (int.Parse(caracteristica.existencia) == 0)
                    {
                        thereIsAnExistenceProduc = false;//NO HAY PRODUCTOS EN EXISTENCIA
                    }
                }
                if (thereIsAnExistenceProduc)
                {
                    FindObjectOfType<CartController>().OnlyBuy(product.idProBodPre.ToString(), idCharacteristic.ToString());
                }
                else
                {
                    MessagesControllers.sharedInstance.CreateNotification("Error", "Lo sentimos, producto sin existencia.", 2f, MessagesControllers.iconsNotifications.error);
                }
                
            });

        //Agrega el producto al carrito
        ////while (quantity<product.Caracteristicas.existencias)
        //addCart.GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    containerAddCart.SetActive(true);  //Activado

        //    //containerAddCart.transform.GetChild(1).GetComponent<Text>().text=; //Contador o score
        //    StartCoroutine(ShowContainerAddCart());

        //    //Comunicacion con webview


        //    //addCart.SetActive(false);

        //}); 
        addCart.GetComponent<Button>().onClick.AddListener(() =>
        {
            bool thereIsAnExistenceProduc = true; //Verificacion de la existencia del producto 
            foreach (var caracteristica in product.Caracteristicas)
            {
                Debug.Log("La existencia de tu producto es de: " + caracteristica.existencia);
                if (int.Parse(caracteristica.existencia) == 0)
                {
                    thereIsAnExistenceProduc = false;//NO HAY PRODUCTOS EN EXISTENCIA
                }
            }
            if (thereIsAnExistenceProduc)
            {
                FindObjectOfType<CartController>().AddCart(
                    product.idProBodPre.ToString(), idCharacteristic.ToString()
                    );
            }
            else
            {
                MessagesControllers.sharedInstance.CreateNotification("Error", "Lo sentimos, producto sin existencia.", 2f, MessagesControllers.iconsNotifications.error);
            }

        });
    }
    public override void onCleanCache()
    {
        if (hasImage)
        {
            rawiImage.texture = null;
            product.Caracteristicas[0].imagenes[0].FotoTexture2DImagen = null;
            rawiImage.DOFade(0, 0);
            foreach (var item in product.Caracteristicas)
            {
                item.imagenes[0].FotoTexture2DImagen = null;
                item.fotoTexture2DCaracteristica = null;
            }
            for (int i = 1; i < characteristicContainer.transform.childCount; i++)
            {
                Destroy(characteristicContainer.transform.GetChild(i).gameObject);
            }
            hasImage = false;
        }
    }

    
    public override void onDestroy()
    {
        rawiImage.texture = null;
        product.Caracteristicas[0].imagenes[0].FotoTexture2DImagen = null;
        foreach (var item in product.Caracteristicas)
        {
            item.imagenes[0].FotoTexture2DImagen = null;
            item.fotoTexture2DCaracteristica = null;
        }
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Define si el contenedor de producto es del Menu principal o del menu de cambio de productos
    /// </summary>
    /// <param name="container"></param>
    private bool IsContainerToChangeProdct(GameObject content) //Si es true es del menu de remmplazo
    {
        bool value = false;
        if (father.name == "Main Menu Content" && father.name == content.transform.parent.name)
        {
            value = false;
        }
        if (father.name == "Change Menu Content" && father.name == content.transform.parent.name)
        {
            value = true;
        }
        return value;
    }

    private void AddCartButton()
    {
        int count = 0;

        //while(){
        
            
         
        //}
    }

    private string VerifyPrice()
    {
        string price = "";
        if (product.Caracteristicas[indexCharacteristic].statusOferta.ToString() == "1")
        {
            offer.SetActive(true);
            float precio = float.Parse(product.Caracteristicas[indexCharacteristic].precio);
            float preciOferta = float.Parse(product.Caracteristicas[indexCharacteristic].precioOferta);
            float total = (float)(Math.Round((double)(100 - ((preciOferta * 100)) / precio), 0));
            price = "<size=18><color=red>$" + product.Caracteristicas[indexCharacteristic].precioOferta + "</color></size>\n<size=8> Antes: $" + product.Caracteristicas[indexCharacteristic].precio + "(-" + total + "%) </size>";
        }
        else
        {
            offer.SetActive(false);
            price = "$" + product.Caracteristicas[indexCharacteristic].precio;
        }
        return price;
    }

    private void ChangeColor(int idCaracteristica)
    {
        this.idCharacteristic = idCaracteristica;
        foreach (var item in product.Caracteristicas)
        {
            if (idCaracteristica == item.idCaracteristica)
            {
                indexCharacteristic = product.Caracteristicas.FindIndex(x => x.idCaracteristica == item.idCaracteristica);
                price.text = VerifyPrice();
                rawiImage.DOFade(0, .5f);
                _ = DownloadTexture(item.imagenes[0].nombreImagen);

                preview.onClick.RemoveAllListeners();
                preview.onClick.AddListener(() => {
                    changeObj = IsContainerToChangeProdct(this.gameObject);
                    if (changeObj == false)
                    {
                        UIController.Instance.SwitchMenu();//OCULTAR EL MENU
                    }
                    else
                    {
                        menuRemplace.HideMenu();
                    }
                    //UIController.sharedInstance.SwitchMenu();//OCULTAR EL MENU
                    downloadModel.StartDownload(product, this.idCharacteristic);});

                buyProducto.GetComponent<Button>().onClick.RemoveAllListeners();
                buyProducto.GetComponent<Button>().onClick.AddListener(() => { FindObjectOfType<CartController>().OnlyBuy(product.idProBodPre.ToString(), idCaracteristica.ToString()); });
                addCart.GetComponent<Button>().onClick.RemoveAllListeners();
                addCart.GetComponent<Button>().onClick.AddListener(() => { FindObjectOfType<CartController>().AddCart(product.idProBodPre.ToString(), idCaracteristica.ToString()); });
                break;
            }
        }
    }
    #endregion

    #region Private Async Methods
    private async UniTask DownloadTexture(string url)
    {
        if (product.Caracteristicas[indexCharacteristic].imagenes[0].FotoTexture2DImagen == null)
        {
            int size = (int)rawiImage.GetComponent<RectTransform>().rect.width;

            product.Caracteristicas[indexCharacteristic].imagenes[0].FotoTexture2DImagen = await Downloads.DownloadTexture(url);
            rawiImage.texture = product.Caracteristicas[indexCharacteristic].imagenes[0].FotoTexture2DImagen;
            rawiImage.DOFade(1, .5f);
            hasImage = true;
        }
        else
        {
            rawiImage.texture = product.Caracteristicas[indexCharacteristic].imagenes[0].FotoTexture2DImagen;
            rawiImage.DOFade(1, .5f);
        }
    }
    /// <summary>
    /// Descarga los colores de un producto, si tiene más colores(ejem. rojo,verde,azul etc.) los botones que aparecen
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid DownloadColors()
    {
        foreach (var item in product.Caracteristicas) //Recorre la lista de caracteristicas del producto
        {
            item.fotoTexture2DCaracteristica = await Downloads.DownloadTexture(item.foto);//descarga la imagen que se aparecera del color
            GameObject temp = characteristicContainer.transform.GetChild(0).gameObject;
            temp = Instantiate(temp, characteristicContainer.transform);
            temp.GetComponent<Button>().onClick.AddListener(() => { ChangeColor(item.idCaracteristica);});
            temp.GetComponent<RawImage>().texture = item.fotoTexture2DCaracteristica;
            temp.SetActive(true);
        }
    }

    /// <summary>
    /// /// Encendido/apagado del indicador de carga de productos 
    /// /// </summary>
    /// /// <param name="hasImage"></param>
    private void LoadingIndicator(bool hasImage)
    {
        if (hasImage == true)
        {
            loadingIndicator.SetActive(false);
        }
        else if (hasImage == false)
        {
            loadingIndicator.SetActive(true);
        }
    }
    #endregion
}
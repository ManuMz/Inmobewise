using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;

#region Estructura carrito
[Serializable]
public class CartObject
{
    public string nombre;
    public string correo;
    public string telefono;
    public string postal_code;
    public CartCollection _cartCollection;
    public string redirectPayment;
}

[Serializable]
public class CartCollection
{
    public List<Cart> data = new List<Cart>();

}
[Serializable]
public class Cart
{
    public string cp;
    public string idProBodPre;
    public string idCaracteristica;
    public string cantidad;
    public bool isValid;
    public int iddCaracteristicaProducto;
    
}

[Serializable]
public class DimensionCollection
{
    public List<Dimension> dimensionsData = new();
}

[Serializable]
public class Dimension
{
    public string idProdBodPre;
    public string alto;
    public string ancho;
    public string largo;
}
#endregion
public class CartController : MonoBehaviour
{
    public static CartController sharedInstance;
    [HideInInspector]
    public config m_config;
    [Header("Visualizacion de variables")]
    public string almacenarRespuesta;
    #region PRIVATE VARIABLES
    //private List<Cart> contentCart = new List<Cart>();
    private MessagesControllers messages;
    //private string[] separadores = { "CP:", "idProBodPre:", "idCaracteristica:" };
    private WebViewController webViewManager;
    //private Usuario usuarioManager;
    private QueriesController queriesManager;
    private CartCollection _cartCollection;

    private Ubication ubication;
    //TestingPlacedMenu testingPlacedMenu;
    TouchController touchController;

    private bool keepProductInCart;
    private Cart lastProductInCart = new ();
    private string productName;
    private int totalHeightProdCart; //altura total acumulada de cada producto que se encuentra en el carrito
    private int totalWidthProdCart;  // ancho total acumulado de cada producto que se encuentra en el carrito
    private int totalLengthProdCart;// largo total acumulado de cada producto que se encuentra en el carrito 
    private bool lastProductEliminated = false;
    #endregion

    #region PUBLIC VARIABLES
    public static string route;
    public CartCollection _toViewCartCollection;
    public CartObject _cartObject;

    //public SubMenuFloor subMenuFloor;
    #endregion


    #region MONOBEHAUVIOUR METHODS
    private void Awake()
    {
        if (!sharedInstance)
            sharedInstance = this;
        else
            Destroy(sharedInstance);
        
        if (PlayerPrefs.GetString("cart") != "")
        {
            try //Si el playerpref Cart no es nulo, deserializa la cadena JSON y la asigna a _cartCollection 
            {
                _cartCollection = JsonUtility.FromJson<CartCollection>(PlayerPrefs.GetString("cart"));

                totalHeightProdCart = PlayerPrefs.GetInt("totalHeight");
                totalWidthProdCart = PlayerPrefs.GetInt("totalWidth");
                totalLengthProdCart = PlayerPrefs.GetInt("totalLength");
            }

            catch (Exception e) // Si el playerpref es nulo, inicializa _cartCollection 
            {
                print(e.Message);
                PlayerPrefs.SetString("cart", "");
                _cartCollection = new CartCollection
                {
                    data = new List<Cart>()
                };
                //si el carrito vuelve a inicializarse, los totales de las dimensiones fisicas parten de cero
                totalHeightProdCart = PlayerPrefs.GetInt("totalHeight", 0);
                totalWidthProdCart = PlayerPrefs.GetInt("totalWidth", 0);
                totalLengthProdCart = PlayerPrefs.GetInt("totalLength", 0);
            }
        }
        else //Si el playerpref es nulo, inicializa _cartCollection 
        {
            PlayerPrefs.SetString("cart", "");
            //Inicializamos carrito
            _cartCollection = new CartCollection
            {
                data = new List<Cart>()
            };
            //si el carrito vuelve a inicializarse, los totales de las dimensiones fisicas parten de cero
            totalHeightProdCart = PlayerPrefs.GetInt("totalHeight", 0);
            totalWidthProdCart = PlayerPrefs.GetInt("totalWidth", 0);
            totalLengthProdCart = PlayerPrefs.GetInt("totalLength", 0);
        }
    }

    private void Start()
    {
        webViewManager = WebViewController.sharedInstance;
        touchController = FindObjectOfType<TouchController>();
        messages = FindObjectOfType<MessagesControllers>();
        queriesManager = FindObjectOfType<QueriesController>();
        //usuarioManager = FindObjectOfType<Usuario>();
        ubication = FindObjectOfType<Ubication>();
        m_config = FindObjectOfType<config>();

        Debug.Log("Alto acumulado al iniciar la aplicación: " + totalHeightProdCart);
        Debug.Log("Ancho acumulado al iniciar la aplicación: " + totalWidthProdCart);
        Debug.Log("largo acumulado al iniciar la aplicación: " + totalLengthProdCart);
    }
    #endregion

    /// <summary>
    /// Elimina un producto del carrito
    /// </summary>
    /// <param name="id"></param>
    public void DeleteCart(int id)
    {
        CartCollection tempCartCollection = new CartCollection();

        foreach (var item in _cartCollection.data)
        {
            // compara si su cp y id del producto, si no coinciden con el producto a eliminar, se mantiene en el carrito.
            if (!(item.cp == ubication.GetPostalCode() && item.idProBodPre == id.ToString()))
            {
                tempCartCollection.data.Add(item);
            }
        }
        _cartCollection.data = tempCartCollection.data;
        SaveChanges();
    }

    public void DeleteLastProductInCart(int idProBodPre )
    {
        CartCollection tempCartCollection = new CartCollection();

        foreach (var item in _cartCollection.data)
        {
            // compara si su cp y id del producto, si no coinciden con el producto a eliminar, se mantiene en el carrito.
            if (!(item.cp == ubication.GetPostalCode() && item.idProBodPre == idProBodPre.ToString()))
            {
                tempCartCollection.data.Add(item);
                lastProductEliminated = true;
            }
        }
        _cartCollection.data = tempCartCollection.data;
        SaveChanges();

        if (lastProductEliminated)
        {
            //TutorialControl.sharedInstance.InstatiateAvailablePackage(tutorialItems.availablePackage);
            Debug.Log("Producto eliminado");
        }
        else
        {
            Debug.Log("Error al eliminar el producto");
            //MessagesControllers.sharedInstance.CreateNotification("Error", "Ocurrio una falla al eliminar el producto",
             //   3f, MessagesControllers.iconsNotifications.error);
        }
    }

    public void RecalculatePhysicDimensionsTotales(int idProBodPre)
    {
        Producto productInCart = queriesManager.GetProductByIdProBodPre(idProBodPre);//Querie para obtener las dimensiones del producto
                                                                                     
        // Del total  obtenido se resta lo correspondiente al producto por eliminar 
        totalHeightProdCart -= int.Parse(productInCart.alto);
        totalWidthProdCart -= int.Parse(productInCart.ancho);
        totalLengthProdCart -= int.Parse(productInCart.largo);

        Debug.Log("Alto acumulado al ELIMINAR el producto " + productInCart.idProBodPre + ": " + totalHeightProdCart + " cm");
        Debug.Log("Ancho acumulado al ELIMINAR el producto " + productInCart.idProBodPre + ": " + totalWidthProdCart + " cm");
        Debug.Log("Largo acumulado al ELIMINAR el producto " + productInCart.idProBodPre + ": " + totalLengthProdCart + " cm");

        SaveTotals();
    }
    /// <summary>
    /// Comprar un solo producto, se verifica que haya una sesion antes de realizar la compra,
    /// Se verifica que el producto exista y si todo esta bien abre el webview.
    /// </summary>
    public void OnlyBuy()
    {
        //if (usuarioManager.CheckSesion())//Revisamos si hay sesion iniciada
        //{
        //    //GameObject actualModel = TouchController.sharedInstance.p_objectSelected;
        //    GameObject actualModel = touchController.p_objectSelected;

        //    //if (actualModel.GetComponent<FloorBehaviour>())
        //    //{
        //        //if (actualModel.GetComponent<FloorBehaviour>().lastProductSelected != null && actualModel.GetComponent<FloorBehaviour>().lastProductSelected.descripcion != "")
        //        //{
        //            //ObjectProperties lastProdObjBehav = actualModel.GetComponent<FloorBehaviour>().objectProperties;

        //            //actualModel.GetComponent<ObjectProperties>().idProBodPre = actualModel.GetComponent<FloorBehaviour>().lastProductSelected.idProBodPre;
        //            //actualModel.GetComponent<ObjectProperties>().idSelected = actualModel.GetComponent<FloorBehaviour>().lastProductSelected.Caracteristicas[ContainerProducto.indexCharacteristic].idCaracteristica;
        //            //Debug.LogError("Cambio de producto comprado \n IDProd:" + lastProdObjBehav.idProBodPre + "\n IDSel: " + lastProdObjBehav.idSelected);
        //        //}
        //    //}
        //    if (!queriesManager.CheckProduct(actualModel.GetComponent<ObjectProperties>().idProBodPre))//Revisamos que el producto exista en la informacion descargada
        //    {
        //        messages.CreateNotification("Carrito", "El producto no esta disponible", 2f, MessagesControllers.iconsNotifications.warning);
        //        return;
        //    }

        //    CartCollection tempCartCollection = new();
        //    Cart _cart = new();
        //    _cart.cp = ubication.GetPostalCode();
        //    _cart.idProBodPre = actualModel.GetComponent<ObjectProperties>().idProBodPre.ToString();
        //    //_cart.idCaracteristica = actualModel.GetComponent<ObjectProperties>().idSelected.ToString();
        //    _cart.idCaracteristica = actualModel.GetComponent<ObjectProperties>().caracteristicas[0].id.ToString();
        //    _cart.cantidad = "1";
        //    //_cart.isValid = true;
        //    tempCartCollection.data.Add(_cart);
        //    _toViewCartCollection = tempCartCollection;
        //    string url = config.sharedInstance.urlArviSpaceForms("cart", config.localFiles.yes);

        //    var webView1 = gameObject.AddComponent<UniWebView>();
        //    webViewManager.ShowWebView(webView1, url, 1);
        //    //messages.createAnswer("Aviso", "El codigo postal actual es: " + FindObjectOfType<Ubication>().p_postal_code + "\n ¿Desea continuar?", ()=>messages.closeAnswer(), () => { webViewManager.showWebView(url); messages.closeAnswer(); });
        //}
        //else
        //{
        //    messages.CreateAnswer("Aviso", "No hay sesión iniciada. \n ¿Deseas iniciar sesión?",
        //      () => { messages.CloseAnswer(); },
        //      () => { usuarioManager.OpenProfile(); messages.CloseAnswer();}
        //    );
        //}
    }

    /// <summary>
    /// Compra directa de un producto
    /// </summary>
    /// <param name="idProBodPre"></param>
    /// <param name="idCaracterisitica"></param>
    public void OnlyBuy(string idProBodPre, string idCaracterisitica)
    {
        //// se verifica si el usuario ha iniciado sesion
        //if (usuarioManager.CheckSesion())
        //{
        //    // se verifica si el producto existe por su ID
        //    if (!queriesManager.CheckProduct(int.Parse(idProBodPre)))//Revisamos que el producto exista en la informacion descargada
        //    {
        //        messages.CreateNotification("Carrito", "El producto no esta disponible", 2f, MessagesControllers.iconsNotifications.warning);
        //        return;
        //    }

        //    // se crea un obj temporal para almacenar el carrito
        //    CartCollection tempCartCollection = new();
        //    Cart _cart = new();
        //    _cart.cp = ubication.GetPostalCode();
        //    _cart.idProBodPre = idProBodPre;
        //    _cart.idCaracteristica = idCaracterisitica;
        //    _cart.cantidad = "1";
        //    //_cart.isValid = true;
        //    tempCartCollection.data.Add(_cart);

        //    _toViewCartCollection = tempCartCollection;
        //    // se construye la url para mostrar el carrito en el webview
        //    string url = config.sharedInstance.urlArviSpaceForms("cart", config.localFiles.yes);
        //    var webView1 = gameObject.AddComponent<UniWebView>();
        //    webViewManager.ShowWebView(webView1, url, 1);
        //    //messages.createAnswer("Aviso", "El codigo postal actual es: " + FindObjectOfType<Ubication>().p_postal_code + "\n ¿Desea continuar?", () => messages.closeAnswer(), () => { webViewManager.showWebView(url); messages.closeAnswer(); });
        //}
        //else
        //{
        //    messages.CreateAnswer("Aviso", "No hay sesión iniciada. \n ¿Deseas iniciar sesión?",
        //       () => { messages.CloseAnswer(); },
        //       () => { usuarioManager.OpenProfile(); messages.CloseAnswer(); }
        //   );
        //}

    }
    /// <summary>
    /// Abre el carrito
    /// </summary>
    public void OpenCart()
    {
        if (TutorialControl.sharedInstance.IsPlaying() && TutorialControl.sharedInstance.CurrentTutorial() == tutorialItems.notAvailablePackage)
        {
            FindObjectOfType<TutorialItem>().toDestroy = true;
        }

        //if (usuarioManager.CheckSesion())//revisamos si hay sesion iniciada
        //{
        //    Debug.Log("carrito: " + PlayerPrefs.GetString("cart"));
        //    //if (_cartCollection.data.Count > 0)
        //    if (PlayerPrefs.GetString("cart") != "")//verificamos si player pref tiene algo inicializado
        //    {
        //        foreach (var item in _cartCollection.data)//recorremos en busca de productos del actual codigo postal
        //        {
        //            Debug.Log("carrito prod: " + item.idProBodPre);
        //            //print(item.cp +" :: "+ ubication.getPostalCode());
        //            if (item.cp == ubication.GetPostalCode())//si entra una vez cambiamos bandera y rompemos ciclo
        //            {
        //                item.isValid = true;
        //                keepProductInCart = true;

        //            }
        //            else
        //            {
        //                  item.isValid = false;
        //                  keepProductInCart = false;
        //            }
        //            //item.isValid = false;
        //        }

        //        Debug.Log("Alto acumulado al ABRIR EL CARRITO: " + totalHeightProdCart);
        //        Debug.Log("Ancho acumulado al ABRIR EL CARRITO:" + totalWidthProdCart);
        //        Debug.Log("largo acumulado al ABRIR EL CARRITO: " + totalLengthProdCart);

        //        //if (_cartCollection.data.Count > 0)//si bandera es true seguimos de lo contrario no hay productos
        //        if (keepProductInCart && _cartCollection.data.Count > 0)
        //        {
        //            _toViewCartCollection = _cartCollection;
        //            string url = config.sharedInstance.urlArviSpaceForms("cart", config.localFiles.yes);
        //            var webView1 = gameObject.AddComponent<UniWebView>();
        //            //  webViewManager.initnewview(webView1);
        //            webViewManager.ShowWebView(webView1, url, 1);
        //            //messages.createAnswer("Aviso", "El codigo postal actual es: " + FindObjectOfType<Ubication>().p_postal_code + "\n ¿Desea continuar?", ()=>messages.closeAnswer(), () => { webViewManager.showWebView(url); messages.closeAnswer(); });
        //        }

        //        else
        //        {
        //            messages.CreateNotification("Carrito", "No existen productos en el carrito", 2f, MessagesControllers.iconsNotifications.warning);
        //            ClearCart();
        //            //si el carrito vuelve a inicializarse, los totales de las dimensiones fisicas parten de cero
        //            totalHeightProdCart = PlayerPrefs.GetInt("totalHeight", 0);
        //            totalWidthProdCart = PlayerPrefs.GetInt("totalWidth", 0);
        //            totalLengthProdCart = PlayerPrefs.GetInt("totalLength", 0);
        //        }
        //    }
        //    else
        //    {
        //        //messages.CreateNotification("Carrito", "Error con la información del carrito", 2f, MessagesControllers.iconsNotifications.error);
        //        messages.CreateNotification("Carrito", "No hay productos en su carrito", 2f, MessagesControllers.iconsNotifications.warning);
        //    }
        //}
        //else
        //{
        //    messages.CreateAnswer("Aviso", "No hay sesión iniciada. \n ¿Deseas iniciar sesión?",
        //      () => { messages.CloseAnswer(); },
        //      () => { usuarioManager.OpenProfile(); messages.CloseAnswer(); }
        //    );

        //}
    }

    /// <summary>
    /// Revisa si el producto ya existe
    /// </summary>
    /// <param name="idProBodPre"></param>
    /// <returns></returns>
    public bool CheckExists(string idProBodPre)
    {
        // convierte el id de string a int
        int id = int.Parse(idProBodPre);
        // recorre los elementos del carrito
        foreach (var item in _cartCollection.data)
        {
            // se verifica si el id conincido con el id en el carrito
            if (id.ToString() == item.idProBodPre)
            {
                Debug.Log("checkExist" + item.idProBodPre);
                // se retorna true si el producto ya existe
                return true;
            }
        }

        return false;
    }
    #region ServicioExistencias

    ////public bool checkExistsBD(string idCaracteristica, string cantidad)
    ////{
    ////    int id = int.Parse(idCaracteristica);
    ////    string service = m_config.urlArvispaceServices("getConsultaExistenciasProductos.php");
    ////    WWWForm form = new WWWForm();
    ////    form.AddField("idCaracteristica", idCaracteristica);
    ////    form.AddField("cantidad", cantidad);

    ////    var data = (await UnityWebRequest.Post(service, form).SendWebRequest()).downloadHandler.text;


    ////    _cartCollection.data = new List<cart>();

    ////    _cartCollection.data = deserializeJsonPromo(data);

    ////    /*foreach (var item in _cartCollection.data)
    ////    {
    ////        if (id.ToString() == item.idProBodPre)
    ////        {
    ////            Debug.Log("checkExist" + item.idProBodPre);
    ////            return true;
    ////        }
    ////    }*/

    ////    return false;
    ////}
    //List<cart> deserializeJsonPromo(string json)
    //{
    //    List<cart> list = new List<cart>();
    //    try
    //    {
    //        list = JsonHelper.getJsonArray<cart>(json).ToList();
    //        //foreach (var item in list)
    //        //{
    //        //    item.SubCategorias.ForEach(x => x.Productos.ForEach(j => j.cp = ubication.p_postal_code));
    //        //}
    //        _cartCollection.data = list.ToList();
    //    }
    //    catch (Exception)
    //    {
    //        //retry();
    //    }
    //    return list.ToList();
    //}
    //public async UniTask<string> getExistencias(string idCaracteristica, string cantidad)
    //{
    //    //Este es para validar las existencias en la base de datos, si no se quiere hacer asi solo es cuetsion de comentar la funcion.

    //    //Debug.Log("1.- entro a funcion: " + idCaracteristica);

    //    //WWWForm form = new WWWForm();
    //    //form.AddField("idCaracteristica", idCaracteristica);
    //    //form.AddField("cantidad", cantidad);

    //    ////string service = m_config.urlArvispaceServices("");
    //    //try
    //    //{
    //    //    var data = (await UnityWebRequest.Post(m_config.urlArvispaceServices("getConsultaExistenciasProductos.php"), form).SendWebRequest()).downloadHandler.text;
    //    //    _cartCollection.data = new List<cart>();
    //    //    _cartCollection.data = deserializeJsonPromo(data);
    //    //    return idCaracteristica;
    //    //}
    //    //catch(Exception)
    //    //{
    //    //    return cantidad;
    //    //}

    //    string service = m_config.urlArvispaceServices("getConsultaExistenciasProductos.php");
    //    WWWForm form = new WWWForm();
    //    form.AddField("idCaracteristica", idCaracteristica);
    //    form.AddField("cantidad", cantidad);

    //    var data = (await UnityWebRequest.Post(service, form).SendWebRequest()).downloadHandler.text;


    //    _cartCollection.data = new List<cart>();

    //    _cartCollection.data = deserializeJsonPromo(data);
    //   var datosExist = _cartCollection.data;

    //    return datosExist;


    //    //_cartCollection.data = new List<cart>();

    //    // _cartCollection.data = deserializeJsonPromo(data);


    //    //Debug.Log("2.-paasa consultar servicio: " + data);
    //    //Debug.Log("3.- paasa _cartCollection: " + _cartCollection.data);

    //    //print("GERARDO:  "+_cartCollection.data);

    //}
    #endregion
    /*public void addCart()
    {
        GameObject actualModel = FindObjectOfType<TestingPlacedMenu>().p_objectSelected;

        Debug.Log("SE PRESIONO EL BOTON ADDCART");
        //if (FindObjectOfType<config>().app.Equals(config.apps.Interceramic) || FindObjectOfType<config>().app.Equals(config.apps.Vitromex))
        //{
        //    int quantity = actualModel.GetComponent<ObjectBehaviour>().cantidad;

        //    FindObjectOfType<OracleRestAPI>().addCartItem(actualModel.GetComponent<ObjectBehaviour>().idProBodPre, actualModel.GetComponent<ObjectBehaviour>().idSelected,quantity);
        //}
        //else
        //{
        if (!checkExists(actualModel.GetComponent<ObjectBehaviour>().idProBodPre.ToString()))
        {
            cart _cart = new cart();
            _cart.cp = ubication.getPostalCode();
            _cart.idProBodPre = actualModel.GetComponent<ObjectBehaviour>().idProBodPre.ToString();
            _cart.idCaracteristica = actualModel.GetComponent<ObjectBehaviour>().idSelected.ToString();
            _cart.cantidad = actualModel.GetComponent<ObjectBehaviour>().cantidad.ToString();
            _cartCollection.data.Add(_cart);
            saveChanges();
            Debug.Log("AddCart:  "+  _cart);
            messages.createNotification("Carrito", "Se ha agregado un producto a su carrito", 2f, MessagesControllers.iconsNotifications.success);

        }
        else
        {
            messages.createNotification("Carrito", "El producto ya existe en su carrito", 2f, MessagesControllers.iconsNotifications.warning);
        }

    }*/

    public void AddCart()
    {
        ///GameObject actualModel = touchController.objectSelected;
        GameObject actualModel = touchController.p_objectSelected;
        if (!CheckExists(actualModel.GetComponent<ObjectProperties>().idProBodPre.ToString()))
        {

            Producto productInCart = queriesManager.GetProductByIdProBodPre(int.Parse(
                actualModel.GetComponent<ObjectProperties>().idProBodPre.ToString()));//Querie para obtener las dimensiones del producto 
                                                                                                     
            Cart _cart = new();
            _cart.cp = ubication.GetPostalCode();
            _cart.idProBodPre = actualModel.GetComponent<ObjectProperties>().idProBodPre.ToString(); ;
            _cart.idCaracteristica = actualModel.GetComponent<ObjectProperties>().caracteristicas[0].id.ToString();
            _cart.cantidad = "1";
            //_cart.isValid = true;

            //agrega el objeto a la lista 
            _cartCollection.data.Add(_cart);

            SaveChanges(); //Guarda los cambios en DataCollection

            // Suma de variables de dimensión
            totalHeightProdCart += int.Parse(productInCart.alto);
            totalWidthProdCart += int.Parse(productInCart.ancho);
            totalLengthProdCart += int.Parse(productInCart.largo);

            Debug.Log("Alto acumulado al AGREGAR el producto " + productInCart.idProBodPre + ": " + totalHeightProdCart + " cm");
            Debug.Log("Ancho acumulado al AGREGAR el producto " + productInCart.idProBodPre + ": " + totalWidthProdCart + " cm");
            Debug.Log("Largo acumulado al AGREGAR el producto " + productInCart.idProBodPre + ": " + totalLengthProdCart + " cm");

            SaveTotals();
            //if (totalHeightProdCart > 300 || totalWidthProdCart > 300 || totalLengthProdCart > 300) //Si el alto, ancho o largo acumulado es mayor a 300 cm supera el limite
            if (totalHeightProdCart > 200 || totalWidthProdCart > 200) //Si el alto o ancho acumulado supera las 200 unidades                                                                                        
            {
                Debug.Log("Alto acumulado al SUPERAR el limite por el producto " + productInCart.idProBodPre + ": " + totalHeightProdCart + " cm");
                Debug.Log("Ancho acumulado al SUPERAR el limite por el producto " + productInCart.idProBodPre + ": " + totalWidthProdCart + " cm");
                Debug.Log("Largo acumulado al SUPERAR el limite por el producto " + productInCart.idProBodPre + ": " + totalLengthProdCart + " cm");

                lastProductInCart = _cartCollection.data.Last(); //ultimo producto en la lista cartcollection
                var lastProduct = queriesManager.GetProductByIdProBodPre(int.Parse(lastProductInCart.idProBodPre));

                //obtencion del nombre del ultimo producto 
                productName = lastProduct.descripcion;

                UIController.sharedInstance.HideMenu();
                TutorialControl.sharedInstance.InstatiateNotAvailablePackage(tutorialItems.notAvailablePackage, productName);//Notificación notAvailablePackage

                this.RecalculatePhysicDimensionsTotales(int.Parse(lastProductInCart.idProBodPre));
                this.DeleteLastProductInCart(int.Parse(lastProductInCart.idProBodPre));
            }
            else
            {
                messages.CreateNotification("Carrito", "producto agregado, puede seguir con más productos", 3f, MessagesControllers.iconsNotifications.success);
       
            }
        }
        else
        {
            messages.CreateNotification("Carrito", "El producto ya existe en el carrito", 3f, MessagesControllers.iconsNotifications.warning);
        }

    }

    /// <summary>
    /// Agrega al carrito tomando dos parametros que se ocupan para identificar el producto y su caracteristica
    /// </summary>
    /// <param name="idProBodPre"></param>
    /// <param name="idCaracteristica"></param>
    public void AddCart(string idProBodPre, string idCaracteristica)
    {
        if (!CheckExists(idProBodPre))
        {
            Producto productInCart = queriesManager.GetProductByIdProBodPre(int.Parse(idProBodPre));//Querie para obtener las dimensiones del producto                                                             
                                                                
            Cart _cart = new();
            _cart.cp = ubication.GetPostalCode();
            _cart.idProBodPre = idProBodPre;
            _cart.idCaracteristica = idCaracteristica;
            _cart.cantidad = "1";
            //_cart.isValid = true;

            //agrega el objeto a la lista 
            _cartCollection.data.Add(_cart);

            SaveChanges(); //Guarda los cambios en DataCollection

            // Suma de variables de dimensión;
            totalHeightProdCart += int.Parse(productInCart.alto);
            totalWidthProdCart += int.Parse(productInCart.ancho);
            totalLengthProdCart += int.Parse(productInCart.largo);

            Debug.Log("Alto acumulado al AGREGAR el producto " + productInCart.idProBodPre + ": " + totalHeightProdCart + " cm");
            Debug.Log("Ancho acumulado al AGREGAR el producto " + productInCart.idProBodPre + ": " + totalWidthProdCart + " cm");
            Debug.Log("Largo acumulado al AGREGAR el producto " + productInCart.idProBodPre + ": " + totalLengthProdCart + " cm");

            SaveTotals();

            //if (totalHeightProdCart > 300 || totalWidthProdCart > 300 || totalLengthProdCart > 300) //Si el alto, ancho o largo acumulado es mayor a 300 supera el limite
            if(totalHeightProdCart>200 || totalWidthProdCart >200) //Si el alto o ancho acumulado supera las 200 unidades                                                                                     
            {
                Debug.Log("Alto acumulado al SUPERAR el limite por el producto " + productInCart.idProBodPre + ": " + totalHeightProdCart + " cm");
                Debug.Log("Ancho acumulado al SUPERAR el limite por el producto " + productInCart.idProBodPre + ": " + totalWidthProdCart + " cm");
                Debug.Log("Largo acumulado al SUPERAR el limite por el producto " + productInCart.idProBodPre + ": " + totalLengthProdCart + " cm");
    
                lastProductInCart = _cartCollection.data.Last(); //ultimo producto en la lista cartcollection
                var lastProduct = queriesManager.GetProductByIdProBodPre(int.Parse(lastProductInCart.idProBodPre));

                //obtencion del nombre del ultimo producto 
                productName = lastProduct.descripcion;

                UIController.sharedInstance.HideMenu();
                TutorialControl.sharedInstance.InstatiateNotAvailablePackage(tutorialItems.notAvailablePackage, productName);//Notificación notAvailablePackage

                this.RecalculatePhysicDimensionsTotales(int.Parse(lastProductInCart.idProBodPre));
                this.DeleteLastProductInCart(int.Parse(lastProductInCart.idProBodPre));
            }
            else
            {
                messages.CreateNotification("Carrito", "producto agregado, puede seguir con más productos", 3f, MessagesControllers.iconsNotifications.success);
            }
        }
        else
        {
            messages.CreateNotification("Carrito", "El producto ya existe en el carrito", 3f, MessagesControllers.iconsNotifications.warning);
        }

    }
    
    

    // guarda los cambios
    public void SaveChanges()
    {
        // serializa _ cartcollection  a una cadena JSON y la guarda en PlayerPrefs con la clave cart
        PlayerPrefs.SetString("cart", JsonUtility.ToJson(_cartCollection));
    }


    public void SaveTotals()
    {
        PlayerPrefs.SetInt("totalHeight", totalHeightProdCart);
        PlayerPrefs.SetInt("totalWidth", totalWidthProdCart);
        PlayerPrefs.SetInt("totalLength", totalLengthProdCart);
    }

    /// <summary>
    /// Cambia el color del producto
    /// </summary>
    /// <param name="idProBodPre"></param>
    /// <param name="idCaracteristica"></param>
    public void ChangeColor(string idProBodPre, string idCaracteristica)
    {
        // ietera por cada elemneto en el _cartcollection
        foreach (var item in _cartCollection.data)
        {
            // si encuntra un producto con el mismo idProBodPre se actualiza el idCaracteristica
            if (item.idProBodPre == idProBodPre /*&& item.cp == ubication.getPostalCode()*/)
            {
                item.idCaracteristica = idCaracteristica;
                SaveChanges();
            }
        }
    }
    /// <summary>
    /// Limpia los elementos del _cartCollection
    /// </summary>
    public void ClearCart()
    {
        _cartCollection.data.Clear();
        SaveChanges();
    }
    /// <summary>
    /// Cambia la cantidad del producto
    /// </summary>
    /// <param name="idProBodPre"></param>
    /// <param name="idCaracteristica"></param>
    /// <param name="cantidad"></param>
    public void ChangeAmountByProduct(string idProBodPre, string idCaracteristica, string cantidad)
    {
        //Itera en cada producto del _cartCollection.
        foreach (var item in _cartCollection.data)
        {
            // Si encuentra un elemento con el idProBodPre y idCaracteristica coincidentes, se actualiza la cantidad
            if (item.idCaracteristica == idCaracteristica && item.idProBodPre == idProBodPre /*&& item.cp == ubication.getPostalCode()*/)
            {
                item.cantidad = cantidad;
                SaveChanges();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WebViewController : MonoBehaviour
{
    #region Miembro estático del patrón de diseño Singleton
    public static WebViewController sharedInstance;
    #endregion

    #region Variables privadas Serializadas

    [SerializeField]
    private QueriesController queriesController;//variable que instancia la clase QueriesController
    [SerializeField]
    private LoadingController loading; // variable que instancia la clase LoadingController
    [SerializeField]
    private List<GameObject> components;
    [SerializeField]
    private UniWebView webViewLocal;
    [SerializeField]
    private TutorialControl tutorialControl;
    #endregion

    #region Variables privadas
    [Range(0, 1)]
    private float speed;// margen de velocidad con la que se cerrará la vista web 
    private UnityAction pendingAction;
    private bool active;// booleano que indica si se encuentra activo o no UniWebView
    private bool isShown; // booleano que indica si la vista web se está mostrando o no 
    #endregion

    public bool GetIsShown()
    {
        return isShown;
    }

    void Awake()
    {
        if (!sharedInstance)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        InitUniWebView();
        pendingAction = null;
        active = false;
    }

    #region Private Methods
    /// <summary>
    /// Inicializa uniwebview con el indicador de carga deshabilitado  
    /// </summary>
    private void InitUniWebView()
    {
        loading.Close();
        queriesController = FindObjectOfType<QueriesController>();
    }
    /// <summary>
    /// Oculta la vista web
    /// </summary>
    /// <param name="webView1"></param>
    public void HideWebView(UniWebView webView1)
    {
        isShown = false;

        Debug.Log("se va esconder el webview 2");
        loading.Close();
        webView1.Hide(true, UniWebViewTransitionEdge.Top, speed);
        webView1.Stop();
        GameObject.Destroy(webView1);

        components[0].SetActive(false);
        components[0].GetComponent<Canvas>().sortingOrder = 0;

        Screen.orientation = ScreenOrientation.LandscapeLeft;

        active = false;
        if (FindObjectOfType<config>().mUseMode == config.useMode.testing)
        {
            // webView1.CleanCache();
        }

        if (pendingAction != null)
        {
            pendingAction.Invoke();
            pendingAction = null;
        }
    }

    

    /// <summary>
    /// Ejecuta el proceso de carga de la vista web de acuerdo a la url 
    /// </summary>
    /// <param name="webViewLocal"></param>
    /// <param name="url"></param>
    /// <param name="opSize"></param>
    public void InitWebView3(UniWebView webView3, string url, int opSize, System.Action onPageFinished) 
    {
        this.webViewLocal = webView3;


        if (opSize == 1) //opcion de tamaño para vistas web que no sean tutoriales
        {
            components[0].SetActive(true);
            components[0].GetComponent<Canvas>().sortingOrder = 13;

            webViewLocal.ReferenceRectTransform = components[1].GetComponent<RectTransform>();

        }
        else //opcion de tamaño para la vista web de un tutorial
        {
            components[0].SetActive(true);
            components[0].GetComponent<Canvas>().sortingOrder = 13;

            webViewLocal.ReferenceRectTransform = components[1].GetComponent<RectTransform>();
            
        }
        webViewLocal.Load(url);
        if (webViewLocal != null)
        {
            webViewLocal.OnShouldClose += (view) =>
            {
                HideWebView(webViewLocal);
                return false;
            };

            webViewLocal.OnMessageReceived += (view, message) =>
            {
                //Debug.Log("Valor de Messagee: " + message.Path);
                switch (message.Path)
                {
                    case "logguer":
                        //Usuario usuario = Usuario.sharedInstance;
                        //usuario.Login(
                        //    message.Args["correo"],
                        //    message.Args["userName"],
                        //    message.Args["nombreCompleto"],
                        //    message.Args["telefono"],
                        //    message.Args["password"]
                        //);
                        HideWebView(webViewLocal);
                        break;

                    case "auth":
                        HideWebView(webViewLocal);
                        break;
                    case "firstAuth":
                        HideWebView(webViewLocal); 
                        tutorialControl.InstatiateTutorialWelcome(tutorialItems.welcome); //mensaje de bienvenida    
                        break;
                    case "close":
                        Debug.Log("Ingrese a Close");
                        HideWebView(webViewLocal);
                        break;
                    case "closetutorial":
                        //Debug.Log("se va a cerrar desde el webview con tutorial");
                        //hideWebView2(webView3, message.Args["tutorialwatch"]);
                        break;
                    case "delete-cart":

                        Debug.Log("Ingrese a Delete Cart");
                        int id = int.Parse(message.Args["id"]);
                        var indicateFlag = message.Args["flag"];
                        Debug.Log("ID  recibido:" + message.Args["id"]);
                        Debug.Log("Flag recibido" + indicateFlag);
                        if (int.Parse(indicateFlag) == 0)
                        {
                            Debug.Log("Carrito vacio");
                            CartController.sharedInstance.RecalculatePhysicDimensionsTotales(id);
                            CartController.sharedInstance.DeleteCart(id);
                            HideWebView(webViewLocal);
                        }
                        else
                        {
                            CartController.sharedInstance.RecalculatePhysicDimensionsTotales(id);
                            CartController.sharedInstance.DeleteCart(id);
                            webViewLocal.EvaluateJavaScript("location.reload();", (payload) => {
                                if (payload.resultCode.Equals("0"))
                                {
                                    Debug.Log("Recarga exitosa.");
                                }
                                else
                                {
                                    Debug.LogError("Error al recargar.");
                                }
                            });
                        }
                        break;
                    case "cerrar-session":
                        //Usuario.sharedInstance.LogOut();
                        HideWebView(webViewLocal);
                        break;
                    case "editAccount":
                        //usuario = Usuario.sharedInstance;
                        //usuario.Login(
                        //    message.Args["correo"],
                        //    message.Args["userName"],
                        //    message.Args["nombreCompleto"],
                        //    usuario.PhoneNumber,
                        //    message.Args["password"]
                        //);
                        //loadMenu.updateHeader();
                        break;
                    case "open-profile":
                        HideWebView(webViewLocal);
                        //Usuario.sharedInstance.OpenProfile();
                        break;
                    case "open-cart":
                        HideWebView(webViewLocal);
                        CartController.sharedInstance.OpenCart();
                        break;

                    case "only-buy":
                        HideWebView(webViewLocal);
                        CartController.sharedInstance.OnlyBuy(message.Args["idProBodPre"], message.Args["idCaracteristica"]);
                        break;
                    case "location":
                        Ubication.sharedInstance.UpdateLocation(
                            message.Args["postal_code"]
                        );
                        //Debug.Log("me acaban de pasar el codigo postal");
                        HideWebView(webViewLocal);
                        break;
                    case "view-in-camera":
                        HideWebView(webViewLocal);
                        //TestDownloadModel.sharedInstance.startDownload(queriesManager.getProducto(int.Parse(message.Args["idProBodPre"])), int.Parse(message.Args["idCaracteristica"]));
                        break;
                    case "add-cart":
                        CartController.sharedInstance.AddCart(message.Args["idProBodPre"], message.Args["idCaracteristica"]);
                        //FindObjectOfType<VibrationManager>().PlayVibration("selection");
                        break;
                    case "clear-cart":
                        CartController.sharedInstance.ClearCart();
                        break;
                    case "increment-cantidad-cart":
                        CartController.sharedInstance.ChangeAmountByProduct(
                            message.Args["idProBodPre"],
                            message.Args["idCaracteristica"],
                            message.Args["cantidad"]);
                        break;
                    case "decrement-cantidad-cart":
                        CartController.sharedInstance.ChangeAmountByProduct(
                            message.Args["idProBodPre"],
                            message.Args["idCaracteristica"],
                            message.Args["cantidad"]);
                        break;
                    case "change-caracteristica":
                        CartController.sharedInstance.ChangeColor(
                            message.Args["idProBodPre"],
                            message.Args["idCaracteristica"]);
                        break;
                    case "change-view-menu":
                        ConfigurationController.sharedInstance.ChangeConfigurationWebView(int.Parse(message.Args["value"]));
                        break;
                    case "change-config-session":
                        ConfigurationController.sharedInstance.ChangeConfigurationSession(int.Parse(message.Args["value"]));
                        break;
                    case "change-config-cp":
                        ConfigurationController.sharedInstance.ChangeConfigurationPostalCode(int.Parse(message.Args["value"]));
                        break;
                    case "redirect-payment":
                        string param = "id=" + message.Args["id"] + "&use_mode=" + message.Args["use_mode"];
                        string url_payment = config.sharedInstance.urlArviSpaceForms("payment", config.localFiles.yes);
                        ShowWebView(webViewLocal, url_payment, 1, param);
                        break;
                    case "agreements":
                        Debug.Log("Ingrese a Agreements");
                        PlayerPrefs.SetString("agreementsAccepted", "true");
                        Debug.Log("Valor del playerpref de los terminos "+ PlayerPrefs.GetString("agreementsAccepted"));
                        HideWebView(webViewLocal);
                        break;
                }
            };
        }
        else
        {
            Debug.LogError("UniWebView es nulo, no se puede inicializar");
        }
        webViewLocal.OnPageFinished += (view, statusCode, url) =>
        {
            Debug.Log("Pagina cargada con status: " + statusCode);
            onPageFinished();
            if (url.Contains("ubication"))
            {
                //Debug.Log("si viene de ubication");
                TUbication objUbication = new TUbication();
                objUbication.cp = Ubication.sharedInstance.GetPostalCode();
                InitializeForm(webViewLocal, JsonUtility.ToJson(objUbication), 0);
            }
            else if (url.Contains("cart"))
            {
            //    //Debug.Log("si viene de cart");
            //    //Usuario usuario = Usuario.sharedInstance;
            //    CartObject cartObject = new CartObject();
            //    cartObject.nombre = usuario.FullName;
            //    cartObject.correo = usuario.Email;
            //    cartObject.telefono = usuario.PhoneNumber;
            //    cartObject.postal_code = Ubication.sharedInstance.GetPostalCode();
            //    cartObject._cartCollection = CartController.sharedInstance._toViewCartCollection;
            //    cartObject.redirectPayment = config.sharedInstance.urlArviSpaceForms("cart");

            //    InitializeForm(webViewLocal, JsonUtility.ToJson(cartObject), 0);
            }
            else if (url.Contains("store"))
            {
                //Debug.Log("si viene de store");
                TUbication objUbication = new TUbication();
                objUbication.cp = Ubication.sharedInstance.GetPostalCode();
                InitializeForm(webViewLocal, JsonUtility.ToJson(objUbication), 0);
            }
            else if (url.Contains("account"))
            {
                ////Debug.Log("si viene de la cuenta account");
                //Account account = new Account();
                //Usuario usuario = Usuario.sharedInstance;
                //account.nombre = usuario.FullName;
                //account.correo = usuario.Email;
                //account.telefono = usuario.PhoneNumber;
                //account.logged = usuario.CheckSesion();
                //account.postal_code = Ubication.sharedInstance.GetPostalCode();
                //account.configuration = FindObjectOfType<ConfigurationController>().configuration;
                //InitializeForm(webViewLocal, JsonUtility.ToJson(account), 0);

            }
            else if (url.Contains("map"))
            {
                //Debug.Log("si viene de map con coordenadas: " + FindObjectOfType<Geolocation>().latlng + " --- " + Geolocation.sharedInstance.latlng + "latt_ " + Input.location.lastData.latitude + " log: " + Input.location.lastData.longitude);
                InitializeForm(webViewLocal, JsonUtility.ToJson(FindObjectOfType<Geolocation>().GetLating()), 2);
            }
            else if (url.Contains("login"))
            {
                //Debug.Log("si viene de login");
                InitializeForm(webViewLocal, "{}", 0);
            }
            else if (url.Contains("register"))
            {
                //Debug.Log("si viene de register");
                InitializeForm(webViewLocal, "{}", 0);
            }

            else if (url.Contains("tutorial"))
            {
                //Debug.Log("si viene del tutotial");
                InitializeForm(webViewLocal, JsonUtility.ToJson(FindObjectOfType<Geolocation>().GetLating()), 3);
            }

            else if (url.Contains("tutorial2"))
            {
                //Debug.Log("si viene del tutotial");
                InitializeForm(webViewLocal, JsonUtility.ToJson(FindObjectOfType<Geolocation>().GetLating()), 3);
            }
            webViewLocal.SetContentInsetAdjustmentBehavior(UniWebViewContentInsetAdjustmentBehavior.Always);
            Debug.Log("Se muestra el webview");
            webViewLocal.Show();

        };
    }

    /// <summary>
    /// Esta funcion se implementa para ejecutar VUE JS con nueva actualización de Arvispace
    /// </summary>
    /// <param name="web3"></param>
    /// <param name="data"></param>
    /// <param name="opp"></param>
    public void InitializeForm(UniWebView web3, string data, int opp)
    {
        Debug.Log("initializeForm llamado con opp: " + opp + ", data: " + data);
        //opp  => mapp== 2

        //Debug.Log("inicializa el form");
        string useMode = config.sharedInstance.mUseMode.ToString();
        int idApp = (int)config.sharedInstance.app;
        //Debug.Log("va a mandar los datos al init con: " + useMode + " --- " + idApp + " --- " + data);

        if (opp == 2)//llama el init del mapa
        {
            /*
            string tutorial0 = PlayerPrefs.GetString("_tutorial_enable", "");
            int optutorial = 0;

            if (tutorial0 == "0") //es la primera vez que pasa
            {
                PlayerPrefs.SetString("_tutorial_enable", "1");
                // optutorial = 1;//enseña el tutorial
                optutorial = 0;
            }
            else //ha pasado mas de una vez
            {
                optutorial = 0; //no enseñ el tutorial
            }

            */
            //Debug.Log("si viene de map con coordenadas: " + FindObjectOfType<Geolocation>().latlng + " --- " + Geolocation.sharedInstance.latlng + "latt_ " + Input.location.lastData.latitude + " log: " + Input.location.lastData.longitude);


            web3.EvaluateJavaScript("content.init('" + useMode + "','" + idApp + "','" + data + "','" + 0 + "','" + Input.location.lastData.latitude + "','" + Input.location.lastData.longitude + "');", (payload) => {

                if (payload.resultCode.Equals("0"))
                {
                    //Debug.Log("no ocurrio un error.");
                    Debug.Log("JavaScript execution successful.");
                }
                else
                {
                    Debug.LogError("JavaScript execution error: " + payload.resultCode);
                    //Debug.Log("ocurrio un error.");
                }
            });
        }
        else
        {
            web3.EvaluateJavaScript("content.init('" + useMode + "','" + idApp + "','" + data + "');", (payload) => {

                if (payload.resultCode.Equals("0"))
                {
                    //Debug.Log("no ocurrio un error.");
                }
                else
                {
                    //Debug.Log("ocurrio un error.");
                }
            });

        }


        //Debug.Log("pudo ejecutar el init");
    }
    #endregion

    #region Public Methods

    /// <summary>
    /// Habilita el proceso de carga de la vista web
    /// </summary>
    /// <param name="webView1"></param>
    /// <param name="url"></param>
    /// <param name="opSize"></param>
    /// <param name="param"></param>
    public void ShowWebView(UniWebView webView1, string url, int opSize, string param = "")
    {
        Screen.orientation = ScreenOrientation.Portrait; //cambiar a Portrait
     
        if (opSize == 1)//si opsize es igual a 1 no es tutorial
        {
            loading.Open();
        }

        isShown = true;

        //Debug.Log("Ahora si se va a abrir el mapa");
        active = true;
        if (param != "")
        {
            url += "?" + param;
        }
        Debug.Log("URL to load: " + url);
        Debug.Log("Params: " + param);
        InitWebView3(webView1, url, opSize, () =>
        {
            this.webViewLocal.UpdateFrame();
        });
    }
    #endregion

}

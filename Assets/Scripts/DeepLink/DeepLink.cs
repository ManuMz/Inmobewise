using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using static NonConvexMeshCollider;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks.CompilerServices;
//using UnityEditor.Experimental.GraphView;
//using static UnityEditor.Progress;

public class DeepLink : MonoBehaviour
{
    #region Variables privadas serializadas
    [Header("Variables de visualizacion")]
    [SerializeField]
    private string id_inmueble;// ID Inmueble
    [SerializeField]
    private string nameBuilding="";// Nombre del inmueble
    [SerializeField]
    private string buildingCodeFound; // Código del inmueble
    [SerializeField]
    private GameObject prefabDownload;// Prefab referente a la descarga 
    [SerializeField]
    private DownloadComponentController m_downLoadComponentController;// controlador de descargas 
    [SerializeField]
    private RelocatePlayer relocatePlayer;//Referencia a la clase RelocatePlayer
    [SerializeField]
    private GameObject fatherBuildings;// objeto FatherHouses, donde se instanciará cada inmueble 
    [SerializeField]
    private GameObject player;// objeto player
    [SerializeField] private GameObject placement;
    [SerializeField] private LoadingController loadingController;
    [SerializeField] private QueriesController m_queriesController;
    [SerializeField] private TutorialControl tutorialControl;
    [SerializeField] private WithoutBuildingController m_withoutBuildingController;
    [SerializeField] private SaveComponentsController m_saveComponentsController;
    #endregion

    #region Variables privadas
    private GameObject productsAR;
    private bool isDownloading = false;  // proceso de descarga
    private GameObject downloadIndicator;   // indicador de descarga
    private GameObject downloadedObj; //Objeto descargado
    private bool deepLinkHandled = false;
    private Animator intro;
    private float time = 0;

    #endregion

    public string NameBuilding
    {
        get { return nameBuilding; } set { nameBuilding = value; } 
    }

    //void Start()
    //{
    //    // Registrar el evento para manejar deep links en la primera ejecucion
    //    Application.deepLinkActivated += OnDeepLinkActivated;

    //    if (Application.isMobilePlatform && !isDownloading)// si la app esta en una plataforma movil y no se ha descargado algun modelo, intenta obtener un enlace
    //    {                                              // true 
    //        string uri = GetDeepLink();// obtiene el enlace
    //        if (!string.IsNullOrEmpty(uri))
    //        {
    //            HandleDeepLink(uri);   // maneja el enlace si existe  
    //        }
    //    }
    //}

    void Start()
    {
        Application.deepLinkActivated += OnDeepLinkActivated;

        if (Application.isMobilePlatform)
        {
            string uri = GetDeepLink();

            if (!string.IsNullOrEmpty(uri))
            {
                Debug.Log("Enlace profundo activado en Start: " + uri);
                HandleDeepLink(uri);
                deepLinkHandled = true;
            }
            else
            {
                // Iniciar corrutina para verificar el enlace profundo
                StartCoroutine(CheckDeepLinkCoroutine());
            }
        }      

        //Obtener el intro
        foreach (var item in config.sharedInstance.GetIntros())
        {
            if (item.gameObject.activeSelf)
            {
                intro = item.GetComponent<Animator>();
            }
        }
    }
    private void Update()
    {
        if (intro.runtimeAnimatorController.animationClips[0].length <= time)
        {
            if (ShouldDownloadAsset())
            {
                if (!isDownloading)
                {
                    Debug.Log("Listo para descargar despues de la intro.");
                    isDownloading = true;
                }
            }
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    IEnumerator CheckDeepLinkCoroutine()
    {
        while (!deepLinkHandled)
        {
            string uri = GetDeepLink();

            if (!string.IsNullOrEmpty(uri))
            {
                Debug.Log("Enlace profundo recibido en corrutina: " + uri);
                HandleDeepLink(uri);
                deepLinkHandled = true;
                yield break;
            }

            // Esperar un segundo antes de la próxima verificación
            yield return new WaitForSeconds(1f);
        }
    }

    private bool ShouldDownloadAsset()
    {
        return deepLinkHandled && Application.isMobilePlatform &&
               TerminosController.IsAgreementAccepted() &&
               Ubication.IsLocationUpdated();
    }

    /// <summary>
    /// // Método para manejar enlaces profundos activados
    /// </summary>
    /// <param name="url"></param>
    private void OnDeepLinkActivated(string uri)
    {
        Debug.Log("Enlace profundo activado en OnDeepLinkActivated: " + uri);

        // Procesar el enlace profundo recibido
        if (!string.IsNullOrEmpty(uri))
        {
            HandleDeepLink(uri);
            deepLinkHandled = true;
        }
    }

    private void OnDestroy()
    {
        Application.deepLinkActivated -= OnDeepLinkActivated;
    }
    #region Private Methods 
    /// <summary>
    /// este metodo maneja el enlace profundo que se recibe y extrae el id_inmueble
    /// </summary>
    /// <param name="uri"></param>
    private void HandleDeepLink(string uri)
    {
        Debug.Log("La url obtenida en HandleDeepLink es: " + uri);
        string[] urlParts = uri.Split('/'); //Se recibe el URL completo y lo divide

        foreach (string part in urlParts)   //verifica si hay un ID de inmueble en el enlace
        {
            if (part.StartsWith("Codigo_Inmueble="))
            {
                int found = part.IndexOf("=");
                string buildingCodeTaked = part.Substring(found + 1);
                Debug.Log("Codigo del inmueble recibido: " + buildingCodeTaked);

                if (buildingCodeTaked != null)
                {
                    m_queriesController.GetBuildingIdByBuildingCode(buildingCodeTaked);
                    int buildingId = int.Parse(m_queriesController.GetBuildingId());
                    m_saveComponentsController.StartToLoadDefaultData(buildingId,buildingCodeTaked);

                    //StartDownloadBuildingAssetBundleByBuildingCode(buildingCodeTaked);
                    return;
                }
                else
                {
                    Debug.LogError("Error, el codigo del inmueble recibido es nulo");
                }
            }
            else if (part.StartsWith("id_inmueble="))
            {
                string idInmuebleStr = part.Substring("id_inmueble=".Length);
                id_inmueble = idInmuebleStr;
                int idInmueble;

                if (int.TryParse(idInmuebleStr, out idInmueble))
                {
                    Debug.Log("ID del inmueble recibido:" + idInmueble);
                    StartDownloadBuildingAssetBundleByBuildingId(idInmueble.ToString());
                    return;
                }
                else
                {
                    Debug.LogError("Error al convertir ID del inmueble a entero:" + idInmuebleStr);
                }
            }
        }
    }

    /// <summary>
    /// este metodo marca que se ha iniciado una descarga
    /// </summary>
    /// <param name="buildingAtribute"></param>
    private void StartDownloadBuildingAssetBundleByBuildingId(string buildingId)
    {
        //isDownloading = true; // Marca que se ha iniciado una descarga
        _ = DownloadBuildingAssetBundleByBuildingId(buildingId);  // Realiza la descarga
    }

    private void StartDownloadBuildingAssetBundleByBuildingCode(string buildingCode)
    {
        //isDownloading = true; // Marca que se ha iniciado una descarga
        _ = DownloadBuildingAssetBundleByBuildingcode(buildingCode);  // Realiza la descarga
    }

    /// <summary>
    ///Actualiza el indicador de progreso de la descarga
    /// </summary>
    /// <param name="currentProgress"></param>
    private void Progress(float currentProgress)
    {
        float percentage = currentProgress * 100;//saca un porcentaje de lo que lleva de descarga
        float rounded = (float)(Math.Round((double)percentage, 0));//redondeamos a dos digitos
        if (percentage == 100)
        {
            //Cierre del canvas de descarga 
            loadingController.CloseAssetBundle();
        }
        downloadIndicator.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = currentProgress;
        downloadIndicator.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = rounded.ToString() + " %";
    }

    /// <summary>
    /// este metodo gestiona la descarga de los modelos del inmueble
    /// </summary>
    /// <param name="idInmueble"></param>
    /// <returns></returns>
    private async UniTaskVoid DownloadBuildingAssetBundleByBuildingId(string buildingId)
    {
        UIController.Instance.HideMenuBuildings(); //Oculta el menú de inmuebles
        productsAR = player.GetComponent<TouchController>().GetFatherProducts();// Extrae el objeto producsAr y lo instancia en este metodo para su uso
        relocatePlayer.GetComponent<RelocatePlayer>().PosInitial(); //Colocar al usuario / player en la posición por default

        if (GetBuildingDataByBuildingID(buildingId).Equals(true))//Verificar si el inmueble existe y obtener sus datos a partir del ID 
        {
            m_withoutBuildingController.CloseWithoutBuildingCanvas(); //Desactivar canvas que indica inmueble no registrado /  no disponible
            string path = "";
            string platform = GetPlatform();

            _ = UrlArvispaceAssetsfromBuildings(buildingCodeFound, platform, (x) => path = x); // Realiza la solicitud para obtener la URL del assetBundle
            downloadIndicator = InstantiateInPlacement(prefabDownload);  // Muestra un indicador de descarga (puedes personalizar esto seg�n tus necesidades)
            loadingController.OpenAssetBundle();

            try
            {
                Debug.Log("valor de path:" + path);
                AssetBundle bundle = await Downloads.DownloadAssetBundle(path, (value) => Progress(value), () => Destroy(downloadIndicator)); // Descarga el AssetBundle
                Debug.Log("Valor de AssetBundle" + bundle);
                if (bundle != null)
                {
                    MenuBuildings.Clear(fatherBuildings);//Limpia Padre de los inmuebles
                    MenuBuildings.Clear(productsAR);// Limpia Padre de los productos

                    GameObject obj = bundle.LoadAsset<GameObject>(buildingCodeFound);// Carga el modelo desde el AssetBundle
                    bundle.Unload(false);
                    downloadedObj = Instantiate(obj, fatherBuildings.transform);
                    MessagesControllers.sharedInstance.CreateNotification("Confirmation", "Inmueble descargado", 3f, MessagesControllers.iconsNotifications.success);
                    TutorialControl.sharedInstance.InstatiateTutorialWelcome(tutorialItems.welcome);//Tutorial de Bienvenida
                    TutorialControl.sharedInstance.Panel().transform.Find("welcome(Clone)/Contorno/Panel/message").GetComponent<Text>().text = NameBuilding;
                 
                }
                else // AssetBundle nulo -> no se cuenta con modelo 3D del inmueble
                {
                    MenuBuildings.Clear(productsAR);// Limpia Padre de los productos
                    EmpyPlacement();
                    MessagesControllers.sharedInstance.CreateNotification("Error", "Lo sentimos, no contamos con el modelo 3D del inmueble", 4f, MessagesControllers.iconsNotifications.error);
                    if (tutorialControl)
                    {
                        if (tutorialControl.IsPlaying()) //obtenci�n de la variable que indica que el tutorial esta en ejecuci�n 
                        {
                            FindObjectOfType<TutorialItem>().toDestroy = true; //Destrucci�n del tutorialItem
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MenuBuildings.Clear(productsAR);// Limpia Padre de los productos
                Debug.LogError("Error en el proceso de descarga del AssetBundle del inmueble:" + e.Message);
                MessagesControllers.sharedInstance.CreateNotification("Error", "Falla en la descarga del inmueble", 4f, MessagesControllers.iconsNotifications.error);
            }
            //finally
            //{
            //    isDownloading = false; // restablece la variable.
            //}
        }
        else {

            MenuBuildings.Clear(productsAR);// Limpia Padre de los productos
            Debug.LogError("Error, el inmueble no cuenta con ID, no está registrado en la BD");
            //MessagesControllers.sharedInstance.CreateNotification("Error", "El inmueble no se encuentra disponible", 3f, MessagesControllers.iconsNotifications.error);
            m_withoutBuildingController.OpenWithoutBuildingCanvas();//Canvas que indica inmueble no registrado / no disponible 
        }
    }

   
    private async UniTaskVoid DownloadBuildingAssetBundleByBuildingcode(string buildingCode)
    {
        UIController.Instance.HideMenuBuildings(); //Oculta el menú de inmuebles
        productsAR = player.GetComponent<TouchController>().GetFatherProducts();// Extrae el objeto producsAr y lo instancia en este metodo para su uso
        relocatePlayer.GetComponent<RelocatePlayer>().PosInitial(); //Colocar al usuario / player en la posición por default

        if (GetBuildingDataByBuildingCode(buildingCode).Equals(true))//Verificar si el inmueble existe y obtener sus datos
        {
            m_withoutBuildingController.CloseWithoutBuildingCanvas(); //Desactivar canvas que indica inmueble no registrado /  no disponible
            string path = "";
            string platform = GetPlatform();

            _ = UrlArvispaceAssetsfromBuildings(buildingCode, platform, (x) => path = x); // Realiza la solicitud para obtener la URL del assetBundle
            downloadIndicator = InstantiateInPlacement(prefabDownload);  // Muestra un indicador de descarga (puedes personalizar esto segun tus necesidades)
            loadingController.OpenAssetBundle();

            try
            {
                Debug.Log("valor de path:" + path);
                AssetBundle bundle = await Downloads.DownloadAssetBundle(path, (value) => Progress(value), () => Destroy(downloadIndicator)); // Descarga el AssetBundle
                Debug.Log("Valor de AssetBundle" + bundle);

                if (bundle != null)
                {
                    MenuBuildings.Clear(fatherBuildings);//Limpia Padre de los inmuebles
                    MenuBuildings.Clear(productsAR);// Limpia Padre de los productos

                    GameObject obj = bundle.LoadAsset<GameObject>(buildingCode);// Carga el modelo desde el AssetBundle
                    bundle.Unload(false);
                    downloadedObj = Instantiate(obj, fatherBuildings.transform);
                    MessagesControllers.sharedInstance.CreateNotification("Confirmation", "Inmueble descargado", 3f, MessagesControllers.iconsNotifications.success);
                    TutorialControl.sharedInstance.InstatiateTutorialWelcome(tutorialItems.welcome);//Tutorial de Bienvenida
                    TutorialControl.sharedInstance.Panel().transform.Find("welcome(Clone)/Contorno/Panel/message").GetComponent<Text>().text = NameBuilding;

                }
                else // AssetBundle nulo -> no se cuenta con modelo 3D del inmueble
                {
                    MenuBuildings.Clear(productsAR);// Limpia Padre de los productos
                    EmpyPlacement();
                    MessagesControllers.sharedInstance.CreateNotification("Error", "Lo sentimos, no contamos con el modelo 3D del inmueble", 4f, MessagesControllers.iconsNotifications.error);

                    if (tutorialControl)
                    {
                        if (tutorialControl.IsPlaying()) //obtenci�n de la variable que indica que el tutorial esta en ejecuci�n 
                        {
                            FindObjectOfType<TutorialItem>().toDestroy = true; //Destruccion del tutorialItem
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                MenuBuildings.Clear(productsAR);// Limpia Padre de los productos
                Debug.LogError("Error en el proceso de descarga del AssetBundle del inmueble:" + ex.Message);
                MessagesControllers.sharedInstance.CreateNotification("Error", "Falla en la descarga del inmueble", 4f, MessagesControllers.iconsNotifications.error);
            }
            //finally
            //{
            //    isDownloading = false; // restablece la variable.
            //}
        }
        else
        {
            MenuBuildings.Clear(productsAR);// Limpia Padre de los productos
            Debug.LogError("Error, el inmueble no cuenta con ID, no está registrado en la BD");
            //MessagesControllers.sharedInstance.CreateNotification("Error", "El inmueble no se encuentra disponible", 3f, MessagesControllers.iconsNotifications.error);
            m_withoutBuildingController.OpenWithoutBuildingCanvas();//Canvas que indica inmueble no registrado / no disponible 
        }

    }


    /// <summary>
    /// busca un inmueble especifico por su ID en la lista de componentsInmueble
    /// </summary>
    /// <param name="buildingID"></param>
    /// <returns></returns>
    private bool GetBuildingDataByBuildingID(string buildingID)
    {
        Inmuebles building = new(); // objeto building / inmueble de la clase Inmuebles 

        if (m_downLoadComponentController.componentsBuildings.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.componentsBuildings)
            {
                foreach (var subCategory in category.Tipo_Inmueble)
                {
                    foreach (var inmueble in subCategory.Inmuebles)
                    {   // verifica si el ID del inmueble coincide con el buscado
                        if (inmueble.Id_Inmueble.Equals(buildingID))
                        {
                            building = inmueble; // Guarda el inmueble encontrado
                            building.tipo_Inmueble = subCategory; // guarda la subcategoria del inmueble
                            nameBuilding = inmueble.Nombre_Inmueble; // asigna el nombre del inmueble
                            buildingCodeFound = inmueble.Codigo_Inmueble; // asigna el codigo del inmueble

                            Debug.Log("EL ID DEL INMUEBLE ES: " + building.Id_Inmueble);
                            Debug.Log("EL CÓDIGO DEL INMUEBLE ES: " + buildingCodeFound);
                            return true;
                        }

                    }
                }
            }      
        }
        return false;// devuelve False si inmueble No es encontrado
    }
    private bool GetBuildingDataByBuildingCode(string buildingCode)
    {
        Inmuebles building = new(); // objeto building / inmueble de la clase Inmuebles 
        if (m_downLoadComponentController.componentsBuildings.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.componentsBuildings)
            {
                foreach (var subCategory in category.Tipo_Inmueble)
                {
                    foreach (var inmueble in subCategory.Inmuebles)
                    {   // verifica si el ID del inmueble coincide con el buscado
                        if (inmueble.Codigo_Inmueble.Equals(buildingCode))
                        {
                            building = inmueble; // Guarda el inmueble encontrado
                            building.tipo_Inmueble = subCategory; // guarda la subcategoria del inmueble
                            nameBuilding = inmueble.Nombre_Inmueble; // asigna el nombre del inmueble
                            buildingCodeFound = inmueble.Codigo_Inmueble; // asigna el codigo del inmueble

                            Debug.Log("EL ID DEL INMUEBLE ES: " + building.Id_Inmueble);
                            Debug.Log("EL CÓDIGO DEL INMUEBLE ES: " + buildingCodeFound);
                            return true;
                        }

                    }
                }
            }
        }
        return false;// devuelve False si inmueble No es encontrado
    }

    /// <summary>
    /// Este metodo construye la url para descargar un asset
    /// </summary>
    /// <param name="buildingCode"></param>
    /// <param name="platform"></param>
    /// <param name="pathResponse"></param>
    /// <returns></returns>
    private async UniTaskVoid UrlArvispaceAssetsfromBuildings(string buildingCode, string platform, Action<string> pathResponse)
    {
        Debug.Log("Ingrese a UrlArvispaceAssetsHouses");
        string path = Path.Combine("https://arvispace.com/InmobewiseApp/resources/assetbundlesfrombuildings/", platform, buildingCode); //Iniciando descarga de modelo
        pathResponse(path);//Ruta del assetbundle:
        Debug.Log("Path del Inmueble:" + path);
    }
    /// <summary>
    /// Obtiene y devuelve la plataforma en la que se ejecuta
    /// </summary>
    /// <returns></returns>
    private string GetPlatform()
    {
        RuntimePlatform platform = Application.platform;

        switch (platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return "windows";
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return "macos";
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            default:
                return "unknown";
        }
    }

    /// <summary>
    /// este metodo obtiene el enlace 
    /// </summary>
    /// <returns></returns>
    private string GetDeepLink()
    {

        if (Application.isMobilePlatform) // verifica en que se esta ejecutando la app
        {
            string deepLink = UnityEngine.Application.absoluteURL;// Obtener el enlace profundo de la aplicacion

            //para IOS
            if (deepLink.StartsWith("file://")) 
            {
                deepLink = deepLink.Replace("file://", "");
            }

            return deepLink; // devuelve el enlace procesado
        }
        return null;
    }

    private GameObject InstantiateInPlacement(GameObject toInstantiate)
    {
        EmpyPlacement();

        return Instantiate(toInstantiate, placement.transform);
    }

    private void EmpyPlacement()
    {
        for (int i = 0; i < placement.transform.childCount; i++)
        {
            Destroy(placement.transform.GetChild(i).gameObject);
        }
    }
    #endregion
}

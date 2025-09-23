using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;
using UnityEngine.Android;
using System.IO;
using static ObjectProperties;
//using Unity.Android.Gradle;
using UnityEditor;
using System.Data.Common;
using UnityEngine.Networking;
using static SaveComponentsController;
using System.Linq;

public class DownloadModel : MonoBehaviour
{
    #region Variables publicas
    public static DownloadModel sharedInstance;
    #endregion

    #region Variables privadas
    [SerializeField]
    private TouchController touchController;
    [SerializeField]
    private QueriesController queriesController;
    [SerializeField]
    private GameObject prefabDownload;
    [SerializeField]
    private TutorialControl tutorialControl;
    [SerializeField]
    private LoadingController loadingController;
    [Header("PADRE DONDE SE INSTANCIAN EL PREFAB DOWNLOAD")]
    [SerializeField] private GameObject placement;

    [SerializeField] private GameObject player; 

    /// <summary>
    /// Lista que guarda todas las descargas
    /// </summary>
    /// 
    [Header("Visualizacion de descargas")]
    [Tooltip("lista de descargas")]
    [SerializeField]
    private List<GameObject> downloadsProducts;

    [Tooltip("lista de modelos descargados")]
    [SerializeField]
    private List<GameObject> downloadsBuildings;

    [Tooltip("Indicador de descarga")]
    private GameObject downloadIndicator;

    [Tooltip("Inmueble descargado")]
    private GameObject downloadedBuilding;

    [SerializeField] private GameObject fatherBuildings;
    [SerializeField] private GameObject fatherProducts;
    [SerializeField] private SaveComponentsController saveComponentsController;
    
    private Hash128 assetBundleHash;
    private int idProbodPre;
    private int previousidProbodPre;
    private bool idProBodPreHasChanged = false;

  
    #endregion

    public int IdProBodPre
    {
        get { return idProbodPre; } 
        set
        {
            if (idProbodPre != value) // Compara si ha cambiado
            {
                previousidProbodPre = idProbodPre; // Guarda el valor anterior
                idProbodPre = value;
                idProBodPreHasChanged = true;
            }
            else
            {
                idProBodPreHasChanged = false;
            }
        }
    }

    public bool HasObjectChanged()
    {
        return idProBodPreHasChanged;
    }

    public GameObject GetFatherBuildings()
    {
        return fatherBuildings;
    }

    public GameObject GetFatherProducts()
    {
        return fatherProducts;
    }

    public Hash128 GetAssetBundleHash()
    {
        return assetBundleHash; 
    }
        
    public GameObject DownloadedBuilding
    {
        get { return downloadedBuilding; }
        set { downloadedBuilding = value; }
    }    
    public List<GameObject> DownloadedProducts
    {
        get
        {
            return downloadsProducts;   
        }
        set
        {
            downloadsProducts = value;
        }

    }
    private void Awake()
    {
        if (sharedInstance)
        {
            Destroy(this);
        }
        else
        {
            sharedInstance = this;
        }
    }

    private void Start()
    {
        // Limpiar la cach� al inicio
        ClearTempCache();
        downloadsBuildings = new List<GameObject>();// agrega lista de modelos descargados
        downloadsProducts = new List<GameObject>(); //agrega lista de productos descargados
             
    }

    #region Private Methods / Async Methods

    #region Cache Manager
    public void ClearTempCache()
    {
        if (Directory.Exists(Application.temporaryCachePath))
        {
            Directory.Delete(Application.temporaryCachePath, true);
            Debug.Log("Cache temporal eliminada.");
        }
    }

    public void DisplayCacheSize()
    {
        string tempCachePath = Application.temporaryCachePath;
        string persistencathPath = Application.persistentDataPath; 
        if (Directory.Exists(tempCachePath) && Directory.Exists(persistencathPath))
        {
            long cacheSize = GetDirectorySize(tempCachePath);
            long cachePersistentSize = GetDirectorySize(persistencathPath); 
            Debug.Log($"El caché temporal ocupa: {FormatBytes(cacheSize)}");
            Debug.Log($"El caché Persistente ocupa: {FormatBytes(cachePersistentSize)}");

        }
        else
        {
            Debug.Log("El directorio de caché temporal no existe.");
        }
    }
    private long GetDirectorySize(string path)
    {
        long size = 0;

        // Sumar el tamaño de los archivos en el directorio
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        foreach (FileInfo file in dirInfo.GetFiles())
        {
            size += file.Length;
        }

        // Sumar el tamaño de los archivos en subdirectorios
        foreach (DirectoryInfo dir in dirInfo.GetDirectories())
        {
            size += GetDirectorySize(dir.FullName);
        }

        return size;
    }
    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double len = bytes;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
    #endregion

    

    private void Progress(float currentProgress)
    {
        float percentage = currentProgress * 100;//saca un porcentaje de lo que lleva de descarga
        float rounded = (float)(Math.Round((double)percentage, 0));//redondeamos a dos digitos
        if (percentage == 100)
        {
            //Cierre del canvas de descarga 
            loadingController.CloseAssetBundle();
        }
        downloadIndicator.transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = currentProgress;
        downloadIndicator.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = rounded.ToString() + " %";
    }

    /// <summary>
    /// Valida si el producto ya fue descargado
    /// </summary>
    /// <param name="idProBodPre">String id del producto seleccionado</param>
    /// <returns>GameObject if found product else return null</returns>
    public GameObject ValidateIsDownloaded(string idProBodPre)
    {
        foreach (var item in downloadsProducts)
        {
            if (item.gameObject.name.Split('(')[0] == idProBodPre)
            {
                return item.gameObject;
            }

        }
        return null;
    }

    /// <summary>
    /// Instanciar producto seleccionado ya sea de porductos descargados previamente o que apenas se descargan
    /// </summary>
    /// <param name="product">Objeto seleccionado desde menu</param>
    /// <param name="idCharacteristic">INT id de caracteristica seleccionada</param>
    private void InstantiateProductDownloaded(Producto product, int idCharacteristic)
    {
        var idState = new AutoIncrement();

        ObjectProperties objectPropertiesTemp = touchController.p_objectSelected.GetComponent<ObjectProperties>();
        //para construir el objeto y poder ser usado en el metod0
        objectPropertiesTemp.producto = product;
        objectPropertiesTemp.idSelected = idCharacteristic;
        objectPropertiesTemp.idProBodPre = product.idProBodPre;

        objectPropertiesTemp.UniqueId= idState.GenerateId();
        
        touchController.p_objectSelected = Instantiate(touchController.p_objectSelected, new Vector3(0, -5f, 0), Quaternion.identity, touchController.GetFatherProducts().transform);
    }

    /// <summary>
    /// busqueda del modelo descargado (Productos)
    /// </summary>
    /// <param name="product"></param>
    /// <param name="idCharacteristic"></param>
    /// <returns></returns>
    private async UniTaskVoid DownloadAsset(Producto product, int idCharacteristic)
    {
        GameObject isDownloaded = ValidateIsDownloaded(product.idProBodPre.ToString());//Validac��n de que se encuentra descargado

        if (ContainerProducto.changeObj)
        {
            GameObject oldObj = touchController.p_objectSelected;
            Destroy(oldObj);
        }
        if (isDownloaded)
        {
            touchController.p_objectSelected = isDownloaded.gameObject;
            InstantiateProductDownloaded(product, idCharacteristic);
        }
        else
        {
            string path = "";
            print("Downloading...");
            await config.sharedInstance.urlArvispaceAssets(product.idProBodPre.ToString(), (x) => path = x);
            print("Path: "+ path);

            downloadIndicator = InstantiateInPlacement(prefabDownload);
            loadingController.OpenAssetBundle();

            string temp = path;

            AssetBundle bundle = await Downloads.DownloadAssetBundle(temp, (value) => Progress(value), () => Destroy(downloadIndicator));

            if (bundle != null) //El assetbundle existe, ha sido descargado
            {
                GameObject obj = bundle.LoadAsset<GameObject>(product.idProBodPre.ToString());//el modelo descargado se almacena en la variable obj---ME

                //Verificar si el AssetBundle cuenta con ObjectProperties
                ObjectProperties verificateObjectProperties = obj.GetComponent<ObjectProperties>();

                if (verificateObjectProperties != null)
                {
                    touchController.p_objectSelected = bundle.LoadAsset<GameObject>(product.idProBodPre.ToString());
                    downloadsProducts.Add(touchController.p_objectSelected); //el modelo descargado se agrega a la lista de modelos descargados
                    bundle.Unload(false);
                    print("Downloaded");
                    InstantiateProductDownloaded(product, idCharacteristic);
                    MessagesControllers.sharedInstance.CreateNotification("Confirmation", "Producto descargado", 3f, MessagesControllers.iconsNotifications.success);
                }
                else
                {
                    MessagesControllers.sharedInstance.CreateNotification("Error", "Error al obtener el producto", 3f, MessagesControllers.iconsNotifications.error);
                }
            }
            else //AssetBundle completamente nulo
            {
                EmpyPlacement();
                MessagesControllers.sharedInstance.CreateNotification("Error", "Lo sentimos, no contamos con el modelo 3D del producto", 5f, MessagesControllers.iconsNotifications.error);
                if (tutorialControl)
                {
                    if (tutorialControl.IsPlaying()) //obtenci�n de la variable que indica que el tutorial esta en ejecuci�n 
                    {
                        FindObjectOfType<TutorialItem>().toDestroy = true; //Destrucci�n del tutorialItem
                    }
                }
            }
        }
    }
    /// <summary>
    /// Descarga del modelo del inmueble a traves del boton Visualizar 
    /// </summary>
    /// <param name="building"></param>
    /// <returns></returns>
    private async UniTaskVoid DownloadAssetBuildings(Inmuebles building)
    {
        UIController.sharedInstance.HideMenuBuildings();//Oculta el Menu de los Inmuebles
        RelocatePlayer.sharedInstance.PosInitial();//Reestablece la posicion del jugador en la pos inicial

        string path = "";
        print("Downloading...");
        Debug.Log("EL ID  QUE RECIBI ES: " + building.Codigo_Inmueble);
        await config.sharedInstance.UrlArvispaceAssetsBuildings(building.Codigo_Inmueble.ToString(), (x) => path = x);

        print(path);
        downloadIndicator = InstantiateInPlacement(prefabDownload);
        loadingController.OpenAssetBundle();

        string temp = path;
        AssetBundle bundle = await Downloads.DownloadAssetBundle(temp, (value) => Progress(value), () => Destroy(downloadIndicator));//descarga del modelo

        if (bundle != null)
        {
            MenuBuildingsView.Clear(fatherBuildings);//Limpia Padre de los inmuebles
            MenuBuildingsView.Clear(fatherProducts);//Limpia Padre de los Productos

            GameObject obj = bundle.LoadAsset<GameObject>(building.Codigo_Inmueble.ToString());//el modelo descargado se almacena en la variable obj
            downloadsBuildings.Add(obj);//el modelo descargado se agrega a la lista de modelos descargados
            bundle.Unload(false);

            downloadedBuilding = Instantiate(obj, fatherBuildings.transform);//instancia el objeto
            print("Downloaded");
            SaveComponentsController.sharedInstance.GetBuildingCodeUseToSave(building.Codigo_Inmueble);
            MessagesControllers.sharedInstance.CreateNotification("Confirmation", "Inmueble descargado", 3f, MessagesControllers.iconsNotifications.success);

            TutorialControl.sharedInstance.InstatiateTutorialWelcome(tutorialItems.welcome);//TUTORIAL DE BIENVENIDA
            TutorialControl.sharedInstance.Panel().transform.Find("welcome(Clone)/Contorno/Panel/text_name_inmueble").GetComponent<Text>().text = building.Nombre_Inmueble;
        }
        else //Bundle nulo
        {
            MenuBuildingsView.Clear(fatherProducts);//Limpia Padre de los Productos
            EmpyPlacement();
            MessagesControllers.sharedInstance.CreateNotification("Error", "Lo sentimos, no contamos con el modelo 3D del inmueble", 7f, MessagesControllers.iconsNotifications.error);
            if (tutorialControl)
            {
                if (tutorialControl.IsPlaying()) //obtenci�n de la variable que indica que el tutorial esta en ejecuci�n 
                {
                    FindObjectOfType<TutorialItem>().toDestroy = true; //Destrucci�n del tutorialItem
                }
            }
        }
    }

    /// <summary>
    /// Descarga del modelo del inmueble a partir del codigo
    /// </summary>
    /// <param name="buildingCode"></param>
    /// <returns></returns>
    private async UniTaskVoid DownloadAssetBundleByBuildingCode(string buildingCode)
    {
        UIController.sharedInstance.HideMenuBuildings();//Oculta el Menu de los Inmuebles
        RelocatePlayer.sharedInstance.PosInitial();//Reestablece la posicion del jugador en la pos inicial

        string path = "";
        print("Downloading...");
        await config.sharedInstance.UrlArvispaceAssetsBuildings(buildingCode, (x) => path = x);
        print(path);
        
        downloadIndicator = InstantiateInPlacement(prefabDownload);//instancia el prefab que contiene la animaci�n / elementos de descarga en el GameObject downloadindicator
        loadingController.OpenAssetBundle();

        string temp = path;
        AssetBundle bundle = await Downloads.DownloadAssetBundle(temp, (value) => Progress(value), () => Destroy(downloadIndicator));//descarga del modelo

        if (bundle != null)
        {
            MenuBuildingsView.Clear(fatherBuildings);//Limpia Padre de los inmuebles
            MenuBuildingsView.Clear(fatherProducts);//Limpia Padre de los Productos

            GameObject obj = bundle.LoadAsset<GameObject>(buildingCode);//el modelo descargado se almacena en la variable obj
            downloadsBuildings.Add(obj);//el modelo descargado se agrega a la lista de modelos descargados
            bundle.Unload(false);

            downloadedBuilding = Instantiate(obj, fatherBuildings.transform);//instancia el objeto
            print("Downloaded");
            SaveComponentsController.sharedInstance.GetBuildingCodeUseToSave(buildingCode);
            MessagesControllers.sharedInstance.CreateNotification("Confirmation", "Inmueble descargado", 3f, MessagesControllers.iconsNotifications.success);

            TutorialControl.sharedInstance.InstatiateTutorialWelcome(tutorialItems.welcome);//TUTORIAL DE BIENVENIDA
            TutorialControl.sharedInstance.Panel().transform.Find("welcome(Clone)/Contorno/Panel/text_name_inmueble").GetComponent<Text>().text = queriesController.GetBuildingName();
        }
        else //Bundle nulo
        {
            MenuBuildingsView.Clear(fatherProducts);//Limpia Padre de los Productos
            EmpyPlacement();
            MessagesControllers.sharedInstance.CreateNotification("Error", "Lo sentimos, no contamos con el modelo 3D del inmueble", 7f, MessagesControllers.iconsNotifications.error);
            if (tutorialControl)
            {
                if (tutorialControl.IsPlaying()) //obtencion de la variable que indica que el tutorial esta en ejecuci�n 
                {
                    FindObjectOfType<TutorialItem>().toDestroy = true; //Destruccion del tutorialItem
                }
            }
        }
    }

    #endregion
    private GameObject InstantiateInPlacement(GameObject toInstantiate)
    {
        EmpyPlacement();

        return Instantiate(toInstantiate, placement.transform);
    }

    #region Public Methods

    /// Producto product es un tipo de referencia, por lo que pasa el acceso de la variable al metodo 
    /// </summary>
    /// <param name="product"></param>
    /// <param name="idCharacteristic"></param>
    public void StartDownload(Producto product, int idCharacteristic)
    {
        _ = DownloadAsset(product, idCharacteristic);
    }
    public void StartDownloadBuildings(Inmuebles buildings)
    {
        _ = DownloadAssetBuildings(buildings);
    }

    public void StartDownloadBuildingCode(string code)
    {
        _ = DownloadAssetBundleByBuildingCode(code);
    }

    public void EmpyPlacement()
    {
        for (int i = 0; i < placement.transform.childCount; i++)
        {
            Destroy(placement.transform.GetChild(i).gameObject);
        }
    }
    #endregion

    /*[System.Serializable]
    public class AssetBundleBuilder 
    {
        [MenuItem("Assets/Build AssetBundles")]
        public static void BuildAllAssetBundles()
        {
            string path = "Assets/AssetBundles";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            Debug.Log("AssetBundles generados en: " + path);
        }

    }*/  
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;
using static SaveComponentsController;

[Serializable]
public class StatusDefinition {
    public string status;
}

public class DownloadComponentController : MonoBehaviour
{
    #region Variables Publicas
    /// <summary>
    /// Lista de las categorias de los productos
    /// </summary>
    public List<Categoria> components;
    /// <summary>
    /// Lista de las categorias de los inmuebles
    /// </summary>
    public List<CategoriaInmueble> componentsBuildings;
    #endregion

    #region Variables Privadas
    [SerializeField]
    private config m_config;
    [SerializeField]
    Menu m_menuManager;
    //[SerializeField]
    //MenuBuildings m_menuManagerInmueble;
    [SerializeField]
    MenuBuildingsView m_menuManagerBuldingsView; 
    CancellationTokenSource cts;
    [SerializeField]
    private int limitTry = 1;
    [SerializeField]
    private int countToTry;
    [SerializeField] private WithoutProductsController m_withoutProductsController;
    #endregion

    private bool existProducts; 
    public bool GetExistProducts()
    {
        return existProducts;
    }

    private void Awake()
    {
        cts = new CancellationTokenSource();
    }

    void Start()
    {
        StartDownload();
    }

    public void StartDownload() {
        /*if (components != 0)
        { 
        }*/
        _ = DownloadProductsBuildings(); // Descarga del servicio de las casas
        _ = DownloadProducts(); //Descarga del servicio de los productos
    }

    #region OBTENCION DEL SERVICIO DE INMUBLES
    /// <summary>
    /// Descarga de Deserializacion de los inmuebles(SERVICIO DE LOS INMUEBLES)
    /// </summary>
    /// <returns></returns>
    async UniTaskVoid DownloadProductsBuildings()
    {
        componentsBuildings = new List<CategoriaInmueble>();
        WWWForm form = new WWWForm();

        //string service = "https://inmobiliaria.arvispace.com/servicios/Catalogo_Inmuebles.php";//Servicio older 
        string service = "https://inmobiliaria.arvispace.com/InmobeWiseExample/Services/rute.php?petition=GetInmueblesModelados";

        switch (config.sharedInstance.GetmUseMode())
        {
            case config.useMode.testing:
                //service = "https://inmobiliaria.arvispace.com/servicios/Catalogo_Inmuebles.php";;//Servicio older
                service = "https://inmobiliaria.arvispace.com/InmobeWiseExample/Services/rute.php?petition=GetInmueblesModelados";
                break;
            case config.useMode.production:
                //service = "https://inmobiliaria.arvispace.com/servicios/Catalogo_Inmuebles.php";;//Servicio older
                service = "https://inmobiliaria.arvispace.com/InmobeWiseExample/Services/rute.php?petition=GetInmueblesModelados";
                break;
        }
        var data = (await UnityWebRequest.Get(service).SendWebRequest()).downloadHandler.text; //espera la respuesta del serv

        componentsBuildings = DeserializeJsonBuilding(data);

        //m_menuManagerInmueble.InstatiateCategoriesBuildings();//Instancia las categorias de los inmuebles en el catalogo de los inmubles.
        m_menuManagerBuldingsView.InstatiateProductsBuildinigsView();//Instancia todos los inmuebles inmuebles en el catalogo de los inmubles.
    }

    /// <summary>
    /// Deserializa el Json(el servicio de inmobiliaria),(data).
    /// </summary>
    List<CategoriaInmueble> DeserializeJsonBuilding(string json)
    {
        List<CategoriaInmueble> listBuildings = new List<CategoriaInmueble>();
        try
        {
            listBuildings = JsonHelper.getJsonArray<CategoriaInmueble>(json).ToList();
            componentsBuildings = listBuildings.ToList();
        }
        catch (Exception)
        {
            //Debug.Log("No estoy deserializando el servicio de inmobewise.");
            Retry();
        }
        return listBuildings.ToList();
    }
    #endregion

    #region OBTENCION DEL SERVICIO DE PRODUCTOS
    /// <summary>
    /// Descarga de Deserializacion de los productos(SERVICIO DE LOS PRODUCTOS)
    /// </summary>
    /// <returns></returns>
    async UniTaskVoid DownloadProducts()
    {
        if (!m_config)
            await UniTask.WaitUntil(() => m_config != null);

        components = new List<Categoria>();//lista de las categorias de los productos
        WWWForm form = new WWWForm();

        string service = "";
        switch (config.sharedInstance.GetApp())
        {
            case config.apps.Inmobewise:
                if (config.sharedInstance.GetmUseMode() == config.useMode.production)
                {
                    var status = (await UnityWebRequest.Get("https://sandbox.arvispace.com/status.json").SendWebRequest()).downloadHandler.text;


                    StatusDefinition definition;
                    definition = JsonConvert.DeserializeObject<StatusDefinition>(status);
                    //service = definition.status == "production" ? "https://arvispace.com/serviciosASAR/ProductosAsar.php?cp=7200" : "https://arvispace.com/serviciosASARAmbientePruebas/allproducts.php?cp=7200";
                    //service = definition.status == "production" ? "https://arvispace.com/serviciosASAR/ProductosAsar.php" : "https://arvispace.com/serviciosASARAmbientePruebas/allproducts.php";

                    //service = definition.status == "production" ? "https://arvispace.com/serviciosASAR/ProductosAsarInmobewise.php?cp=72100" : "https://arvispace.com/serviciosASARAmbientePruebas/allproducts.php?cp=72100";
                    //service = definition.status == "production" ? "https://arvispace.com/serviciosASAR/ProductosAsarInmobewise.php?cp=72014" : "https://arvispace.com/serviciosASARAmbientePruebas/allproducts.php?cp=72014";
                    //service = definition.status == "production" ? "https://arvispace.com/serviciosASAR/ProductosAsarInmobewise.php?cp=90500" : "https://arvispace.com/serviciosASARAmbientePruebas/allproducts.php?cp=90500";
                    service = definition.status == "production" ? "https://arvispace.com/serviciosASAR/ProductosAsarInmobewise.php" : "https://arvispace.com/serviciosASARAmbientePruebas/allproducts.php";
                    form.AddField("cp", Ubication.sharedInstance.GetPostalCode());

                    MessagesControllers.sharedInstance.CreateNotification("Confirmation", "El CP RECUPERADO: " + Ubication.sharedInstance.GetPostalCode(), 3f, MessagesControllers.iconsNotifications.success);
                }
                else
                {
                    //Debug.Log("testing selected");
                    service = m_config.urlArvispaceServices(config.sharedInstance.GetmUseMode() == config.useMode.production ? "ProductosAsar.php" : "allproducts.php");
                    form.AddField("cp", Ubication.sharedInstance.GetPostalCode());
                    //form.AddField("idEmpresaApp", (int)config.apps.Arvistyle);
                }
                break;
        }


        var data = (await UnityWebRequest.Post(service, form).SendWebRequest().WithCancellation(cts.Token)).downloadHandler.text;

        components = DeserializeJsonProducts(data);

        if (components.Count <= 0)
        {
            existProducts = false;
            //m_withoutProductsController.OpenWithoutProductsCanvas();
        }
        else
        {
            existProducts = true;
            //MessagesControllers.sharedInstance.CreateNotification("Confirmation", "El CP RECUPERADO: " + Ubication.sharedInstance.GetPostalCode(), 5f, MessagesControllers.iconsNotifications.success);
            //m_withoutProductsController.CloseWithoutProductsCanvas();
            m_menuManager.InstatiateCategories();//Instancia las categorias en el catalogo de productos
        }
        
        //m_menuManager.InstatiateCategories();//Instancia las categorias en el catalogo de productos
    }

    /// <summary>
    /// Deserializa el Json(el servicio),(data).
    /// </summary>
    List<Categoria> DeserializeJsonProducts(string json)
    {
        List<Categoria> listCategories = new List<Categoria>();
        try
        {
            listCategories = JsonHelper.getJsonArray<Categoria>(json).ToList();
            components = listCategories.ToList();
        }
        catch (Exception)
        {
            Retry();
        }
        return listCategories.ToList();
    }

    #endregion


    /// <summary>
    /// Intenta Descargar y Deserializar los componentes de los servicios
    /// </summary>
    private void Retry()
    {
        countToTry++;
        cts.Cancel();
        StartDownload();
    }

    #region Obtencion de inmuebles older 
    /*Obtencion de inmuebles Older
   public List<Inmuebles> inmobi;
   async UniTaskVoid getInmuebles()
   {
       inmobi = new List<Inmuebles>();
       WWWForm form = new WWWForm();

       //string url = "https://arvispace.com/serviciosASARPrueba/obtenerInmueblesAsar.php";
       string url = "https://inmobiliaria.arvispace.com/servicios/Catalogo_Inmuebles.php";

       switch (config.sharedInstance.m_useMode)
       {
           case config.useMode.testing:
               //url = "https://arvispace.com/serviciosASARPrueba/obtenerInmueblesAsar.php";
               url = "https://inmobiliaria.arvispace.com/servicios/Catalogo_Inmuebles.php";
               break;
           case config.useMode.production:
               //url = "https://arvispace.com/serviciosASARPrueba/obtenerInmueblesAsar.php";
               url = "https://inmobiliaria.arvispace.com/servicios/Catalogo_Inmuebles.php";
               break;
       }
       var data = (await UnityWebRequest.Get(url).SendWebRequest()).downloadHandler.text;

       inmobi = DeserializeJsonBuildings(data);

       //m_menuManager.instatiateHouses();
   }

   List<Inmuebles> DeserializeJsonBuildings(string json)
   {
       List<Inmuebles> listBuildings = new List<Inmuebles>();
       try
       {
           listBuildings = JsonHelper.getJsonArray<Inmuebles>(json).ToList();
           inmobi = listBuildings.ToList();
       }
       catch (Exception)
       {
           //retry();
       }
       return listBuildings.ToList();
   }
   */
    #endregion
}

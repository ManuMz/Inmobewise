using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Newtonsoft.Json;
using static SaveComponentsController;
using static UnityEngine.UI.GridLayoutGroup;
using System.Data.Common;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using NUnit.Framework.Constraints;
using JetBrains.Annotations;

public class SaveComponentsController : MonoBehaviour
{
    public static SaveComponentsController sharedInstance;

    [Header("SCROLL'S DE MENU INMUEBLES FAVORITOS")]
    [SerializeField] private ScrollMenuController m_scrollToFavoriteBuildingsView;

    [Header("CONTENEDORES INMUEBLES FAVORITOS")]
    [SerializeField] private GameObject contentFavoriteBuildingsView;
    [SerializeField] private GameObject prefabToFavoriteBuildingView;
    [SerializeField] private DownloadComponentController m_downloadComponentController;
    //[SerializeField] private Usuario m_Usuario;
    [SerializeField] private GameObject fatherBuildings;
    [SerializeField] private GameObject fatherProducts;
    [SerializeField] private DownloadModel m_downloadModel;
    [SerializeField] private TouchController m_touchController;
    [SerializeField] private LoadingController m_loadingController;
    [SerializeField] private GameObject m_placement;
    [SerializeField] private GameObject m_prefabDownload;
    [SerializeField] private GameObject m_prefabLoading;
    [SerializeField] private MessagesControllers m_MessagesController;
    [SerializeField] private UIController m_UIController;
    [SerializeField] private QueriesController m_QueriesController; 
    private string url = "https://arvispace.com/serviciosASAR/";
    private List<string> buildingPictures;
    private string buildingCodeUseToSave="";
    private List<InmuebleData> buildingsToLoad;
    private List<ProductoData> productsToLoad;
    private GameObject m_downloadIndicator;
    private GameObject m_loadingIndicator;  
    private GameObject loadedBuilding;
    //Variables requeridas como parametros de envio para servicios php
    private string _productCollection="";
    private int _buildingId;
    //Modficadores de acceso y propiedades
    public string ProductCollection
    {
        get { return _productCollection; }
        set { _productCollection = value; }
    }

    public int BuildingId
    {
        get { return _buildingId; }
        set { _buildingId = value; }
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
    void Start()
    {

    }

    /// <summary>
    /// Método receptor del código_inmueble
    /// </summary>
    /// <param name="paramBuildingCode"></param>
    public void GetBuildingCodeUseToSave(string paramBuildingCode)
    {
        buildingCodeUseToSave = paramBuildingCode;
    }

    public void GetOriginalProducts()
    {
        var originalProducts = m_QueriesController.GetAllProducts();
        foreach(var item in originalProducts)
        {
            Debug.Log($"IdProBodPre del producto: {item.idProBodPre}");
        }
    }

    public void StartSaveInmueble()
    {
        //if (!m_Usuario.CheckSesion())
        //{
        //    m_MessagesController.CreateAnswer("Aviso", "Para guardar el inmueble, necesita iniciar sesión",
        //     () => { m_MessagesController.CloseAnswer(); },
        //     () => { m_Usuario.OpenProfile(); m_MessagesController.CloseAnswer(); }
        //   );
        //   return;

        //}

        //Indicador de Carga
        m_loadingIndicator = InstantiateInPlacement(m_prefabLoading);
        _ = StartToSaveData((value) => LoadProgressIndicator(value), () => Destroy(m_loadingIndicator));
    }

    public async UniTaskVoid StartToSaveData(Action<float>progressFunction, Action successAction=null)
    {
        try
        {
            await SaveBuildingData(progressFunction);
            await SaveProductsData(progressFunction);
            
            progressFunction?.Invoke(1.0f); //finalizar progreso

            successAction?.Invoke();  //Llamar acción final

            TutorialControl.sharedInstance.CreatePopupNotification(tutorialItems.popupNotification, PopupsNotificationsIcons.successIcon,
                    "¡REALIZADO!", "Inmueble guardado correctamente", 3f);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al guardar inmueble y productos:{ex.Message}");
            //progressFunction?.Invoke(1.0f);
        }     
    }

    public async void StartToLoadData(int paramBuildingId, string paramBuildingCode)
    {
        m_downloadIndicator = InstantiateInPlacement(m_prefabDownload);//instancia el prefab que contiene la animacion / elementos de descarga en el GameObject downloadindicator
        m_loadingController.OpenAssetBundle();

        try
        {
            await LoadBuilding(paramBuildingCode, (value) => DownloadProgressIndicator(value * 0.5f));
            await LoadProducts(paramBuildingId, (value) => DownloadProgressIndicator(0.5f + value * 0.5f));

            DownloadProgressIndicator(1f); // Al final, marca 100%
        }
        catch (Exception ex)
        {

            Debug.LogError($"Error al cargar el inmueble y los productos en la escena:{ex.Message}");
        }
        finally
        {
            if (m_downloadIndicator != null)
                Destroy(m_downloadIndicator);                                          
        }
    }


    public async void StartToLoadDefaultData(int paramBuildingId, string paramBuildingCode)
    {
        m_downloadIndicator = InstantiateInPlacement(m_prefabDownload);//instancia el prefab que contiene la animacion / elementos de descarga en el GameObject downloadindicator
        m_loadingController.OpenAssetBundle();

        try
        {
            await LoadBuilding(paramBuildingCode, (value) => DownloadProgressIndicator(value * 0.5f));
            await LoadDefaultProducts(paramBuildingId, (value) => DownloadProgressIndicator(0.5f + value * 0.5f));
            
            DownloadProgressIndicator(1f); // Al final, marca 100%
        }
        catch (Exception ex)
        {

            Debug.LogError($"Error al cargar el inmueble  y productos predefinidos en la escena:{ex.Message}");
        }
        finally
        {
            if (m_downloadIndicator != null)
                Destroy(m_downloadIndicator);
        }
    }

    public void StartToLoadFavoriteBuildings()
    {
        //if (!m_Usuario.CheckSesion())
        //{
        //    m_MessagesController.CreateAnswer("Aviso", "Para ingresar a este apartado necesita iniciar sesión",
        //     () => { m_MessagesController.CloseAnswer(); },
        //     () => { m_Usuario.OpenProfile(); m_MessagesController.CloseAnswer(); }
        //   );
        //    return;

        //}
        //Indicador de Carga
        m_loadingIndicator = InstantiateInPlacement(m_prefabLoading);
        _ = LoadFavoriteBuildings((value) => LoadProgressIndicator(value), () => Destroy(m_loadingIndicator));
    }

    private async UniTask SaveBuildingData(Action <float>progressFunc)
    {
        Debug.Log($"Ingrese a SaveInmueble con el codigo: {buildingCodeUseToSave}");

        string service = "insertar_inmueble.php";
        string path = url + service;

        if (m_downloadComponentController.componentsBuildings.Count == 0)
        {
            Debug.LogError("No hay componentes de inmuebles para guardar.");
            return;
        }

        foreach (var category in m_downloadComponentController.componentsBuildings)
        {
            foreach (var subCategory in category.Tipo_Inmueble)
            {
                foreach (var inmueble in subCategory.Inmuebles)
                {
                    if (!inmueble.Codigo_Inmueble.Equals(buildingCodeUseToSave)) continue;

                    BuildingId = int.Parse(inmueble.Id_Inmueble);
                    //*Imagenes del inmueble
                    buildingPictures = inmueble.Pictures.Select(p => p.Picture).ToList();

                    // *Crear el objeto con los datos del inmueble
                    InmuebleData building = new()
                    {
                        Id_Inmueble = int.Parse(inmueble.Id_Inmueble),
                        Codigo_Inmueble = inmueble.Codigo_Inmueble,
                        Nombre_Inmueble = inmueble.Nombre_Inmueble,
                        Descripcion_Inmueble = inmueble.Descripcion_Inmueble,
                        Picture1 = buildingPictures[0],
                        Picture2 = buildingPictures[1],
                        Picture3 = buildingPictures[2],
                        Picture4 = buildingPictures[3],
                        Picture5 = buildingPictures[4],
                        //correo = Usuario.sharedInstance.Correo,
                    };

                    // Convertir el objeto a JSON
                    string jsonData = JsonUtility.ToJson(building);
                    Debug.Log($"JSON enviado SaveBuildingData: {jsonData}");

                    try
                    {
                        using UnityWebRequest request = new UnityWebRequest(path, "POST")
                        {
                            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData)),
                            downloadHandler = new DownloadHandlerBuffer()
                        };

                        request.SetRequestHeader("Content-Type", "application/json");

                        //Objeto de progress
                        var progress = Progress.Create<float>(percent => progressFunc?.Invoke(percent * 0.5f));

                        await request.SendWebRequest().ToUniTask(progress);

                        if (request.result == UnityWebRequest.Result.Success)
                        {
                            Debug.Log($"Petición exitosa en SaveBuildingData: {request.downloadHandler.text}");
                        }
                        else
                        {
                            Debug.LogError($"Error en la petición: {request.error}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error en la solicitud: {ex.Message}");
                    }

                    return; // Salimos después de guardar el inmueble correcto
                }   
            }
        }

        Debug.LogError("No se encontró el inmueble con el código especificado");
    }

    private async UniTask SaveProductsData(Action<float> progressFunc) //GUardado de productos con clones
    {
        string service = "Update_ProductoInmueble.php";
        string path = url + service;

        //Creación de la coleccion de productos
        ProductosDataCollection productosDataCollection = new()
        {
            productos = new List<ProductoData>()
        };

        //Referencia a productos originales
        var originalProducts = m_QueriesController.GetAllProducts();
      
        //Agrupación de productos y sus clones
        foreach(var product in originalProducts)
        {
            await UniTask.Yield(); // Evita bloqueos
          
            ProductoData updatedProductoData = new()
            {
                IdProBodPre = product.idProBodPre,
                idCaracteristica = product.Caracteristicas.FirstOrDefault().idCaracteristica, //Uso de FirstOrDefault ya que por el momento todos los productos cuentan con una
                                                                                              //caracteristica
                clones = new List<Clone>()
            };

            foreach (Transform cloneProduct in fatherProducts.transform) //Clones de los productos originales
            {
                var cloneProps = cloneProduct.GetComponent<ObjectProperties>();
                if (cloneProps == null)
                {
                    Debug.LogError("el clone del Producto NO cuenta con ObjectProperties.");
                    return;
                }
                 
                if (cloneProps.idProBodPre == product.idProBodPre)
                {
 
                    updatedProductoData.clones.Add(new Clone
                    {
                        IdClone = cloneProduct.gameObject.GetComponent<ObjectProperties>().UniqueId,
                        posicion = cloneProduct.transform.position,
                        rotacion = cloneProduct.transform.rotation
                    });
                }

            }
            if (updatedProductoData.clones.Count > 0)
            {
                productosDataCollection.productos.Add(updatedProductoData);
            }
        }

        //Los datos de los productos se transforman en JSON
        ProductCollection = JsonUtility.ToJson(productosDataCollection);

        //Construcción de JSON que recibirá el servicio
        InmuebleData paramsForService = new()
        {
            Id_Inmueble = BuildingId,
            //correo = Usuario.sharedInstance.Correo,
            Productos_Data = ProductCollection,

        };

        // Convertir el objeto a JSON
        string jsonData = JsonUtility.ToJson(paramsForService);

        Debug.Log($"JSON enviado en SaveProductsData: {jsonData}");

        try
        {
            using UnityWebRequest request = new UnityWebRequest(path, "POST")
            {
                uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");

            //Objeto de progress
            var progress = Progress.Create<float>(percent => progressFunc?.Invoke(0.5f + percent * 0.5f));
            await request.SendWebRequest().ToUniTask(progress);

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Petición exitosa en SaveProductsData: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Error en la petición: {request.error}");
            }
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en la solicitud: {ex.Message}");
        }
    }

    private async UniTask LoadBuilding(string parameter,Action<float>loadBuildingProgress)
    {
        //Ruta del servidor donde se encuentran los Assetbundles de los inmuebles
        string path = "";
        await config.sharedInstance.UrlArvispaceAssetsBuildings(parameter, (x) => path = x);

        //Descarga de assetbundle 
        var progress = Progress.Create<float>(percent => loadBuildingProgress?.Invoke(percent * 0.5f));//Objeto de progress

        var request = await UnityWebRequestAssetBundle.GetAssetBundle(path).SendWebRequest().ToUniTask(progress);

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log("Error al descargar el AssetBundle" + request.error);
            return;
        }

        AssetBundle bundle = ((DownloadHandlerAssetBundle)request.downloadHandler).assetBundle;

        if (bundle != null)
        {
            MenuBuildingsView.Clear(fatherBuildings);//Limpia Padre de los inmuebles
            MenuBuildingsView.Clear(fatherProducts);//Limpia Padre de los Productos
            RelocatePlayer.sharedInstance.PosInitial();//Reestablece la posicion del jugador en la pos inicial
            UIController.sharedInstance.HideFavoriteBuildingsMenu();
            UIController.sharedInstance.HideMenuBuildings();
            //Cargar el GameObject
            GameObject obj = bundle.LoadAsset<GameObject>(parameter);

            //Descargar el AssetBundle después de su uso
            bundle.Unload(false);

            loadedBuilding = Instantiate(obj, fatherBuildings.transform);
        }
        else
        {
            Debug.LogError($"El AssetBundle del inmueble {parameter} es nulo");
        }

    }

    private async UniTask LoadProducts(int buildingId, Action<float> loadProductsProgress)
    {
        string service = "Get_ProductoInmueble.php";
        string path = url + service;

        InmuebleData paramsForService = new()
        {
            //correo = Usuario.sharedInstance.Correo,
            Id_Inmueble = buildingId,  
        };

        string jsonData = JsonUtility.ToJson(paramsForService);

        try
        {
            using UnityWebRequest request = new UnityWebRequest(path, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest().ToUniTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error en la solicitud: {request.error}");
                return;
            }

            string dataResponse = request.downloadHandler.text;
            Debug.Log($"La Data de los productos en el inmueble favorito es: {dataResponse}");

            ProductosDataCollection productsFromJSON = new();

            try
            {
                productsFromJSON = JsonUtility.FromJson<ProductosDataCollection>(dataResponse);
                productsToLoad = productsFromJSON.productos ?? new List<ProductoData>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al deserializar JSON y obtener la lista de productos: {ex.Message}");
                return;
            }

            if (productsToLoad == null || productsToLoad.Count == 0)
            {
                Debug.LogError("No se encontraron productos para cargar");
                return;
            }

            Dictionary<int, ProductoData> productDictionary = productsToLoad.ToDictionary(p => p.IdProBodPre, p => p);
            List<UniTask> tasks = new();

            int total = productsToLoad.Count;
            int completed = 0;

            foreach (var product in productsToLoad)
            {
                var task = LoadClonesByProduct(product, productDictionary).ContinueWith(() =>
                {
                    completed++;
                    float progress = (float)completed / total;
                    loadProductsProgress?.Invoke(progress);
                });

                tasks.Add(task);
            }

            await UniTask.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en la solicitud: {ex.Message}");
        }

    }

    private async UniTask LoadDefaultProducts(int buildingId, Action<float> loadProductsProgress)
    {
        string service = "Get_DefaultProductoInmueble.php";
        string path = url + service;

        InmuebleData paramsForService = new()
        {
            Id_Inmueble = buildingId,
        };

        string jsonData = JsonUtility.ToJson(paramsForService);

        try
        {
            using UnityWebRequest request = new UnityWebRequest(path, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest().ToUniTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error en la solicitud: {request.error}");
                return;
            }

            string dataResponse = request.downloadHandler.text;
            Debug.Log($"La Data de los productos es: {dataResponse}");

            ProductosDataCollection productsFromJSON = new();

            try
            {
                productsFromJSON = JsonUtility.FromJson<ProductosDataCollection>(dataResponse);
                productsToLoad = productsFromJSON.productos ?? new List<ProductoData>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al deserializar JSON y obtener la lista de productos: {ex.Message}");
                return;
            }

            if (productsToLoad == null || productsToLoad.Count == 0)
            {
                Debug.LogError("No se encontraron productos para cargar");
                return;
            }

            Dictionary<int, ProductoData> productDictionary = productsToLoad.ToDictionary(p => p.IdProBodPre, p => p);
            List<UniTask> tasks = new();

            int total = productsToLoad.Count;
            int completed = 0;

            foreach (var product in productsToLoad)
            {
                var task = LoadClonesByProduct(product, productDictionary).ContinueWith(() =>
                {
                    completed++;
                    float progress = (float)completed / total;
                    loadProductsProgress?.Invoke(progress);
                });

                tasks.Add(task);
            }

            await UniTask.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en la solicitud: {ex.Message}");
        }
    }

    private async UniTask LoadClonesByProduct(ProductoData product, Dictionary<int, ProductoData> productDictionary)
    {
        
        //Ruta del servidor donde se encuentran los Assetbundles de los productos
        string path = "";
        await config.sharedInstance.urlArvispaceAssets(product.IdProBodPre.ToString(), (x) => path = x);

        AssetBundle bundle = await Downloads.DownloadAssetBundleProduct(path);
        if (bundle == null)
        {
            Debug.LogError($"El AssetBundle del producto {product.IdProBodPre} es nulo.");
            return;
        }

        GameObject obj = bundle.LoadAsset<GameObject>(product.IdProBodPre.ToString()); // Cargar prefab del producto original   
        if (obj == null)
         {   
            Debug.LogError($"No se pudo cargar el prefab del producto {product.IdProBodPre} desde el bundle.");
            return;
        }

        // Lista local de clones para evitar sobrescritura en ejecuciones paralelas
        var clonesToLoad = product.clones ?? new List<Clone>();

        try //Obtener clones e instanciarlos
        {
            if (productDictionary.TryGetValue(product.IdProBodPre, out ProductoData productToLoad))
            {
                if (clonesToLoad == null || clonesToLoad.Count == 0)
                {
                    Debug.LogError($"El producto {product.IdProBodPre} no tiene clones para instanciar.");
                    return;
                }

                int count = 0;
                foreach (var clone in clonesToLoad)
                {
                    if (clone == null)
                    {
                        Debug.LogError($"Clone nulo encontrado para el producto {product.IdProBodPre}");
                        continue;
                    }

                    GameObject loadedClone = Instantiate(obj, clone.posicion, clone.rotacion, fatherProducts.transform);

                    if (loadedClone == null)
                    {
                        Debug.LogError("No se pudo instanciar un clon.");
                        continue;
                    }

                    ////Acceso a ObjectProperties
                    ObjectProperties objectProperties = loadedClone.GetComponent<ObjectProperties>();
                    objectProperties.idSelected = product.idCaracteristica;  
                    objectProperties.idProBodPre= product.IdProBodPre;

                    objectProperties.isSelected = true;
                    objectProperties.placed= true;
                    objectProperties.validePlaced = true;

                    loadedClone.layer = 11;
                    count++;

                    // Cede el frame cada 10 instancias para no congelar el hilo principal
                    if (count % 10 == 0) await UniTask.Yield();
                }
                Debug.Log($"Producto {product.IdProBodPre} y {clonesToLoad.Count} clones instanciados correctamente.");
            }
            else
            { 
                Debug.LogError($"Producto {product.IdProBodPre} no encontrado en el diccionario.");
                return;
            }
        }
        catch(Exception ex)
        {

            Debug.LogError($"Error al obtener e instanciar clones:{ex.Message}");
        }
        finally
        {
            bundle.Unload(false);
        }
    } 
    private async UniTask LoadFavoriteBuildings(Action<float> loadFavoriteBuildingsProgress, Action successAction = null)
    {
        try
        {
            // Etapa 1: Preparación de la solicitud (10% del progreso)
            loadFavoriteBuildingsProgress?.Invoke(0.1f);

            string service = "https://arvispace.com/serviciosASAR/Obtener_Inmuebles.php";

            InmuebleData correoInmuebleData = new()
            {
                //correo = Usuario.sharedInstance.Correo
            };

            string jsonData = JsonUtility.ToJson(correoInmuebleData);

            using (UnityWebRequest request = new UnityWebRequest(service, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                // Asignar 30% del progreso a la petición
                var progress = Progress.Create<float>(percent =>
                {
                    // Mapear el progreso de 10% a 40% (30% del total)
                    loadFavoriteBuildingsProgress?.Invoke(0.1f + percent * 0.3f);
                });

                await request.SendWebRequest().ToUniTask(progress);

                // Verificar si hubo errores en la solicitud
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error en la solicitud: {request.error}");
                    return;
                }
                // Etapa 3: Procesamiento de datos (20% del progreso)
                loadFavoriteBuildingsProgress?.Invoke(0.4f);

                string data = request.downloadHandler.text;
                Debug.Log($"La Data de los inmuebles favoritos es: {data}");

                // Procesamiento con progreso intermedio
                await UniTask.DelayFrame(1); // Para asegurar que se actualice la UI

                //Deserializar JSON
                buildingsToLoad = DeserializeJsonFavoriteBuildings(data);
                loadFavoriteBuildingsProgress?.Invoke(0.6f);


                if (buildingsToLoad == null || buildingsToLoad.Count == 0)
                {
                    // Etapa 4: Procesamiento de caso vacío (10% del progreso)
                    loadFavoriteBuildingsProgress?.Invoke(0.7f);
                    
                    successAction?.Invoke();
                    //TutorialControl.sharedInstance.CreateInfoNotification(tutorialItems.infoNotification,
                      //      "No has guardado inmuebles", "¡Guarda algunos ahora mismo!", "Ir a Explorar", () => m_UIController.ShowMenuBuildings());

                }
                else
                {
                    // Etapa 4: Instanciación de productos (40% del progreso)
                    var instantiateProgress = Progress.Create<float>(percent =>
                    {
                        // Mapear el progreso de 60% a 100%
                        loadFavoriteBuildingsProgress?.Invoke(0.6f + percent * 0.4f);
                    });

                    await InstatiateFavoriteProductsAsync(instantiateProgress);
                    successAction?.Invoke();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error inesperado en LoadFavoriteBuildings: {ex.Message}");
            loadFavoriteBuildingsProgress?.Invoke(1f); // Asegurar que se complete incluso con error
        }
    }

    private List<InmuebleData> DeserializeJsonFavoriteBuildings(string json)
    {
        //Deserializar JSON
        InmueblesDataCollection inmueblesFromJSON = new();
        try
        {
            inmueblesFromJSON = JsonUtility.FromJson<InmueblesDataCollection>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en la generación de la lista Inmuebles Favoritos: {ex.Message}");

        }
        return inmueblesFromJSON.data;
    }

    private async UniTask InstatiateFavoriteProductsAsync(IProgress<float> progress)
    {
        try
        {
            Clear(m_scrollToFavoriteBuildingsView.m_container);

            int totalBuildings = buildingsToLoad.Count;
            int processed = 0;

            foreach (var inmueble in buildingsToLoad)
            {
                loadedBuilding = Instantiate(
                    prefabToFavoriteBuildingView,
                    m_scrollToFavoriteBuildingsView.m_container.transform);

                loadedBuilding.GetComponent<ContainerFavoriteBuilding>().favoriteBuilding = inmueble;

                processed++;
                progress.Report(processed / (float)totalBuildings);

                // Permitir que la UI se actualice cada pocos elementos
                if (processed % 5 == 0)
                {
                    await UniTask.Yield();
                }
            }

            m_scrollToFavoriteBuildingsView.onChangeContent();
            m_scrollToFavoriteBuildingsView.ShowElements();
            m_UIController.ShowFavoriteBuildingsMenu();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al instanciar productos favoritos: {ex.Message}");
            throw;
        }
    }
    private static void Clear(params GameObject[] containers)
    {
        foreach (var item in containers)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                Destroy(item.transform.GetChild(i).gameObject);
            }
        }
    }
    private GameObject InstantiateInPlacement(GameObject toInstantiate)
    {
        EmpyPlacement();

        return Instantiate(toInstantiate, m_placement.transform);
    }

    public void EmpyPlacement()
    {
        for (int i = 0; i < m_placement.transform.childCount; i++)
        {
            Destroy(m_placement.transform.GetChild(i).gameObject);
        }
    }
    private void DownloadProgressIndicator(float currentProgress)
    {
        float percentage = currentProgress * 100;//saca un porcentaje de lo que lleva de descarga
        float rounded = (float)(Math.Round((double)percentage, 0));//redondeamos a dos digitos
        if (percentage == 100)
        {
            //Cierre del canvas de descarga 
            m_loadingController.CloseAssetBundle();
        }
        m_downloadIndicator.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = currentProgress;
        m_downloadIndicator.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = rounded.ToString() + " %";
    }
    private void LoadProgressIndicator(float currentProgress)
    {
        m_loadingIndicator.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = currentProgress;
    }
    public void ClearCache(string assetBundleName)
    {
        Debug.Log($"Valor recibido en ClearCache:{assetBundleName}");
        
        // Intenta limpiar todas las versiones en caché del AssetBundle
        if (Caching.ClearAllCachedVersions(assetBundleName))
        {
            Debug.Log($"Se eliminaron todas las versiones en caché de: {assetBundleName}");
        }
        else
        {
            Debug.LogWarning($"No se pudo limpiar la caché de: {assetBundleName} (posiblemente no existía en la caché)");
        }
    }

    [System.Serializable]
    public class InmuebleData
    {
        public int Id_Inmueble;
        public string Codigo_Inmueble;
        public string Nombre_Inmueble;
        public string Descripcion_Inmueble;
        public string Picture1;
        public string Picture2;
        public string Picture3;
        public string Picture4;
        public string Picture5;
        public string correo;
        public string Productos_Data;
    }

    [System.Serializable]
    public class InmueblesDataCollection
    {
        public string status;
        public string msg;
        public List<InmuebleData> data;  
    }

}

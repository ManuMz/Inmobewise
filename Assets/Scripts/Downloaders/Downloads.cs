using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
//using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
//using UnityEditor.PackageManager;
using UnityEngine.SocialPlatforms;

public class Downloads
{
    //private static Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();--------------------------------Verificar su uso-------------------------------------------------------[1]
    /// <summary>
    /// Descarga de texturas / imagenes
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async UniTask<Texture2D> DownloadTexture(string url)
    {
        try
        {
            var request = await UnityWebRequestTexture.GetTexture(url).SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                return null;
            }
            else
            {
                return ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
        }
        catch (Exception)
        {
            Texture2D texture = await Resources.LoadAsync<Texture2D>("404") as Texture2D;//En caso de no existir la textura de imagen 
                                                                                         // se mostrara la imagen/textura 404
            Debug.LogError("TextName: " + texture.name);
            return texture;
        }
    }
    /// <summary>
    /// Descarga de texto
    /// </summary>
    /// <param name="url"></param>
    /// <param name="form"></param>
    /// <returns></returns>
    public static async UniTask<string> DownloadText(string url, WWWForm form = null)
    {
        UnityWebRequest request;

        if (form == null)
        {
            request = await UnityWebRequest.Get(url).SendWebRequest();
        }
        else
        {
            request = await UnityWebRequest.Post(url, form).SendWebRequest();
        }

        if (request.isHttpError || request.isNetworkError)
        {
            MessagesControllers.sharedInstance.CreateNotification("Error", "No se encuentra el texto en el servidor", 2f, MessagesControllers.iconsNotifications.error);
            return "error";
        }

        return request.downloadHandler.text;
    }
    /// <summary>
    /// Descarga de AssetBundles de los inmuebles
    /// </summary>
    /// <param name="url"></param>
    /// <param name="progressFunction"></param>
    /// <param name="successAction"></param>
    /// <returns></returns>
    public static async UniTask<AssetBundle> DownloadAssetBundle(string url, Action<float> progressFunction, Action successAction = null)
    {
        try
        {
            Debug.Log("Valor de URL en DownloadAssetBundle: " + url);

            // 1. Limpiar la cache si es necesario (opcional)
            /*if (forceClearCache)
            {
                Debug.Log("Limpiando la cache de Asset Bundles...");
                if (Caching.ClearCache())
                {
                    Debug.Log("Cache de Asset Bundles limpiada correctamente.");
                }
                else
                {
                    Debug.Log("Cache de Asset Bundles NO LIMPIADA correctamente.");
                }
            }*/

            // 2. Crear el objeto de progreso para la descarga
            var progress = Progress.Create<float>((percent) => progressFunction(percent));
            // 3.Intentar descargar el AssetBundle
            var request = await UnityWebRequestAssetBundle.GetAssetBundle(url).SendWebRequest().ToUniTask(progress);

            // 4. Manejo de errores si la descarga falla
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log("Error al descargar el AssetBundle" + request.error);
                return null;// Se retornar? algun elemento que indique que existe un error al descargar el 
                            //modelo o que no existe: return LoadLocalAssetBundle();
            }

            // 5. Llamar a la acci?n de ?xito si se pas?
            successAction?.Invoke();
            // 6. Retornar el Asset Bundle descargado
            return ((DownloadHandlerAssetBundle)request.downloadHandler).assetBundle;
        }
        catch (Exception ex)
        {
            //MessagesControllers.sharedInstance.CreateNotification("Error", "No hay modelo en el servidor", 3f, MessagesControllers.iconsNotifications.error);
            Debug.Log(ex.Message);
            return null;
            //Intentamos cargar un AssetBundle local en caso de error
            //return LoadLocalAssetBundle("notfound404"); (Solo de referencia)
        }
    }

    public static async UniTask<AssetBundle> DownloadAssetBundleProduct(string url)
    {
        
        try
        {
            Debug.Log("Valor de URL en DownloadAssetBundle: " + url);
            //Intentar descargar el AssetBundle
            var request = await UnityWebRequestAssetBundle.GetAssetBundle(url).SendWebRequest();

            //Manejo de errores si la descarga falla
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log("Error al descargar el AssetBundle" + request.error);
                return null;
            }

            //Retornar el Asset Bundle descargado
            return ((DownloadHandlerAssetBundle)request.downloadHandler).assetBundle;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return null;

        }
    }

    /// <summary>
    /// M?todo para cargar un AssetBundle desde un archivo local
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    private static AssetBundle LoadLocalAssetBundle(string bundleName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, bundleName);

        //-----------------------------------------------------------------------Verificar si se requiere el uso del diccionario de assetbundles descargados desde el servidor---------------------------------[2]
        //// Verifica si el AssetBundle ya est? cargado
        //if (loadedBundles.TryGetValue(bundleName, out AssetBundle existingBundle))
        //{
        //    Debug.Log("El AssetBundle ya est? cargado: " + bundleName);
        //    return existingBundle;
        //}
        //-------------------------------------------------------------------------------------------------
        AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
        if (assetBundle == null)
        {
            Debug.LogError("No se pudo cargar el AssetBundle desde la ruta: " + path);
        }
        else//-------------------------------
        {
            // Guardar el AssetBundle cargado localmente-------------------------------------------------Verificar su uso dentro del caso-------------------------------------------------------------------------[3]
            //loadedBundles[bundleName] = assetBundle;

        }//---------------------------------------------

        Debug.Log("Se cargo el AssetBundle de manera local: " + bundleName);
        return assetBundle;
    }

    /// <summary>
    /// Carga de bytes
    /// </summary>
    public class Uploads
    {
        public static async UniTask<bool> UploadBytes(string url, byte[] data, Action<float> progressFunction = null)
        {
            var progress = Progress.Create<float>((percent) => progressFunction?.Invoke(percent));
            UnityWebRequest www = await UnityWebRequest.Put(url, data).SendWebRequest().ToUniTask(progress);
            if (www.result.Equals(UnityWebRequest.Result.Success))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

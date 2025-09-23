using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class RESTApi //Sustituye a lo que se puede hacer
                     //con la clase nativa .Net: HttpClient
{
    public static async UniTask<string> HTTPGET(string url) //DownloadHandler
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        //Agregar headers
        request.SetRequestHeader("Content-Type", "application/json");

        //Ejecutar operacion
        await request.SendWebRequest();

        //Evaluar resultado de la operacion
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error en operacion GET:{request.error}");
            return null;
        }
        else
        {
            return request.downloadHandler.text;
        }

    }

    public static async UniTask HTTPPOST(string url,WWWForm formData)//UploadHandler
    {
        UnityWebRequest request = UnityWebRequest.Post(url,formData);

        //Agregar headers
        request.SetRequestHeader("Content-Type", "application/json");

        //Ejecutar operacion
        await request.SendWebRequest();

        //Evaluar resultado de la operacion
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error en operacion POST:{request.error}");
        }
        else
        {

        }

    }

    public static async UniTask HTTPPUT()
    {

    }
    public static async UniTask HTTPDELETE()
    {

    }

    //SUBIDA - DESCARGA DE ASSETS
    public static async UniTask Upload(string url)
    { 

    }
    public static async UniTask<T>Download<T>(string url)
        where T : class
    {
        UnityWebRequest request;

        switch (typeof(T).Name)
        {
            case  nameof(Texture2D):
                request = UnityWebRequestTexture.GetTexture(url);
                break;
            case nameof(AssetBundle):
                request = UnityWebRequestAssetBundle.GetAssetBundle(url);   
                break;
            default: 
                throw new ArgumentException();
        }
        
        //Ejecutar operacion
        await request.SendWebRequest();

        //Evaluar resultado de la operacion
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error en operacion Download:{request.error}");
            //return null;
        }

        switch (typeof(T).Name)
        {
            case nameof(Texture2D):
                return DownloadHandlerTexture.GetContent(request) as T;

            case nameof(AssetBundle):
                return DownloadHandlerAssetBundle.GetContent(request) as T;

            default:
                return null;
        }
    }

}

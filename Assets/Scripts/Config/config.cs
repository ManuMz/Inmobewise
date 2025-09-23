using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System;
//using UnityEditor.Experimental.GraphView;

[ExecuteInEditMode]
public class config : MonoBehaviour
{
    public static config sharedInstance;

    #region Enums

    /// <summary>
    /// Enum useMode hace referencia al entorno en que se ejecutara la app, ya sea para pruebas o para realizar un compilado
    /// </summary>
    public enum useMode
    {
        testing,
        production
    }
    ///<summary>
    /// Enum localFiles hace referencia al uso de archivos que se encuentran, en carpetas locales de la app  
    /// </summary>
    public enum localFiles
    {
        yes,
        no //NO se usa
    }
    /// <summary>
    /// Enum localAssets hace referencia al uso de AssetBundles que se encuentran de manera local  
    /// </summary>
    public enum localAssets
    {
        Yes,
        No
    }
    /// <summary>
    /// En enum apps podemos agregar otras apps que requieren descargar archivos, assetbundles  
    /// </summary>
    public enum apps
    {
        Arvispace = 1, //produccion
        Inmobewise = 88 //produccion EMPRESA APP
    }
    #endregion

    #region variables privadas serializadas 
    [SerializeField]
    public apps app; //variable global referente al enum apps

    [SerializeField]
    /*private*/
    public useMode mUseMode;//variable global referente al enum useMode

    [SerializeField]
    private List<GameObject> intros;// al momento de que solicite una demo de determinada app, la lista intros se modificará con los elementos de dicha app  
    
    [SerializeField]
    private localAssets mLocalAssets; //variable global referente al enum localAssets
    #endregion

    #region Obtener valores de variables privadas 
    
    /// <summary>
    /// Obtiene el valor del enum apps actual/en ejecución 
    /// </summary>
    /// <returns></returns>
    public apps GetApp()
    {
        return app;
    }
    /// <summary>
    /// Obtiene el valor del enum useMode actual/ en ejecución
    /// </summary>
    /// <returns></returns>
    public useMode GetmUseMode() 
    {
        return mUseMode;
    }
    /// <summary>
    /// Obtiene el valor del enum localAssets actual/en ejecución
    /// </summary>
    /// <returns></returns>
    public localAssets GetLocalAssets() 
    {
        return mLocalAssets;
    }
    /// <summary>
    /// Obtiene la lista de intros de acuerdo a la app en ejecución
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetIntros()
    {
        return intros;
    }

    #endregion
    private void Awake()
    {
        if (!sharedInstance)
            sharedInstance = this;
        else
            Destroy(gameObject);
    }

    #region Public Methods
    /// <summary>
    /// servicios web a consumir 
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public string urlArvispaceServices(string service)
    {                                                     
        string url = "";

        switch (app)
        {
            case apps.Inmobewise:
                switch (mUseMode)
                {
                    case useMode.testing:
                        url = "https://arvispace.com/serviciosASARAmbientePruebas/";
                        break;
                    case useMode.production://------------->
                        //url = "https://arvispace.com/serviciosASAR/";
                        url = "https://arvispace.com/InmobewiseApp/serviciosASAR/";
                        break;
                }
                break;
                
        }

        return url + service;
    }
    
    /// <summary>
    /// Obtención de assetbundles a partir de su identificador ID
    /// </summary>
    /// <param name="name"></param>
    /// recibe idProBodPre de producto
    /// <param name="pathResponse"></param>
    /// <returns></returns>
    public IEnumerator urlArvispaceAssets(string name, Action<string> pathResponse) 
    {
#if UNITY_ANDROID
        string platform = "android";
#else
        string platform = "ios";
#endif
        string path = "";

        if (mLocalAssets.Equals(localAssets.Yes))
        {
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(Path.Combine(Application.streamingAssetsPath, "assetBundles", platform, name)))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    //path = Path.Combine("https://arvispace.com/resources/assetbundle/", platform, name);//ingresa a la base de datos, plataforma y modelo a descargar
                    path = Path.Combine("https://arvispace.com/InmobewiseApp/resources/assetbundles/", platform, name);//ingresa a la base de datos, plataforma y modelo a descargar
                }
                else
                {
                    path = Path.Combine(Application.streamingAssetsPath, "assetbundles", platform, name);//Ingresa por medio de la ruta de acceso streamingAssetsPath a la carpeta local assetbundles, plataforma y modelo a descargar 
                }
            }
        }
        else
        {
            //path = Path.Combine("https://arvispace.com/resources/assetbundle/", platform, name);//ingresa a la base de datos, plataforma y modelo a descargar
            path = Path.Combine("https://arvispace.com/InmobewiseApp/resources/assetbundles/", platform, name);//ingresa a la base de datos, plataforma y modelo a descargar
        }

        pathResponse(path);
    }

    /// <summary>
    /// Obtención de assetbundle para descarga de los inmuebles
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pathResponse"></param>
    /// <returns></returns>
    public IEnumerator UrlArvispaceAssetsBuildings(string name, Action<string> pathResponse) 
    {
        //#if UNITY_EDITOR
        //        string platform = "windows";
#if UNITY_ANDROID
        string platform = "android";
#else
        string platform = "ios";
#endif
        string path = "";

        if (mLocalAssets.Equals(localAssets.Yes))
        {
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(Path.Combine(Application.streamingAssetsPath, "assetBundles", platform, name)))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    //path = Path.Combine("https://arvispace.com/assetbundlefromhouses/", platform, name);//ingresa a la base de datos, plataforma y modelo a descargar
                    path = Path.Combine("https://arvispace.com/InmobewiseApp/resources/assetbundlesfrombuildings/", platform, name);//ingresa a la base de datos, plataforma y modelo a descargar
                }
                else
                {
                    path = Path.Combine(Application.streamingAssetsPath, "assetbundles", platform, name);//Ingresa por medio de la ruta de acceso streamingAssetsPath a la carpeta local assetbundles, plataforma y modelo a descargar 
                }
            }
        }
        else
        {
            //Debug.Log("Descarga exitosa. Utilizando assetbundle local.");
            //path = Path.Combine("https://arvispace.com/assetbundlefromhouses/", platform, name);//ingresa a la base de datos, plataforma y modelo a descargar
            path = Path.Combine("https://arvispace.com/InmobewiseApp/resources/assetbundlesfrombuildings/", platform, name);//ingresa a la base de datos, plataforma y modelo a descargar
        }

        //Debug.Log("Ruta del assetbundle: " + path);
        pathResponse(path);
    }

    /// <summary>
    /// Acceso a perfil y compras en la app
    /// </summary>
    /// <param name="file"></param>
    /// <param name="localFiles"></param>
    /// <returns></returns>
    public string urlArviSpaceForms(string file, localFiles localFiles = localFiles.no)
    {                                                                                   
        string url = "";
        switch (localFiles)
        {
            case localFiles.yes://------------------>
                switch (app)
                {  
                    case apps.Inmobewise:
                        switch (mUseMode)
                        {
                            case useMode.testing://NO UTILIZAMOS
                                //url = UniWebViewHelper.StreamingAssetURLForPath("www/arvispace/v1/");
                                url = "https://arvispace.com/ArvisApp/testing/v1/";
                                break;
                            case useMode.production://--------------->
                                //url = UniWebViewHelper.StreamingAssetURLForPath("www/arvispace/v1/");
                                //url = "https://arvispace.com/ArvisApp/Production/v1/";
                                url = "https://arvispace.com//InmobewiseApp/Production/";
                                break;
                        }
                        break;
                        
                }
                url += file + "/index.html";
                break;
            case localFiles.no:
                switch (app)
                {
                    case apps.Arvispace:
                        switch (mUseMode)
                        {
                            case useMode.testing:
                                url = "https://sandbox.arvispace.com/front-end/v1/"; //NO UTILIZAMOS
                                break;
                            case useMode.production:
                                url = "https://production.arvispace.com/front-end/v1/"; //NO UTILIZAMOS
                                break;
                        }
                        break;
                }
                url += file;
                break;
            default:
                break;
        }
        return url;
    }
    #endregion


    #region Desactivación de intro
    /// <summary>
    /// metodo que recorre cada item de la lista intros y lo desactiva, se encuentra inhabilitado por el momento
    /// </summary>
    void desactiveIntros()
    {
        foreach (var item in intros)
        {
            item.SetActive(false);
        }
    }
    #endregion

    //#if UNITY_EDITOR
    #region DESCOMENTARLO PARA CUANDO SE TENGAN MAS INTROS
    //private void OnValidate()
    //{
    //    desactiveIntros();
    //    if (app == apps.Arvispace)
    //    {
    //        intros[0].SetActive(true);
    //    }
    //}
    #endregion
}

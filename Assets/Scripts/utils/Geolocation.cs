using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Android;
using System;


[SerializeField]
public class GeolocationData //Clase referente a los datos necesarios para la geolocali
{
    public float lat;
    public float lng;
}

public class Geolocation : MonoBehaviour
{
    public static Geolocation sharedInstance;

    #region Variables privadas serializadas
    [Header("Variables de visualizacion")]
    public bool isLocate; //boooleano que indica si está localizado

    public bool isAsked; //booleano que indica si se han otorgado los permisos de ubicación

    public bool tryLocate = false; //booleano que indica si se intenta localizar

    public bool troubleToLocate = false;// booleano que indica si existe un problema al localizar
    #endregion

    #region Variables privadas
    private GeolocationData latlng = new GeolocationData();// Referencia a la clase GeolocationData
    private float timer;
    private float limitToRequest = 5;
    private int maxWait = 10;
    private bool locationServiceStarted = false;
    #endregion

    #region Obtener valores de variables privadas
    public GeolocationData GetLating()
    {
        return latlng;
    }
    #endregion

    private void Awake()
    {
        sharedInstance = this;
        timer = 0;

#if UNITY_ANDROID || UNITY_IOS
        _ = GetLatLonUsingGPS();
#endif
    }

    private void Start()
    {
        RequestLocationPermission();
    }

    private void OnDestroy()
    {
        Input.location.Stop(); // Detener el servicio de ubicación al cerrar la aplicación
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Verificar la ubicación si no está localizado
        if (!isLocate)
        {
            // Verificar permisos
            if (!isAsked && !Input.location.isEnabledByUser)
            {
                //isAsked = true;
                RequestLocationPermission();
            }
            // Iniciar la geolocalización si los permisos están concedidos y no se ha intentado antes
            else if (/*Permission.HasUserAuthorizedPermission(Permission.FineLocation) && */!tryLocate && !locationServiceStarted)
            {
                StartCoroutine(InitializeLocationService());
            }

            // Manejar el tiempo de espera si los permisos fueron solicitados
            if (isAsked && /*!Permission.HasUserAuthorizedPermission(Permission.FineLocation)&&*/ timer < limitToRequest)
            {
                timer += Time.deltaTime;
            }
            // Reiniciar si el tiempo de espera ha pasado
            else if (isAsked && !/*Permission.HasUserAuthorizedPermission(Permission.FineLocation)*/!isLocate && timer > limitToRequest)
            {
                isAsked = false;
                tryLocate = false;
                timer = 0;
            }
        }
#endif
    }
    #region Private Methods / Coroutines
    void RequestLocationPermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            isAsked= true;
        }
#endif
#if UNITY_IOS
        isAsked = true;
#endif
    }

    IEnumerator InitializeLocationService()
    {
        locationServiceStarted = true;
        yield return new WaitForSeconds(1);  // Puedes ajustar el tiempo según sea necesario
#if UNITY_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
#endif
        {
            Input.location.Start();
            _ = GetLatLonUsingGPS();
        }
    }

    void Retry()//---------------------------------------------------------------Verificar si se puede eliminar------------------------------------------------------------------------[1]
    {
        _ = GetLatLonUsingGPS();
    }

    void RequestActive()//--------------------------------------------------------------Verificcar si es necessario-------------------------------------------------------------------[2]
    {
        MessagesControllers.sharedInstance.CreateAnswer("Cuidado", "Active la geolocalización del dispositivo por favor", () => { Application.Quit(); },
            () =>
            {
                MessagesControllers.sharedInstance.CloseAnswer();
                _ = GetLatLonUsingGPS();
            }
        );
    }

    async UniTaskVoid geolocation()//---------------------------------------------Verificar si es necesario......................................................................[3]
    {
        // Resto del código de geoLocation...
        //   Debug.Log("entra a hacer la geolocalitation con: "+ Input.location.lastData.latitude+" --- "+ Input.location.lastData.longitude);
#if UNITY_EDITOR

        isLocate = true;

#else
        tryLocate = true;
        if (!Input.location.isEnabledByUser)
        {
            RequestActive();
            troubleToLocate = true;
            return;
        }
        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            maxWait--;
        }

        if (maxWait < 1)
        {
            troubleToLocate = true;
            tryLocate = false;
            print("Timed out");
            Retry();
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            troubleToLocate = true;
            tryLocate = false;
            print("Unable to determine device location");
        }
        else
        {
            isLocate = true;
            latlng.lat = Input.location.lastData.latitude;
            latlng.lng = Input.location.lastData.longitude;
            //FindObjectOfType<MessagesControllers>().createNotification("","Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp,5,MessagesControllers.iconsNotifications.success);
        }
        Input.location.Stop();
#endif
    }

    private IEnumerator GetLatLonUsingGPS()//---------------------------------------------Verificar su uso / funcionamiento--------------------------------------------------------------------[4]
    {
        // Resto del código de GetLatLonUsingGPS...
        Debug.Log("entra a conseguir las coordenadas desde gps");

#if UNITY_EDITOR
        // Debug.Log("es editor");
        isLocate = true;
        yield return true;
#endif
   
       // Debug.Log("no es editor");

        tryLocate = true;
        Input.location.Start();
        int maxWait = 5;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            Debug.Log("maxwait");
            //statusTxt.text = "Failed To Iniyilize in 10 seconds";
            troubleToLocate = true;
            tryLocate = false;
            print("Timed out");
            Retry();
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed)//no se le ha dado permisos al usuario para la aplicacion
        {
           Debug.Log("no se le ha dado permisos al usuario");
             troubleToLocate = true;
            tryLocate = false;
          //  statusTxt.text = "Failed To Initialize";
            yield break;
        }
        else
        {  //ya se le dio los permisos y empieza capturar las coordenadas
           Debug.Log("ya se le dio permisos al usuario");
           // statusTxt.text = "waiting before getting lat and lon";
            // yield return new WaitForSeconds(5);
            // Access granted and location value could be retrieve
            
            RequestActive();
            troubleToLocate = true;

            isLocate = true;
            latlng.lat = Input.location.lastData.latitude;
            latlng.lng = Input.location.lastData.longitude;

            double longitude = Input.location.lastData.longitude;
            double latitude = Input.location.lastData.latitude;

          //  AddLocation(latitude, longitude);
            Debug.Log( "consiguio las coordenadas" + Input.location.status + "  lat:" + latitude + "  long:" + longitude);
        }
        //Stop retrieving location
        Input.location.Stop();
        StopCoroutine("Start");                                                                                   
                                                     

    }
#endregion
}
using UnityEngine;
using System.Collections;

public class DeepLinkController : MonoBehaviour
{
    public static DeepLinkController sharedInstance
    {
        get; private set;   
    }

    private string _deeplinkURL;

    


    //Propiedades
    public string DeepLinkURL
    {
        get{ return _deeplinkURL;}
        set{ _deeplinkURL = value;}
    }


    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
            Application.deepLinkActivated += onDeepLinkActivated; //El evento onDeepLinkActivated
                                                                  //se genera cuando alguien ingresa a la app 
                                                                  //con un enlace profundo

            if (!string.IsNullOrEmpty(Application.absoluteURL))//La URL se encuentra
                                                               //almacenada dentro de la aplicacion
            {
                //Se procesa el enlace profundo si la url no es nula
                onDeepLinkActivated(Application.absoluteURL);
            }
            //Se inicializa la variable global privada
            else _deeplinkURL = "[none]";
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {

    }

    //La aplicacion no se esta ejecutando:
    //el dispositivo abre la aplicacion y Application.absoluteURL
    //almacena la url que el mismo dispositivo envia
    private void onDeepLinkActivated(string url)
    {
        _deeplinkURL = url;

        _deeplinkURL.Split('/')[0].Trim();

       // HandleDeepeLink
    }
    /// <summary>
    /// Manipulacion de enlace profundo
    /// </summary>
    private void HandleDeepLink(string url)
    {   

        //recepcion del token
        //.Split('')[1];
    }

    private void ExtractParametersFromURL(string url) //extraer datos de usuario, idToken, AccessToken
    {
        var query = new System.Uri(url).Query;

        foreach (var param in query) 
        {

        
        
        }

        //Comunicacion con FirebaseManager
        //FirebaseManager firebaseManager = new FirebaseManager();
        //firebaseManager.SignInGoogle(googleIdToken, googleAccessToken);
    }
}

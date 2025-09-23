
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Account{
    //Usuario testing
    //content.init("testing","1",'{"nombre":"Ivan Villegas rojas","correo":"villegas.rojas.ivan@gmail.com","telefono":"2211620123","postal_code":"90796"}')
    public string nombre;
    public string correo;
    public string telefono;
    public string postal_code;
    public bool logged;
    public Configuration configuration;
}
[Serializable]
public class Usuario
{    
    //Campos privados
    private string email;
    private string userName;
    private string fullName;
    private string phoneNumber;
    private string password;
    
    //Propiedades de solo lectura y escritura
    public string Email { get => email; set => email = value; }
    public string UserName { get => userName; set => userName = value; }
    public string FullName { get => fullName; set => fullName = value; }
    public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
    public string Password { get => password; set => password = value; }

    //private void Awake()
    //{
    //    if (sharedInstance)
    //        Destroy(this);
    //    else
    //        sharedInstance = this;
    //    // Intenta iniciar sesion con los datos que se guardaron en PlayerPrefs.
    //    Login(
    //        PlayerPrefs.GetString("correo"),
    //        PlayerPrefs.GetString("userName"),
    //        PlayerPrefs.GetString("nombreCompleto"),
    //        PlayerPrefs.GetString("telefono"),
    //        PlayerPrefs.GetString("contraseña")
    //    );
    //    //Si estamos en el editor, inicia sesion con estos datos.
    //    #if UNITY_EDITOR
    //    Login(
    //        //"villegas.rojas.ivan@gmail.com",
    //        //"Ivan Villegas Rojas",
    //        //"Ivan Villegas Rojas",
    //        //"2211620123",
    //        //"haloreach547"

    //        //"", //=>Sin inicio    de sesion
    //        //"",
    //        //"",
    //        //"",
    //        //""

    //        //"alberto7003@gmail.com", => USUARIO SIN FAVORITOS
    //        //"BetoMaster",
    //        //"Alberto Corona Sanchez",
    //        //"2221345099",
    //        //"12345678"

    //        "anyataylorjoy@gmail.com", //=>USUARIO CON FAVORITOS
    //        "Anya Mendoza Joy",
    //        "Anya Mendoza Joy",
    //        "2221245678",
    //        "123456789"

    //    );
    //    #endif
    //    //cerrarSesion();
    //}
    ///// <summary>
    ///// Borra la ultima Sesión almacenada
    ///// </summary>
    //public void DeleteLastSesion() {
    //    PlayerPrefs.SetString("correo", "");
    //    PlayerPrefs.SetString("userName", "");
    //    PlayerPrefs.SetString("nombreCompleto", "");
    //    PlayerPrefs.SetString("telefono", "");
    //    PlayerPrefs.SetString("contraseña", "");
    //}

    ///// <summary>
    ///// Guarda la informacion de la sesion actual
    ///// </summary>
    //public void SaveLastSession() {
    //    PlayerPrefs.SetString("correo", this.correo);
    //    PlayerPrefs.SetString("userName", this.userName);
    //    PlayerPrefs.SetString("nombreCompleto", this.nombreCompleto);
    //    PlayerPrefs.SetString("telefono", this.telefono);
    //    PlayerPrefs.SetString("contraseña", this.contraseña);
    //}

    ///// <summary>
    ///// Verifica si todos los campos de la Sesion estan llenos
    ///// </summary>
    ///// <returns></returns>
    //public bool CheckSesion() {
    //    if (
    //        this.correo.Equals("") ||
    //        this.UserName.Equals("") ||
    //        this.correo.Equals("") ||
    //        this.telefono.Equals("") ||
    //        this.contraseña.Equals(""))
    //    {
    //        // Si falta algun dato, la sesion no es valida.
    //        return false;
    //    }
    //    else {
    //        // Todos los datos estan completos.
    //        return true;
    //    }
    //}
    ///// <summary>
    ///// Abre el webview del perfil o login dependiendo del estado de la sesion
    ///// </summary>
    //public void OpenProfile() {

    //    var webView1 = gameObject.AddComponent<UniWebView>();

    //    if (config.sharedInstance.mUseMode == config.useMode.production)
    //    {
    //        //Debug.Log("produccion perfil");
    //        if (CheckSesion())
    //        {
    //            //Debug.Log("cuenta perfil");
    //            // WebViewController.sharedInstance.initnewview(webView1);
    //            WebViewController.sharedInstance.ShowWebView(webView1, config.sharedInstance.urlArviSpaceForms("account",config.localFiles.yes),1);
    //        }
    //        else
    //        {
    //            //Debug.Log("login perfil");
    //            // WebViewController.sharedInstance.initnewview(webView1);
    //            WebViewController.sharedInstance.ShowWebView(webView1,config.sharedInstance.urlArviSpaceForms("login",config.localFiles.yes),1);//correjir login para vue js
    //        }
    //    }
    //    else {
    //        //Debug.Log("testing perfil");
    //        if (CheckSesion())
    //        {
    //           // WebViewController.sharedInstance.initnewview(webView1);
    //            WebViewController.sharedInstance.ShowWebView(webView1,config.sharedInstance.urlArviSpaceForms("account",config.localFiles.yes),1);
    //        }
    //        else
    //        {
    //           // WebViewController.sharedInstance.initnewview(webView1);
    //            WebViewController.sharedInstance.ShowWebView(webView1,config.sharedInstance.urlArviSpaceForms("login",config.localFiles.yes),1);//correjir login para vue js
    //        }
    //    }
    //}

    ///// <summary>
    ///// Iniciar Sesion
    ///// </summary>
    ///// <param name="correo"></param>
    ///// <param name="userName"></param>
    ///// <param name="nombreCompleto"></param>
    ///// <param name="telefono"></param>
    ///// <param name="contraseña"></param>
    ///// <returns></returns>
    //public string Login(string correo, string userName, string nombreCompleto, string telefono,string contraseña) {

    //    //gerardodj.garcia@gmail.com
    //    //123456789
    //    //Debug.Log("empieza el inicia sesion");
    //    this.correo = correo;
    //    this.userName = userName;
    //    this.nombreCompleto = nombreCompleto;
    //    this.telefono = telefono;
    //    this.contraseña = contraseña;
    //    if (PlayerPrefs.GetInt("_sesion") == 1)
    //    {
    //        //Debug.Log("va a asignar los datos");
    //        PlayerPrefs.SetString("correo", Correo);
    //        PlayerPrefs.SetString("userName", UserName);
    //        PlayerPrefs.SetString("nombreCompleto", NombreCompleto);
    //        PlayerPrefs.SetString("telefono", Telefono);
    //        PlayerPrefs.SetString("contraseña", contraseña);
    //    }
    //    return this.correo + this.userName + this.nombreCompleto + this.telefono +this.contraseña;
    //}
    ///// <summary>
    ///// Cerrar Sesion
    ///// </summary>
    ///// <returns></returns>
    //public string LogOut()
    //{
    //    this.correo = "";
    //    this.userName = "";
    //    this.nombreCompleto = "";
    //    this.telefono = "";
    //    this.contraseña = "";
    //    PlayerPrefs.SetString("correo", "");
    //    PlayerPrefs.SetString("userName", "");
    //    PlayerPrefs.SetString("nombreCompleto", "");
    //    PlayerPrefs.SetString("telefono", "");
    //    PlayerPrefs.SetString("contraseña", "");
    //    return "Cerramos sesión";
    //}
}

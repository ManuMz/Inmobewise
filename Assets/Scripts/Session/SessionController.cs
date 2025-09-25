using System;
using UnityEngine;

public class SessionController : Singleton<SessionController> //unica instancia del objeto
{
    //public static SessionController sharedInstance;

    #region Delegados de Eventos
    //cada evento nativo requiere un delegado

    //public delegate void ChangeUserState();//-> delegado de firma libre
    public delegate void EventHandler();// ->delegado dado de manera nativa(.Net)
                                        //usa una firma estandar
    #endregion

    #region Eventos
    //Eventos nativos C#

    //public event ChangeUserState OnChangeUserState; //evento establecido de manera libre

    public event EventHandler<User> OnUserStateChanged; //Evento usando EventHandler
                                                        //con la clase User que proporciona
                                                        //los datos para el evento
 
    #endregion
    private void Awake()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SessionController es el Publisher de los eventos
        OnUserStateChanged?.Invoke(this, new User ()); //Invocacion del evento
    }
    
    void Update()
    {
        
    }

   

    private void OnDestroy()
    {
        
    }

}

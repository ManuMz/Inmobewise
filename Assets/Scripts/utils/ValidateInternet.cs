using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ValidateInternet : MonoBehaviour
{
    #region Variables privadas serializadas
    [SerializeField] 
    private MessagesControllers messages;
    #endregion

    #region Variables privadas
    [Header("Visualizacion de variable")]
    [SerializeField]private bool isNotificated;
    [SerializeField]
    private LoadingWifiController loadingWifiController;
    #endregion

    private void Awake()
    {
        isNotificated = false; //Indica que el usuario no será notificado hasta que despliegue la app y realice la conexion a internet 
    }
    void Update()
    {
        if (!isNotificated) // el boooleano cambiara de acuerdo al estado de conexion a internet
        {
            if (Validate())
            {
                isNotificated = false;
                //loadingWifiController.gameObject.SetActive(false);
                loadingWifiController.CloseWiFi();
            }
            else
            {
                isNotificated = true;
                messages.CreateAnswer("Error de red", "No se pudo establecer una conexión a internet la aplicación va a reiniciarse.",
                    () => { messages.CloseAnswer(); StartCoroutine(DelayToValidate()); },
                    () => { messages.CloseAnswer(); StartCoroutine(DelayToValidate()); }
                );
                //loadingWifiController.gameObject.SetActive(true);
                loadingWifiController.OpenWiFi();
            }
        }
    }

    #region Private Methods / Coroutines
    /// <summary>
    /// Verifica el acceso a la conexion de internet
    /// </summary>
    /// <returns></returns>
    public bool Validate()
    {
        bool flag = true;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            flag = false;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            flag = true;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            flag = true;
        }
        return flag;
    }
    /// <summary>
    /// Ejecuta una escena de carga/espera mientras se valida la conexion a internet
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayToValidate() {
        yield return new WaitForSeconds(1f);
        isNotificated = false;
        //SceneManager.LoadScene("init");
    }
    #endregion
}

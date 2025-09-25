using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModalController : MonoBehaviour //Controlador de Modal
{

    //private UnityEvent event; evento por activar
    
    [SerializeField]
    private AlertType alertType; //identificador del objeto que recibira el
                                 //trigger

    //eventos del Modal
    IEnumerator OnEnableUIAlert()
    {
        yield return null;
    }

}

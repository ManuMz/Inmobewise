using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RelocatePlayer : MonoBehaviour
{
    public static RelocatePlayer sharedInstance;

    #region Variables privadas 
    [Tooltip("Arreglo de puntos de spawn")]
    [SerializeField]
    private Transform[] spawnPoints; //puntos de referencia respecto a la posición inicial del jugador
    [Tooltip("player")]
    [SerializeField]
    private GameObject player; // objeto Jugador
    [Tooltip("joystic de player")]
    [SerializeField]
    private GameObject[] joystick; // objeto Joystick, que dirige el movimiento del jugador (giro de camara)
    [Tooltip("Script de movimiento de player")]
    [SerializeField]
    private InputController inputController; // variable de referencia de la clase InputController
    [Tooltip("Padre de los inmuebles")]
    [SerializeField]
    private GameObject buildingsFather; //objeto padre donde se instanciará cada inmueble
    private bool startPos = false;//booleano para permitir las funciones del jugador
    #endregion

    private void Awake()
    {
        if (!sharedInstance)
            sharedInstance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        PosInitial(); // Se ejecuta el metodo desde que la clase esta habilitada
    }
    private void Update()
    {
        if (startPos)
        {
            inputController.enabled = false; // deshabilita el script InputController.cs
            StartCoroutine(EnableInput());
            startPos = false;
        }

        if (buildingsFather.transform.childCount > 0)
        {
            inputController.enabled = true; // Habilita el script InputController.cs si existen instancias(hijos) en el objeto buildingFather
        }
        else
        {
            inputController.enabled = false; // deshabilita el script InputController.cs
        }
    }

    #region Private Coroutines
    /// <summary>
    /// Corutina que habilita el controlador de movimiento del jugador
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnableInput()
    {
        if (!inputController.enabled)//Deshabilitado
        {
            yield return new WaitForSeconds(0.1f);
            inputController.enabled = true;//Habilitado
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Reubica al jugador en la posición dada por los spawnpoints
    /// </summary>
    /// <param name="spawnIndex"></param>
    public void Relocate(int spawnIndex) 
    {
        for (int i = 0; i <= spawnPoints.Length; i++)
        {
            if (i == spawnIndex)
            {
                player.transform.position = spawnPoints[spawnIndex].position;//posición de los puntos de spawneo               
                player.transform.rotation = spawnPoints[spawnIndex].rotation;//rotación de los puntos 
            }
        }
    }

    /// <summary>
    /// Se establece la posición inicial del jugador en el entorno
    /// </summary>
    public void PosInitial()
    {
        startPos = true;

        player.transform.GetChild(2).rotation = player.transform.GetChild(1).transform.rotation;//asigna el valor del tercer hijo del player a la rotación de la cámara, la rotacion siempre sea 0
        
        player.transform.position = spawnPoints[1].position;//la posicion del jugador sea 0
        player.transform.rotation = spawnPoints[1].rotation;//la rotacion del jugador sea 0
        //Debug.Log("entre a pos inicial");
    }
    #endregion
}

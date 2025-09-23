using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DataPersistanceManager : MonoBehaviour
{
    #region Patron Singleton
    public static DataPersistanceManager sharedInstance { get; private set; }
    #endregion

    private ProductoData productoData;
    private ProductosDataCollection productosDataCollection;
    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;
    private string correo;
    private string buildingCode;
    private void Awake()
    {
        if(sharedInstance!= null)
        {
            Debug.LogError($"Error sharedInstance en DataPersistanceManager");
        }
        
        sharedInstance = this; 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath,correo, buildingCode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NewProductCollection()
    {
        this.productoData = new ProductoData();
    }
    public void SaveProductCollection()
    {

    }

    public void LoadProductCollection()
    {
         if(this.productoData == null)
        {
            Debug.Log("Producto no encontrado");
            NewProductCollection();
        }
    }

    private List<IDataPersistence> ProductDataCollection()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects =  FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();



        return new List<IDataPersistence>(dataPersistenceObjects); 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBuildingsView : MonoBehaviour
{
    #region Variables privadas
    [Header("SCROLES DE MENU INMUEBLES")]
    [SerializeField] private ScrollMenuController m_scrollToProductsView;

    [Header("CONTENEDORES INMUEBLES")]
    [SerializeField] private GameObject conteBuildingsView;

    [Header("ID DE CATEGORIA SELECCIONADA VISUALIZACION")]
    [SerializeField] private int id_category;
  

    [SerializeField] private QueriesController m_queriesController;
    [SerializeField] private SaveComponentsController m_saveComponentsController;   
    [SerializeField] private DownloadComponentController downloadComponentController;
    [SerializeField] private DownloadModel downloadModel;

    [Header("CONTENEDORES")]
    [SerializeField] private GameObject prefabToProductBuildingView;

    [SerializeField] private GameObject relocatePlayer;
    [SerializeField] private GameObject fatherBuilding;
    [SerializeField] private GameObject fatherProducts;
    [SerializeField] private InputField inputBuildingCode;
    #endregion


    /// <summary>
    /// Instancia todos los inmuebles
    /// </summary>
    public void InstatiateProductsBuildinigsView()
    {
        Clear(m_scrollToProductsView.m_container);

        foreach (var item in m_queriesController.GetAllBuildingsView())
        {
            GameObject producto = Instantiate(prefabToProductBuildingView, m_scrollToProductsView.m_container.transform);
            producto.GetComponent<ContainerInmuebleVisualizer>().building = item;
        }

        m_scrollToProductsView.onChangeContent();
        m_scrollToProductsView.ShowElements();
    }

    /// <summary>
    /// Buscador de ID  de inmuebles
    /// </summary>
    /// <param name="inputValue"></param>
    public void Search_Inmueble_Id(string inputValue)
    {
        if (!m_queriesController.ExistBuilding(inputValue)) 
        {
            Clear(fatherProducts);//Limpia Padre de los Productos
            ClearInputBuildingCode(inputBuildingCode); //Limpieza del input donde se ingresa el codigo inmueble
            MessagesControllers.sharedInstance.CreateNotification("Error", "Inmueble no encontrado", 3f, MessagesControllers.iconsNotifications.error);
            return;
        }

        m_queriesController.GetBuildingIdByBuildingCode(inputValue);
        int buildingId = int.Parse(m_queriesController.GetBuildingId());
        m_saveComponentsController.StartToLoadDefaultData(buildingId,inputValue);
    }

    /// <summary>
    /// Destruye los hijos del parametro.
    /// </summary>
    /// <param name="containers"></param>
    public static void Clear(params GameObject[] containers)
    {
        foreach (var item in containers)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                Destroy(item.transform.GetChild(i).gameObject);
            }
        }
    }

    public static void ClearInputBuildingCode(InputField input)
    {
        //Text inputComponentText = input.GetComponent<Text>();
        input.text = string.Empty;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class MenuBuildings : MonoBehaviour
{
    #region Variables Privadas
    [Header("SCROLES DE MENU INMUEBLES")]
    [SerializeField] private ScrollMenuController m_scrollToCategoriesBuildings;
    [SerializeField] private ScrollMenuController m_scrollToSubCategoriesBuildings;
    [SerializeField] private ScrollMenuController m_scrollToProducts;

    [Header("CONTENEDORES CATEGORIAS Y SUBCATEGORIAS")]
    [SerializeField] private GameObject conteCategoriasInmueble;
    [SerializeField] private GameObject conteSubCategoriasInmueble;
    [SerializeField] private GameObject conteInmuebles;

    [Header("ID DE CATEGORIA SELECCIONADA VISUALIZACION")]
    [SerializeField] private int id_category;
    private string input;

    [SerializeField] private QueriesController queries;
    [SerializeField] private DownloadComponentController downloadComponentController;
    [SerializeField] private DownloadModel downloadModel;

    [Header("CONTENEDORES")]
    [SerializeField] private GameObject prefabToCategoryBuilding;
    [SerializeField] private GameObject prefabToSubcategoryBuilding;
    [SerializeField] private GameObject prefabToProductBuilding;

    [SerializeField] private GameObject relocatePlayer;
    [SerializeField] private GameObject fatherBuilding;
    [SerializeField] private GameObject fatherProducts;
    #endregion

    /// <summary>
    /// Instancias categorias de inmuebles 
    /// </summary>
    public void InstatiateCategoriesBuildings()
    {
        Clear(m_scrollToCategoriesBuildings.m_container);

        foreach (var item in queries.GetCategoryBuilding())
        {
            GameObject categoryBuilding = Instantiate(prefabToCategoryBuilding, m_scrollToCategoriesBuildings.m_container.transform);
            categoryBuilding.GetComponent<ContainerCategoriaInmueble>().categoryBuilding = item;
        }
        m_scrollToCategoriesBuildings.onChangeContent();
        m_scrollToCategoriesBuildings.ShowElements();
        queries.GetAllBuildingsView();
    }
    
    /// <summary>
    /// Instancia las subcategorias de inmuebles
    /// </summary>
    /// <param name="category">Categoria seleccionada</param>
    public void InstatiateSubCategoriesBuildings(CategoriaInmueble category)
    {
        Clear(m_scrollToSubCategoriesBuildings.m_container, m_scrollToProducts.m_container);

        foreach (var item in queries.GetSubCategoryByCategoryBuilding(category.Id_Tipo_Publicacion))
        {
            id_category = category.Id_Tipo_Publicacion;
            GameObject subCategory = Instantiate(prefabToSubcategoryBuilding, m_scrollToSubCategoriesBuildings.m_container.transform);
            subCategory.GetComponent<ContainerSubCategoriaInmueble>().tipoinmueble = item;
        }

        m_scrollToSubCategoriesBuildings.onChangeContent();
        m_scrollToSubCategoriesBuildings.ShowElements();

        m_scrollToSubCategoriesBuildings.ShowElements01();

    }

    /// <summary>
    /// Instancia los productos recibidos del query
    /// </summary>
    /// <param name="subCategory">clase subcategoria que es seleccionada por el boton</param>
    public void InstatiateProductsBuildinigs(Tipo_Inmueble subCategory)
    {
        int numm = 0;

        Clear(m_scrollToProducts.m_container);

        foreach (var item in queries.GetProductBySubCategoryAndCategory(id_category,  subCategory.Id_Tipo_Inmueble))
        {
            GameObject producto = Instantiate(prefabToProductBuilding, m_scrollToProducts.m_container.transform);
            producto.GetComponent<ContainerInmueble>().buildings = item;

            numm++;
        }

        m_scrollToProducts.onChangeContent();
        m_scrollToProducts.ShowElements();
    }

    
    /// <summary>
    /// Buscador de ID  de inmuebles
    /// </summary>
    /// <param name="inputValue"></param>
    public void Search_Inmueble_Id(string inputValue)
    {
    //    input = inputValue;
    //    //queries.GetBuildingByInputField(input);

    //    if (queries.GetFoundBuildingByCode() == true)//Si el codigo fue encontrado
    //    {
    //        UIController.sharedInstance.HideMenuBuildings();//Oculta el Menu de los Inmuebles
    //        relocatePlayer.GetComponent<RelocatePlayer>().PosInitial();
    //        Clear(fatherBuilding);//Limpia Padre de los inmuebles
    //        Clear(fatherProducts);//Limpia Padre de los Productos

    //        downloadModel.StartDownloadBuildingCode(input);//Comienza la descarga del INMUEBLE depende del ID introducido
    //    }
    //    else
    //    {
    //        MessagesControllers.sharedInstance.CreateNotification("NOTIFICACION", "No se encontro el ID", 3f, MessagesControllers.iconsNotifications.error);
    //    }
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
}

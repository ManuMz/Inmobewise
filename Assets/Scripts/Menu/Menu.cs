using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class Menu : MonoBehaviour
{
    #region Variables Publicas
    #endregion

    #region Variables Privadas
    [Header("SCROLLES DE MENU")]
    [SerializeField] private ScrollMenuController m_scrollToCategories;
    [SerializeField] private ScrollMenuController m_scrollToSubCategories;
    [SerializeField] private ScrollMenuController m_scrollToProducts;

    [SerializeField] private ScrollMenuController m_scrollToProducts2;

    [SerializeField] private QueriesController queriesController;

    //Asignacion de Prefabs o Contenedores
    [SerializeField] GameObject prefabToCategory;
    [SerializeField] GameObject prefabToSubcategory;
    [SerializeField] GameObject prefabToProduct;

    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Instancias categorias 
    /// </summary>
    public void InstatiateCategories()
    {
        Clear(m_scrollToCategories.m_container);
        foreach (var item in queriesController.GetCategories())
        {
            GameObject category = Instantiate(prefabToCategory, m_scrollToCategories.m_container.transform);
            category.GetComponent<ContainerCategoria>().category = item;
        }  
        m_scrollToCategories.onChangeContent();
        m_scrollToCategories.ShowElements();
   
    }

    /// <summary>
    /// Instancia las subcategorias
    /// </summary>
    /// <param name="category">Categoria seleccionada</param>
    public void InstatiateSubCategories(Categoria category)
    {
        Clear(m_scrollToSubCategories.m_container, m_scrollToProducts.m_container, m_scrollToProducts2.m_container);

       
            foreach (var item in queriesController.GetSubCategoryByCategory(category.idCategoria))
            {
                GameObject subCategory = Instantiate(prefabToSubcategory, m_scrollToSubCategories.m_container.transform);
                subCategory.GetComponent<ContainerSubCategoria>().subCategory = item;
            }

            m_scrollToSubCategories.onChangeContent();
            m_scrollToSubCategories.ShowElements();

            m_scrollToSubCategories.ShowElements01();
    
    }

    /// <summary>
    /// Instancia los productos recibidos del query
    /// </summary>
    /// <param name="subCategory">clase subcategoria que es seleccionada por el boton</param>
    public void InstatiateProducts(SubCategoria subCategory)
    {
        //Debug.Log("Entre a instanciar los productos con mi subcategoria: " + subCategory);
        Clear(m_scrollToProducts.m_container, m_scrollToProducts2.m_container);

          foreach (var item in queriesController.GetProductoBySubCategory(subCategory.idProducto))
          {
                GameObject producto = Instantiate(prefabToProduct, m_scrollToProducts.m_container.transform);
                producto.GetComponent<ContainerProducto>().product = item;

                GameObject producto2 = Instantiate(prefabToProduct, m_scrollToProducts2.m_container.transform);
                producto2.GetComponent<ContainerProducto>().product = item;
          }

            m_scrollToProducts.onChangeContent();
            m_scrollToProducts.ShowElements();
            m_scrollToProducts2.onChangeContent();
       
    }

    /// <summary>
    /// Destruye los hijos del parametro
    /// </summary>
    /// <param name="containers"></param>
    void Clear(params GameObject[] containers)
    {
        foreach (var item in containers)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                Destroy(item.transform.GetChild(i).gameObject);
            }
        }
    }


    public void ClearAll()
    {
        Clear(m_scrollToCategories.m_container,
            m_scrollToSubCategories.m_container, m_scrollToProducts.m_container, m_scrollToProducts2.m_container);
    }
    public void ClearSubcategoriesAndProducts()
    {
        Clear(m_scrollToSubCategories.m_container, m_scrollToProducts.m_container, m_scrollToProducts2.m_container);
    }
}

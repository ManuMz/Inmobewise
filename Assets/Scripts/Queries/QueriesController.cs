using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.Windows;
using UnityEngine.Rendering.LookDev;
using JetBrains.Annotations;

public class QueriesController : MonoBehaviour
{
    public static QueriesController sharedInstance;
    [SerializeField] private DownloadComponentController m_downLoadComponentController;
    
    private string buildingId;
    private string buildingCode;
    private string buildingName; 
    private bool existProduct;
    public string GetBuildingId()
    {
        return buildingId;
    }
    public string GetBuildingCode()
    {
        return buildingCode;
    }

    public string GetBuildingName()
    {
        return buildingName;
    }
    #region QUERIES DE LOS INMUEBLES

    /// <summary>
    /// Retorna las categoriias de los inmuebles
    /// </summary>
    /// <returns></returns>
    public List<CategoriaInmueble> GetCategoryBuilding()
    {
        List<CategoriaInmueble> categoriesBuildings = new List<CategoriaInmueble>();

        if (m_downLoadComponentController.componentsBuildings.Count > 0)
        {
            foreach (var item in m_downLoadComponentController.componentsBuildings)
            {
                if (categoriesBuildings.Where(el => el.Id_Tipo_Publicacion == item.Id_Tipo_Publicacion).Count() <= 0)
                {
                    categoriesBuildings.Add(item);
                }
            }
        }
        return categoriesBuildings;
    }

    /// <summary>
    /// Retorna las subcategorias por el id del inmueble
    /// </summary>
    /// <param name="idCategory"></param>
    /// <param name="brandSelected"></param>
    /// <returns></returns>
    public List<Tipo_Inmueble> GetSubCategoryByCategoryBuilding(int idCategory)
    {
        List<Tipo_Inmueble> subCategories = new List<Tipo_Inmueble>();
        if (m_downLoadComponentController.componentsBuildings.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.componentsBuildings)
            {
                if (category.Id_Tipo_Publicacion.Equals(idCategory))
                {
                    foreach (var subCategory in category.Tipo_Inmueble)
                    {
                        if (subCategories.Where(el => el.Id_Tipo_Inmueble == subCategory.Id_Tipo_Inmueble).Count() <= 0)
                        {
                            subCategories.Add(subCategory);
                        }
                    }

                }
            }
        }
        return subCategories;
    }

    /// <summary>
    /// Retorna la lista de los productos inmuebles por su id de subcategoria y su id de producto
    /// </summary>
    /// <param name="idSubCategoria"></param>
    /// <param name="brandSelected"></param>
    /// <returns></returns>
    /// <summary>
    /// Get list of productos de inmuebles by id Subcategory
    /// </summary>
    /// <param name="idSubCategoria"></param>
    /// <param name="brandSelected"></param>
    /// <returns></returns>
    public List<Inmuebles> GetProductBySubCategoryAndCategory(int idCategory, int idSubcategory)
    {
        List<Inmuebles> productsBuildings = new List<Inmuebles>();
        if (m_downLoadComponentController.componentsBuildings.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.componentsBuildings)
            {
                if (category.Id_Tipo_Publicacion.Equals(idCategory))
                {
                    foreach (var subCategory in category.Tipo_Inmueble)
                    {
                        if (subCategory.Id_Tipo_Inmueble.Equals(idSubcategory))
                        {
                            foreach (var item in subCategory.Inmuebles)
                            {
                                if (productsBuildings.Where(el => el.Id_Inmueble == item.Id_Inmueble).Count() <= 0)
                                {
                                    item.tipo_Inmueble = subCategory;
                                    productsBuildings.Add(item);
                                }
                            }
                        }
                    }
                }
            }
        }
        return productsBuildings;
    }
    /// <summary>
    /// Retorna todos los inmuebles
    /// </summary>
    /// <returns></returns>
    public List<Inmuebles> GetAllBuildingsView()
    {
        List<Inmuebles> productsBuildings = new List<Inmuebles>();

        if (m_downLoadComponentController.componentsBuildings.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.componentsBuildings)
            {
                foreach (var subCategory in category.Tipo_Inmueble)
                {
                    foreach (var item in subCategory.Inmuebles)
                    {
                        // Verificamos si el producto ya ha sido añadido para evitar duplicados
                        if (productsBuildings.Where(el => el.Id_Inmueble == item.Id_Inmueble).Count() <= 0)
                        {
                            item.tipo_Inmueble = subCategory; // Asociamos el tipo de inmueble
                            //Debug.Log("El inmueble que voy a retornar es: " + item.Nombre_Inmueble);
                            productsBuildings.Add(item);      // Añadimos el producto
                        }
                    }
                }
            }
        }

        return productsBuildings;
    }

    public bool ExistBuilding(string param)
    {
      
        if (m_downLoadComponentController.componentsBuildings.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.componentsBuildings)
            {
                foreach (var subCategory in category.Tipo_Inmueble)
                {
                    foreach (var inmueble in subCategory.Inmuebles)
                    {   // verifica si el codigo del inmueble coincide con el buscado
                        if (inmueble.Codigo_Inmueble.Equals(param))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;// devuelve False si inmueble No es encontrado
    }

    public void GetBuildingIdByBuildingCode(string buildingCode)
    {
        if (m_downLoadComponentController.componentsBuildings.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.componentsBuildings)
            {
                foreach (var subCategory in category.Tipo_Inmueble)
                {
                    foreach (var inmueble in subCategory.Inmuebles)
                    {
                        if (inmueble.Codigo_Inmueble.Equals(buildingCode))
                        {
                            buildingId = inmueble.Id_Inmueble;
                        }
                    }
                }
            }
        }
    }

    #endregion


    #region QUERIES DE LOS PRODUCTOS
    /// <summary>
    /// Retorna todas las categorias de los productos
    /// </summary>
    /// <returns></returns>
    public List<Categoria> GetCategories()
    {
        List<Categoria> categories = new List<Categoria>();
        if (m_downLoadComponentController.components.Count > 0)
        {
            foreach (var item in m_downLoadComponentController.components)
            {
                if (categories.Where(el => el.idCategoria == item.idCategoria).Count() <= 0)
                {
                    categories.Add(item);
                }
            }
            this.existProduct = true;

        }
        else
        {
            //return null;
            this.existProduct = false;
        }
        return categories;
    }

    /// <summary>
    /// Retorna las subcategorias de los productos por el id de categoria
    /// </summary>
    /// <param name="idCategory"></param>
    /// <param name="brandSelected"></param>
    /// <returns></returns>
    public List<SubCategoria> GetSubCategoryByCategory(int idCategory)
    {
        List<SubCategoria> subCategories = new List<SubCategoria>();
        if (m_downLoadComponentController.components.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.components)
            {
                if (category.idCategoria.Equals(idCategory))
                {
                    foreach (var subCategory in category.SubCategorias)
                    {
                        if (subCategories.Where(el => el.idProducto == subCategory.idProducto).Count() <= 0)
                        {
                            subCategories.Add(subCategory);
                        }
                    }
                }
            }
        }
        return subCategories;
    }

    /// <summary>
    /// Retorna la lista de los productos por el id de subcategoria
    /// </summary>
    /// <param name="idSubCategory"></param>
    /// <param name="brandSelected"></param>
    /// <returns></returns>
    public List<Producto> GetProductoBySubCategory(int idSubCategory)
    {
        List<Producto> products = new List<Producto>();
        if (m_downLoadComponentController.components.Count > 0)
        {
            m_downLoadComponentController.components.ForEach(category => category.SubCategorias.ForEach(subCategory => {
                if (subCategory.idProducto.Equals(idSubCategory))
                {
                    foreach (var item in subCategory.Productos)
                    {
                        if (products.Where(el => el.idProBodPre == item.idProBodPre).Count() <= 0)
                        {
                            item.subCategoria = subCategory;
                            products.Add(item);
                        }
                    }
                }

            }));
        }
        return products;
    }

    /// <summary>
    /// obtencion de producto por su idProBodPre
    /// </summary>
    /// <param name="idProBodPre"></param>
    /// <returns></returns>
    public Producto GetProductByIdProBodPre(int idProBodPre)
    {
        Producto productByIdProBodPre = new();
        if (m_downLoadComponentController.components.Count > 0)
        {
            m_downLoadComponentController.components.ForEach(category => category.SubCategorias.ForEach(subCategory =>
            subCategory.Productos.ForEach(producto =>
            {
                if (producto.idProBodPre.Equals(idProBodPre))
                {
                    Debug.Log("Ingrese a product");
                    productByIdProBodPre = producto;
                }    
            })));
               
        }
        return productByIdProBodPre;
    }


    public List<Producto> GetAllProducts()
    {
        List<Producto>allProducts = new List<Producto>();
        var uniqueIds = new HashSet<int>();

        if (m_downLoadComponentController.components.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.components)
            {
                foreach (var subCategory in category.SubCategorias)
                {
                    foreach (var product in subCategory.Productos) 
                    {
                        if(uniqueIds.Add(product.idProBodPre))
                        {
                            allProducts.Add(product);
                        }
                    }
                }

            }
        }
        return allProducts;
    }

    /// <summary>
    /// Revisa si el producto existe en los datos descargadoss
    /// </summary>
    /// <param name="id">IdProBodPre del producto</param>
    /// <returns>Bool</returns>
    public bool CheckProduct(int id)
    {
        if (m_downLoadComponentController.components.Count > 0)
        {
            foreach (var category in m_downLoadComponentController.components)
            {
                foreach (var subCategory in category.SubCategorias)
                {
                    foreach (var item in subCategory.Productos)
                    {
                        if (item.idProBodPre.Equals(id))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    #endregion

    #region QUERIES DE REEMPLAZAR_PRODUCTO
    public SubCategoria getlastsubcatego(int probodpre, string idemprem, string nomsubcate)
    {
        SubCategoria sub01 = null;

        m_downLoadComponentController.components.ForEach(category => category.SubCategorias.ForEach(subCategory =>
        {
            foreach (var item in subCategory.Productos)
            {
                if (item.idEmpresa == idemprem && item.idProBodPre == probodpre && item.nombreProducto/*nombreSubcategoria*/ == nomsubcate)
                {
                    sub01 = subCategory;
                    //Debug.Log("La subcategoria que voy a devolver es: " + sub01);
                }
            }
        }));
        return sub01;
    }
    #endregion

}

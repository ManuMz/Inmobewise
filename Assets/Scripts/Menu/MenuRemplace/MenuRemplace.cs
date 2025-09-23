using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuRemplace : MonoBehaviour
{
    #region patrón de diseño Singleton
    public static MenuRemplace sharedInstance;
    #endregion

    #region Variables privada serializadas
    [SerializeField]
    private GameObject menuremplace; //variable que hace referencia al objeto menuremplace 

    [SerializeField]
    private Button closeButton; //variable que hace referencia al objeto closeButton 

    [SerializeField]
    private Menu menu; // variable con la cual podremos utilizar la clase Menu, requiere asignación del objeto que contiene la clase
    #endregion


    #region Variables Privadas
    //-----3 referencias de lastSubcategory en ObjectProperties.cs (lineas 247, 248, 249)-------------------------------------------------[1]
    public static SubCategoria lastSubcategory; // variable con la cual podremos tener acceso a la clase Subcategoria y sus componentes
    #endregion

    #region Obtener valores de variables privadas
    /*
    public static SubCategoria LastSubcategory()
    {
        return lastSubcategory; 
    }
    */
    #endregion

    #region Asignar Valor desde otro script a variables privadas
    /*
    public static void SetLastSubcategory(SubCategoria value)
    {
        lastSubcategory = value;
    }
    */
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ShowMenu();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            HideMenu();
        }
       
    }

    #region Private Methods
    /// <summary>
    /// Muestra el menu que se despliega al momento de cambiar el producto seleecionado por otro  
    /// </summary>
    public void ShowMenu()
    {
        menuremplace.GetComponent<RectTransform>().DOAnchorPosX(495, 1.5f);
        //Debug.Log("la ultima suncate: " + lastSubcategory.nombreProducto + " --- " + lastSubcategory.idCategoria + " --- " + lastSubcategory.idProducto);
        Debug.Log("El valor de la subcategoria enviado es: " + lastSubcategory);
        menu.InstatiateProducts(lastSubcategory);
    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Oculta el menu que se despliega al momento de cambiar el producto seleecionado por otro  
    /// </summary>
    public void HideMenu()
    {
        menuremplace.GetComponent<RectTransform>().DOAnchorPosX(-1000, 1.5f);//1100
        MessagesControllers.sharedInstance.CloseInfo();
    }
    #endregion
}

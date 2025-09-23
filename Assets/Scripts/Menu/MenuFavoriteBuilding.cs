using UnityEngine;
using UnityEngine.UI;

public class MenuFavoriteBuilding : MonoBehaviour
{
    public static MenuFavoriteBuilding sharedInstance;
    
    #region variables privadas

    [Header("SCROLL'S DE MENU INMUEBLES FAVORITOS")]
    [SerializeField] private ScrollMenuController m_scrollToFavoriteBuildingsView;

    [Header("CONTENEDORES INMUEBLES FAVORITOS")]
    [SerializeField] private GameObject contentFavoriteBuildingsView;

    [SerializeField] private GameObject prefabToFavoriteBuildingView;
    [SerializeField] private SaveComponentsController m_saveComponentsController;
    #endregion

    private GameObject loadedBuilding;
   
}

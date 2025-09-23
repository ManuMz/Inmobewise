using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum tagOfComponent
{
    //Campos para apartado de notificacion:

    _notificationBackBody,
    _notificationIconHeader,
    _notificationTextBody,
    _notificationBackButton,
    _notificationTextHeader,
    _notificationBackHeader,
    _notificationTextButton,

    //Campos para apartado de Menu anillo:

    _ringMenuMargin,
    _ringMenuIcon,
    _ringMenuBack,
    _ringMenuText,

    //Campos para el apartado de pantalla de carga:

    _loadBack,
    _loadText,
    _loadPoints,

    //Campos para el apartado de categorias:

    _categoryMargin,
    _categoryIcon,
    _categoryBackIcon,
    _categoryBack,
    _categoryText,

    //Campos para el apartado de subcategorias:

    _subCategoryMargin,
    _subCategoryIcon,
    _subCategoryBackIcon,
    _subCategoryBack,
    _subCategoryText,

    //Campos para el apartado de producto

    _productMargin,
    _productBackTop,
    _productBackBot,
    _productText,
    _productDiscount,
    _productBackDetails,
    _productIconDetails,
    _productTextDetails,
    _productBackCharacteristic,
    _productBackCart,
    _productIconCart,
    _productTextCart,
    _productBackBuy,
    _productIconBuy,
    _productTextBuy,

    //Campos para el apartado menu

    _menuBack,
    _menuTextAccount,
    _menuMarginIcon,
    _menuBackIcon,
    _menuIcon,
    _menuMarginButton,
    _menuMargin,
    _menuPanelLateralBackBot,
    _menuPanelLateralBackTop,
    _menuPanelLateralBackUserInfo,

    _safearea
}

public class OptionsToPersonalize : MonoBehaviour
{
    public tagOfComponent option;
    public bool isChanged;
    public bool onlyAwake=true;
    private void Awake()
    {
        isChanged = false;
    }
}

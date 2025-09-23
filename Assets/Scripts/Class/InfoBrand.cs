using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InfoBrand
{
    public string idEmpresa;
    public string nombreEmpresa;
    public string foto;
    public string imagenFondoEmpresa;
    public string independiente;

    public string _menuBack;
    public string _menuMarginButton;
    public string _menuBackIcon;
    public string _menuIcon;
    public string _menuMarginIcon;
    public string _menuMargin;
    public string _menuTextAccount;
    public string _menuPanelLateralBackBot;
    public string _menuPanelLateralBackTop;
    public string _menuPanelLateralBackUserInfo;
    public string _categoryMargin;
    public string _categoryBack;
    public string _categoryBackIcon;
    public string _categoryIcon;
    public string _categoryText;
    public string _subCategoryMargin;
    public string _subCategoryBack;
    public string _subCategoryBackIcon;
    public string _subCategoryIcon;
    public string _subCategoryText;
    public string _productMargin;
    public string _productBackTop;
    public string _productBackBot;
    public string _productText;
    public string _productTextDetails;
    public string _productBackDetails;
    public string _productIconDetails;
    public string _productBackCart;
    public string _productIconCart;
    public string _productBackBuy;
    public string _productTextBuy;
    public string _productBackCharacteristic;
    public string _productTextCart;
    public string _productIconBuy;

    public Texture2D FotoTexture2DBrand;

    List<string> categories = new List<string>();
    List<string> subCategories = new List<string>();
    List<string> productos = new List<string>();
}

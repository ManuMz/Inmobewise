using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
public class PersonalizableController : MonoBehaviour
{
    public static PersonalizableController sharedInstance;

    //////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////   
    //_menuMarginButton se uso para los colores de botones de user//
    //////////////////////////////////////////////////////////////// 
    //////////////////////////////////////////////////////////////// 
    //////////////////////////////////////////////////////////////// 

    //r:155g:155b:25a:.8

    [HideInInspector]
    public bool isDone = false;// booleano que indica si esta hecha la personalización por parte del usuario

    #region Variables privadas
    private Personalizable personaizableManager; // referencia a la clase Personalizable
    private CancellationTokenSource cts; // referencia a la clase CancellationTokenSource
    #endregion

    #region Obtener valores de variables privadas
    public bool GetIsDone()
    {
        return isDone;
    }
    #endregion

    #region MONOBEHAVIOUR METHODS
    private void Awake()
    {
        if (!sharedInstance)
            sharedInstance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        cts = new CancellationTokenSource();
        //fondoHeader.texture = spriteHeader;
        Data();
    }
    private void Update()
    {
    }
    #endregion

    #region UTILS
    private void executePersonalizable()
    {
        if (isDone)
        {
            foreach (var item in FindObjectsOfType<OptionsToPersonalize>())
            {
                if (!item.isChanged && item.onlyAwake)
                {
                    switch (item.option)
                    {
                        //
                        case tagOfComponent._notificationBackBody:
                            setColor(item.gameObject, personaizableManager._notificationBackBody);
                            break;


                        //
                        case tagOfComponent._notificationIconHeader:
                            setColor(item.gameObject, personaizableManager._notificationIconHeader);
                            break;


                        //
                        case tagOfComponent._notificationTextBody:
                            setColor(item.gameObject, personaizableManager._notificationTextBody);
                            break;


                        //
                        case tagOfComponent._notificationBackButton:
                            setColor(item.gameObject, personaizableManager._notificationBackButton);
                            break;


                        //
                        case tagOfComponent._notificationTextHeader:
                            setColor(item.gameObject, personaizableManager._notificationTextHeader);
                            break;



                        //
                        case tagOfComponent._notificationBackHeader:
                            setColor(item.gameObject, personaizableManager._notificationBackHeader);
                            break;


                        //
                        case tagOfComponent._notificationTextButton:
                            setColor(item.gameObject, personaizableManager._notificationTextButton);
                            break;


                        //
                        case tagOfComponent._ringMenuMargin:
                            setColor(item.gameObject, personaizableManager._ringMenuMargin);
                            break;


                        //
                        case tagOfComponent._ringMenuIcon:
                            setColor(item.gameObject, personaizableManager._ringMenuIcon);
                            break;


                        //
                        case tagOfComponent._ringMenuBack:
                            setColor(item.gameObject, personaizableManager._ringMenuBack);
                            break;


                        //
                        case tagOfComponent._ringMenuText:
                            setColor(item.gameObject, personaizableManager._ringMenuText);
                            break;


                        //
                        case tagOfComponent._loadBack:
                            setColor(item.gameObject, personaizableManager._loadBack);
                            break;


                        //
                        case tagOfComponent._loadText:
                            setColor(item.gameObject, personaizableManager._loadText);
                            break;


                        //
                        case tagOfComponent._loadPoints:
                            setColor(item.gameObject, personaizableManager._loadPoints);
                            break;


                        //
                        case tagOfComponent._categoryMargin:
                            setColor(item.gameObject, personaizableManager._categoryMargin);
                            break;


                        //
                        case tagOfComponent._categoryIcon:
                            setColor(item.gameObject, personaizableManager._categoryIcon);
                            break;


                        //
                        case tagOfComponent._categoryBackIcon:
                            setColor(item.gameObject, personaizableManager._categoryBackIcon);
                            break;


                        //
                        case tagOfComponent._categoryBack:
                            setColor(item.gameObject, personaizableManager._categoryBack);
                            break;



                        //
                        case tagOfComponent._categoryText:
                            setColor(item.gameObject, personaizableManager._categoryText);
                            break;


                        //
                        case tagOfComponent._subCategoryMargin:
                            setColor(item.gameObject, personaizableManager._subCategoryMargin);
                            break;


                        //
                        case tagOfComponent._subCategoryIcon:
                            setColor(item.gameObject, personaizableManager._subCategoryIcon);
                            break;


                        //
                        case tagOfComponent._subCategoryBackIcon:
                            setColor(item.gameObject, personaizableManager._subCategoryBackIcon);
                            break;


                        //
                        case tagOfComponent._subCategoryBack:
                            setColor(item.gameObject, personaizableManager._subCategoryBack);
                            break;


                        //
                        case tagOfComponent._subCategoryText:
                            setColor(item.gameObject, personaizableManager._subCategoryText);
                            break;


                        //
                        case tagOfComponent._productMargin:
                            setColor(item.gameObject, personaizableManager._productMargin);
                            break;


                        //
                        case tagOfComponent._productBackTop:
                            setColor(item.gameObject, personaizableManager._productBackTop);
                            break;



                        //
                        case tagOfComponent._productBackBot:
                            setColor(item.gameObject, personaizableManager._productBackBot);
                            break;


                        //
                        case tagOfComponent._productText:
                            setColor(item.gameObject, personaizableManager._productText);
                            break;


                        //
                        case tagOfComponent._productDiscount:
                            setColor(item.gameObject, personaizableManager._productDiscount);
                            break;



                        //
                        case tagOfComponent._productBackDetails:
                            setColor(item.gameObject, personaizableManager._productBackDetails);
                            break;


                        //
                        case tagOfComponent._productIconDetails:
                            setColor(item.gameObject, personaizableManager._productIconDetails);
                            break;



                        //
                        case tagOfComponent._productTextDetails:
                            setColor(item.gameObject, personaizableManager._productTextDetails);
                            break;


                        //
                        case tagOfComponent._productBackCharacteristic:
                            setColor(item.gameObject, personaizableManager._productBackCharacteristic);
                            break;



                        //
                        case tagOfComponent._productBackCart:
                            setColor(item.gameObject, personaizableManager._productBackCart);
                            break;


                        //color of icon in product in cart button
                        case tagOfComponent._productIconCart:
                            setColor(item.gameObject, personaizableManager._productIconCart);
                            break;


                        //color of Text on product in cart button when is pressed execute the function to add cart
                        case tagOfComponent._productTextCart:
                            setColor(item.gameObject, personaizableManager._productTextCart);
                            break;


                        //Background of button buy
                        case tagOfComponent._productBackBuy:
                            setColor(item.gameObject, personaizableManager._productBackBuy);
                            break;


                        //Color of icon on icon product on the buy button
                        case tagOfComponent._productIconBuy:
                            setColor(item.gameObject, personaizableManager._productIconBuy);
                            break;



                        //Color text of prodcut on the buy button
                        case tagOfComponent._productTextBuy:
                            setColor(item.gameObject, personaizableManager._productTextBuy);
                            break;


                        //Background of menu of background
                        case tagOfComponent._menuBack:
                            setColor(item.gameObject, personaizableManager._menuBack);
                            break;


                        //menu text on left position of account
                        case tagOfComponent._menuTextAccount:
                            setColor(item.gameObject, personaizableManager._menuTextAccount);
                            break;


                        //Menu Margin Icon
                        case tagOfComponent._menuMarginIcon:
                            setColor(item.gameObject, personaizableManager._menuMarginIcon);
                            break;


                        //Background icon of menu AR or low gamma
                        case tagOfComponent._menuBackIcon:
                            setColor(item.gameObject, personaizableManager._menuBackIcon);
                            break;


                        //Icon of menu AR or low gamma
                        case tagOfComponent._menuIcon:
                            setColor(item.gameObject, personaizableManager._menuIcon);
                            break;


                        //Margin button of menu Ar or low gamma
                        case tagOfComponent._menuMarginButton:
                            setColor(item.gameObject, personaizableManager._menuMarginButton);
                            break;


                        //Margin of menu
                        case tagOfComponent._menuMargin:
                            setColor(item.gameObject, personaizableManager._menuMargin);
                            break;

                        case tagOfComponent._safearea:
                            setColor(item.gameObject, personaizableManager._safearea);
                            break;


                        case tagOfComponent._menuPanelLateralBackBot:
                            setColor(item.gameObject, personaizableManager._menuPanelLateralBackBot);
                            break;

                        case tagOfComponent._menuPanelLateralBackTop:
                            setColor(item.gameObject, personaizableManager._menuPanelLateralBackTop);
                            break;

                        case tagOfComponent._menuPanelLateralBackUserInfo:
                            setColor(item.gameObject, personaizableManager._menuPanelLateralBackUserInfo);
                            break;
                        default:
                            break;

                    }
                    item.isChanged = true;
                }
            }
        }

    }
    public void updateMenuColors(InfoBrand infoBrand) // ------------------------------No cuenta con referencias, pero se encuentra activo por lo que el metodo debe estar asignado de otra forma-------------------------------------[1]
    {
        foreach (var item in FindObjectsOfType<OptionsToPersonalize>())
        {
            if (item.onlyAwake)
            {

                switch (item.option)
                {
                    case tagOfComponent._categoryMargin:
                        setColor(item.gameObject, infoBrand._categoryMargin);
                        break;


                    //
                    case tagOfComponent._categoryIcon:
                        setColor(item.gameObject, infoBrand._categoryIcon);
                        break;


                    //
                    case tagOfComponent._categoryBackIcon:
                        setColor(item.gameObject, infoBrand._categoryBackIcon);
                        break;


                    //
                    case tagOfComponent._categoryBack:
                        setColor(item.gameObject, infoBrand._categoryBack);
                        break;



                    //
                    case tagOfComponent._categoryText:
                        setColor(item.gameObject, infoBrand._categoryText);
                        break;


                    //
                    case tagOfComponent._subCategoryMargin:
                        setColor(item.gameObject, infoBrand._subCategoryMargin);
                        break;


                    //
                    case tagOfComponent._subCategoryIcon:
                        setColor(item.gameObject, infoBrand._subCategoryIcon);
                        break;


                    //
                    case tagOfComponent._subCategoryBackIcon:
                        setColor(item.gameObject, infoBrand._subCategoryBackIcon);
                        break;


                    //
                    case tagOfComponent._subCategoryBack:
                        setColor(item.gameObject, infoBrand._subCategoryBack);
                        break;


                    //
                    case tagOfComponent._subCategoryText:
                        setColor(item.gameObject, infoBrand._subCategoryText);
                        break;


                    //
                    case tagOfComponent._productMargin:
                        setColor(item.gameObject, infoBrand._productMargin);
                        break;


                    //
                    case tagOfComponent._productBackTop:
                        setColor(item.gameObject, infoBrand._productBackTop);
                        break;



                    //
                    case tagOfComponent._productBackBot:
                        setColor(item.gameObject, infoBrand._productBackBot);
                        break;


                    //
                    case tagOfComponent._productText:
                        setColor(item.gameObject, infoBrand._productText);
                        break;


                    //


                    //
                    case tagOfComponent._productBackDetails:
                        setColor(item.gameObject, infoBrand._productBackDetails);
                        break;


                    //
                    case tagOfComponent._productIconDetails:
                        setColor(item.gameObject, infoBrand._productIconDetails);
                        break;



                    //
                    case tagOfComponent._productTextDetails:
                        setColor(item.gameObject, infoBrand._productTextDetails);
                        break;


                    //
                    case tagOfComponent._productBackCharacteristic:
                        setColor(item.gameObject, infoBrand._productBackCharacteristic);
                        break;



                    //
                    case tagOfComponent._productBackCart:
                        setColor(item.gameObject, infoBrand._productBackCart);
                        break;


                    //color of icon in product in cart button
                    case tagOfComponent._productIconCart:
                        setColor(item.gameObject, infoBrand._productIconCart);
                        break;


                    //color of Text on product in cart button when is pressed execute the function to add cart
                    case tagOfComponent._productTextCart:
                        setColor(item.gameObject, infoBrand._productTextCart);
                        break;


                    //Background of button buy
                    case tagOfComponent._productBackBuy:
                        setColor(item.gameObject, infoBrand._productBackBuy);
                        break;


                    //Color of icon on icon product on the buy button
                    case tagOfComponent._productIconBuy:
                        setColor(item.gameObject, infoBrand._productIconBuy);
                        break;



                    //Color text of prodcut on the buy button
                    case tagOfComponent._productTextBuy:

                        //Debug.Log("iten: "+item+" infobrand: "+ infoBrand);
                        setColor(item.gameObject, infoBrand._productTextBuy);
                        break;


                    //Background of menu of background
                    case tagOfComponent._menuBack:
                        setColor(item.gameObject, infoBrand._menuBack);
                        break;


                    //menu text on left position of account
                    case tagOfComponent._menuTextAccount:
                        setColor(item.gameObject, infoBrand._menuTextAccount);
                        break;


                    //Menu Margin Icon
                    case tagOfComponent._menuMarginIcon:
                        setColor(item.gameObject, infoBrand._menuMarginIcon);
                        break;


                    //Background icon of menu AR or low gamma
                    case tagOfComponent._menuBackIcon:
                        setColor(item.gameObject, infoBrand._menuBackIcon);
                        break;


                    //Icon of menu AR or low gamma
                    case tagOfComponent._menuIcon:
                        setColor(item.gameObject, infoBrand._menuIcon);
                        break;


                    //Margin button of menu Ar or low gamma
                    case tagOfComponent._menuMarginButton:
                        setColor(item.gameObject, infoBrand._menuMarginButton);
                        break;


                    //Margin of menu
                    case tagOfComponent._menuMargin:
                        setColor(item.gameObject, infoBrand._menuMargin);
                        break;

                    case tagOfComponent._menuPanelLateralBackBot:
                        setColor(item.gameObject, infoBrand._menuPanelLateralBackBot);
                        break;

                    case tagOfComponent._menuPanelLateralBackTop:
                        setColor(item.gameObject, infoBrand._menuPanelLateralBackTop);
                        break;

                    case tagOfComponent._menuPanelLateralBackUserInfo:
                        setColor(item.gameObject, infoBrand._menuPanelLateralBackUserInfo);
                        break;

                    default:
                        break;

                }
            }

        }

    }
    public void restartMenuColors()// ------------------------------No cuenta con referencias, pero se encuentra activo por lo que el metodo debe estar asignado de otra forma-------------------------------------[2]
    {
        foreach (var item in FindObjectsOfType<OptionsToPersonalize>())
        {
            if (item.onlyAwake)
            {
                switch (item.option)
                {
                    case tagOfComponent._categoryMargin:
                        setColor(item.gameObject, personaizableManager._categoryMargin);
                        break;


                    //
                    case tagOfComponent._categoryIcon:
                        setColor(item.gameObject, personaizableManager._categoryIcon);
                        break;


                    //
                    case tagOfComponent._categoryBackIcon:
                        setColor(item.gameObject, personaizableManager._categoryBackIcon);
                        break;


                    //
                    case tagOfComponent._categoryBack:
                        setColor(item.gameObject, personaizableManager._categoryBack);
                        break;



                    //
                    case tagOfComponent._categoryText:
                        setColor(item.gameObject, personaizableManager._categoryText);
                        break;


                    //
                    case tagOfComponent._subCategoryMargin:
                        setColor(item.gameObject, personaizableManager._subCategoryMargin);
                        break;


                    //
                    case tagOfComponent._subCategoryIcon:
                        setColor(item.gameObject, personaizableManager._subCategoryIcon);
                        break;


                    //
                    case tagOfComponent._subCategoryBackIcon:
                        setColor(item.gameObject, personaizableManager._subCategoryBackIcon);
                        break;


                    //
                    case tagOfComponent._subCategoryBack:
                        setColor(item.gameObject, personaizableManager._subCategoryBack);
                        break;


                    //
                    case tagOfComponent._subCategoryText:
                        setColor(item.gameObject, personaizableManager._subCategoryText);
                        break;


                    //
                    case tagOfComponent._productMargin:
                        setColor(item.gameObject, personaizableManager._productMargin);
                        break;


                    //
                    case tagOfComponent._productBackTop:
                        setColor(item.gameObject, personaizableManager._productBackTop);
                        break;



                    //
                    case tagOfComponent._productBackBot:
                        setColor(item.gameObject, personaizableManager._productBackBot);
                        break;


                    //
                    case tagOfComponent._productText:
                        setColor(item.gameObject, personaizableManager._productText);
                        break;


                    //

                    //
                    case tagOfComponent._productBackDetails:
                        setColor(item.gameObject, personaizableManager._productBackDetails);
                        break;


                    //
                    case tagOfComponent._productIconDetails:
                        setColor(item.gameObject, personaizableManager._productIconDetails);
                        break;



                    //
                    case tagOfComponent._productTextDetails:
                        setColor(item.gameObject, personaizableManager._productTextDetails);
                        break;


                    //
                    case tagOfComponent._productBackCharacteristic:
                        setColor(item.gameObject, personaizableManager._productBackCharacteristic);
                        break;



                    //
                    case tagOfComponent._productBackCart:
                        setColor(item.gameObject, personaizableManager._productBackCart);
                        break;


                    //color of icon in product in cart button
                    case tagOfComponent._productIconCart:
                        setColor(item.gameObject, personaizableManager._productIconCart);
                        break;


                    //color of Text on product in cart button when is pressed execute the function to add cart
                    case tagOfComponent._productTextCart:
                        setColor(item.gameObject, personaizableManager._productTextCart);
                        break;


                    //Background of button buy
                    case tagOfComponent._productBackBuy:
                        setColor(item.gameObject, personaizableManager._productBackBuy);
                        break;


                    //Color of icon on icon product on the buy button
                    case tagOfComponent._productIconBuy:
                        setColor(item.gameObject, personaizableManager._productIconBuy);
                        break;



                    //Color text of prodcut on the buy button
                    case tagOfComponent._productTextBuy:
                        setColor(item.gameObject, personaizableManager._productTextBuy);
                        break;


                    //Background of menu of background
                    case tagOfComponent._menuBack:
                        setColor(item.gameObject, personaizableManager._menuBack);
                        break;


                    //menu text on left position of account
                    case tagOfComponent._menuTextAccount:
                        setColor(item.gameObject, personaizableManager._menuTextAccount);
                        break;


                    //Menu Margin Icon
                    case tagOfComponent._menuMarginIcon:
                        setColor(item.gameObject, personaizableManager._menuMarginIcon);
                        break;


                    //Background icon of menu AR or low gamma
                    case tagOfComponent._menuBackIcon:
                        setColor(item.gameObject, personaizableManager._menuBackIcon);
                        break;


                    //Icon of menu AR or low gamma
                    case tagOfComponent._menuIcon:
                        setColor(item.gameObject, personaizableManager._menuIcon);
                        break;


                    //Margin button of menu Ar or low gamma
                    case tagOfComponent._menuMarginButton:
                        setColor(item.gameObject, personaizableManager._menuMarginButton);
                        break;


                    //Margin of menu
                    case tagOfComponent._menuMargin:
                        setColor(item.gameObject, personaizableManager._menuMargin);
                        break;

                    case tagOfComponent._menuPanelLateralBackBot:
                        setColor(item.gameObject, personaizableManager._menuPanelLateralBackBot);
                        break;

                    case tagOfComponent._menuPanelLateralBackTop:
                        setColor(item.gameObject, personaizableManager._menuPanelLateralBackTop);
                        break;

                    case tagOfComponent._menuPanelLateralBackUserInfo:
                        setColor(item.gameObject, personaizableManager._menuPanelLateralBackUserInfo);
                        break;
                    default:
                        break;

                }
            }

        }

    }
    private void setColor(GameObject item, string codigo)
    {
        //print(codigo);
        Color color;
        if (ColorUtility.TryParseHtmlString(codigo, out color))
        {
            if (item.GetComponent<Image>())
            {
                item.GetComponent<Image>().color = color;
            }
            else if (item.GetComponent<Text>())
            {

                item.GetComponent<Text>().color = color;
            }
            else if (item.GetComponent<RawImage>()) {
                item.GetComponent<RawImage>().color = color;
            }

           // Debug.Log("color01: "+color);
        }
        else {
            string formated = string.Format("Error al convertir el color:{0}", codigo);
            Debug.LogWarning(formated);
        }
    }
    void setColorComponents(List<GameObject> lista, Color color)// ----------------------------------------------------------------Verificar si es necesario-----------------------------------------------------------[3]
    {
        foreach (var item in lista)
        {
            if (item.GetComponent<Image>())
            {
                item.GetComponent<Image>().color = color;
            }
            else if (item.GetComponent<Text>())
            {

                item.GetComponent<Text>().color = color;
            }
        }
    }
    private void Data()
    {
        _ = downloadData();
    }

    private async UniTaskVoid downloadData()
    {
        WWWForm form = new WWWForm();
        form.AddField("idEmpresaApp", (int)FindObjectOfType<config>().GetApp());
        var data = (await UnityWebRequest.Post(config.sharedInstance.urlArvispaceServices("coloresApp.php"), form).SendWebRequest().WithCancellation(cts.Token)).downloadHandler.text;
        try
        {
            personaizableManager = JsonUtility.FromJson<Personalizable>(data);
            isDone = true;
            executePersonalizable();
        }
        catch (Exception e)
        {
            retry();
            Debug.Log(e.Message);
        }
    }

    private void retry() {
        cts.Cancel();
        Data();
    }


    #region Clase Personalizable
    [Serializable]
    public class Personalizable {

        //Campos para apartado de notificacion:
        public string _notificationBackBody;
        public string _notificationIconHeader;
        public string _notificationTextBody;
        public string _notificationBackButton;
        public string _notificationTextHeader;
        public string _notificationBackHeader;
        public string _notificationTextButton;

        //Campos para apartado de Menu anillo:

        public string _ringMenuMargin;
        public string _ringMenuIcon;
        public string _ringMenuBack;
        public string _ringMenuText;

        //Campos para el apartado de pantalla de carga:

        public string _loadBack;
        public string _loadText;
        public string _loadPoints;

        //Campos para el apartado de categorias:

        public string _categoryMargin;
        public string _categoryIcon;
        public string _categoryBackIcon;
        public string _categoryBack;
        public string _categoryText;

        //Campos para el apartado de subcategorias:

        public string _subCategoryMargin;
        public string _subCategoryIcon;
        public string _subCategoryBackIcon;
        public string _subCategoryBack;
        public string _subCategoryText;

        //Campos para el apartado de productos:

        public string _productMargin;
        public string _productBackTop;
        public string _productBackBot;
        public string _productText;
        public string _productDiscount;
        public string _productBackDetails;
        public string _productIconDetails;
        public string _productTextDetails;
        public string _productBackCharacteristic;
        public string _productBackCart;
        public string _productIconCart;
        public string _productTextCart;
        public string _productBackBuy;
        public string _productIconBuy;
        public string _productTextBuy;

        //Campos para el apartado menu

        public string _menuBack;
        public string _menuTextAccount;
        public string _menuMarginIcon;
        public string _menuBackIcon;
        public string _menuIcon;
        public string _menuMarginButton;
        public string _menuMargin;
        public string _menuPanelLateralBackBot;
        public string _menuPanelLateralBackTop;
        public string _menuPanelLateralBackUserInfo;
        

        public string _safearea;

    }
    #endregion


    #endregion
}

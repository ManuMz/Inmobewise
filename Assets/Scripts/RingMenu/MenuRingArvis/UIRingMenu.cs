using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UIRingMenu : MonoBehaviour
{
    public enum typeMenu { Standard, Selected, Color, Animation };
    public typeMenu typeM;
    public List<ButtonRing> buttons;
    public List<ButtonRing> CurrentListButtons = new List<ButtonRing>();
    public GameObject selectedButton;
    public GameObject colorPrefabButton;
    public GameObject animationPrefabButton;
    public GameObject MenuUI;

    private TouchController touchController;
    public bool moveEnable;
    private Vector2 firstClick;

    private int NearestButtonSelected;
    private bool moveToNearest = true;
    private bool moveToSelected;
    private int ButtonSelected;

    /*private*/
    public float maxRadius;/* = 300;*/
    private float currentRadius = 0;
    private float speedRadius = 2.5f;
    private float ringPos = 0.5f;
    private float speed = 40;

    private bool expand;
    private bool shrink;

    void Start()
    {
        touchController = FindObjectOfType<TouchController>();
        //#if UNITY_EDITOR
        Init();
        //#endif
    }
    public void Init()
    {
        typeM = typeMenu.Standard;
        CreateButtonsInRing();
        expand = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveEnable)
        {
            Vector2 current = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            if (Vector2.Distance(current, firstClick) > 0.005f)
            {
                moveToSelected = false;
            }

            RotateMenuTo(current);

            firstClick = Vector2.Lerp(firstClick, current, 4 * Time.deltaTime);
            moveToNearest = true;
        }
        else if (moveToSelected)
        {
            if (SetButtonToCenter(ButtonSelected))
            {
                moveToSelected = false;
            }
        }
        else
        {
            if (CurrentListButtons.Count > 0)
            {
                if (moveToNearest)
                {
                    NearestButtonSelected = GetTheNearestButton();
                    if (GetAngle(NearestButtonSelected) > 5)
                    {
                        SetButtonToCenter(NearestButtonSelected);
                    }
                    else
                        moveToNearest = false;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && moveEnable)
        {
            moveEnable = false;
        }

        RadiusUpdate();


        if (touchController.p_objectSelected)
        {
            if (typeM != typeMenu.Selected && typeM != typeMenu.Color && typeM != typeMenu.Animation)
            {
                typeM = typeMenu.Selected;

                shrink = true;
            }
        }
        else
        {
            if (typeM != typeMenu.Standard)
            {
                typeM = typeMenu.Standard;

                shrink = true;
            }
        }
    }

    public void MoveMenu()
    {
        if (!moveEnable)
        {
            moveEnable = true;
            firstClick = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        }
    }

    void RadiusUpdate()
    {
        if (expand)
        {
            currentRadius = Mathf.Lerp(currentRadius, maxRadius, speedRadius * 1.5f * Time.deltaTime);
            moveToNearest = true;
            selectedButton.transform.localPosition = Vector3.Lerp(selectedButton.transform.localPosition, Vector3.zero, speedRadius * 1.5f * Time.deltaTime);
            if (Math.Abs(currentRadius - maxRadius) <= maxRadius * 0.05f)
            {
                currentRadius = maxRadius;
                expand = false;
            }
        }
        else if (shrink)
        {
            currentRadius = Mathf.Lerp(currentRadius, 0, speedRadius * 1.5f * Time.deltaTime);
            selectedButton.transform.localPosition = Vector3.Lerp(selectedButton.transform.localPosition, Vector3.left * (selectedButton.GetComponent<RectTransform>().sizeDelta.x / 2), speedRadius * 1.5f * Time.deltaTime);
            if (Math.Abs(currentRadius) <= maxRadius * 0.05f)
            {
                currentRadius = 0;
                shrink = false;
                CreateButtonsInRing();

                expand = true;
            }
        }
        else if (moveEnable)
        {
            currentRadius = Mathf.Lerp(currentRadius, maxRadius * 1.2f, speedRadius / 2 * Time.deltaTime);
        }
        else if (currentRadius > maxRadius)
        {
            currentRadius = Mathf.Lerp(currentRadius, maxRadius, speedRadius / 2 * Time.deltaTime);
        }

        UpdatePosButtons();
    }

    public void SelectButton(GameObject g)
    {
        for (int i = 0; i < CurrentListButtons.Count; i++)
        {
            if (CurrentListButtons[i].button == g)
            {
                ButtonSelected = i;
                moveToSelected = true;
                break;
            }
        }
    }

    void RotateMenuTo(Vector2 current)
    {
        int n = CurrentListButtons.Count;

        if (current.y > firstClick.y) //Arriba
        {
            ringPos += speed * Time.deltaTime * Vector3.Distance(current, firstClick);
        }
        else if (current.y < firstClick.y)
        {
            ringPos -= speed * Time.deltaTime * Vector3.Distance(current, firstClick);
        }

        UpdatePosButtons();
    }

    void UpdatePosButtons()
    {
        int n = CurrentListButtons.Count;
        for (int i = 0; i < CurrentListButtons.Count; i++)
        {
            float angle = (i + ringPos) * Mathf.PI * 2f / n;
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * currentRadius, Mathf.Sin(angle) * currentRadius, 0);
            CurrentListButtons[i].button.transform.localPosition = newPos;
        }
    }

    bool SetButtonToCenter(int key)
    {
        int n = CurrentListButtons.Count;
        Vector3 center = MenuUI.transform.position;
        Vector3 a = CurrentListButtons[key].button.transform.position;
        Vector2 r = a - center;

        if (r.y > 0) //Rotar a la derechaW
        {
            ringPos -= 0.1f * Time.deltaTime * GetAngle(key);
        }
        else //Rotar a la izquierda
        {
            ringPos += 0.1f * Time.deltaTime * GetAngle(key);
        }

        if (r.y < 0.2f && r.y > -0.2f)
        {
            return true;
        }

        UpdatePosButtons();

        return false;
    }


    int GetTheNearestButton()
    {
        int key = 0;
        float angle = 360;

        Vector2 right = (CurrentListButtons[0].button.transform.parent.position + Vector3.right * 100) - CurrentListButtons[0].button.transform.parent.position;

        if (CurrentListButtons.Count > 0)
        {
            for (int i = 0; i < CurrentListButtons.Count; i++)
            {
                if (GetAngle(i) < angle)
                {
                    angle = GetAngle(i);
                    key = i;
                }
            }
        }

        return key;
    }

    float GetAngle(int key)
    {
        Vector3 center = MenuUI.transform.position;
        Vector3 a = center + (Vector3.right * 100);
        Vector3 b = CurrentListButtons[key].button.transform.position;
        Vector3 directionA = Vector3.Normalize(center - a);
        Vector3 directionB = Vector3.Normalize(center - b);
        return Vector3.Angle(directionA, directionB); //degrees

    }

    void CreateButtonsInRing()
    {
        int minButtons = 4;
        int colors = 0;
        Producto producto = new Producto();
        int n = CountButtons(typeM);

        switch (typeM)
        {
            case typeMenu.Selected:
                selectedButton.SetActive(true);
                //colors = countColors(int.Parse(manager.p_objectSelected.name.Split('(')[0]));//--------------------APENAS
                break;
            case typeMenu.Standard:
                selectedButton.SetActive(false);
                break;
            case typeMenu.Color:
                //selectedButton.SetActive(true);  
                print(colors);
                n = colors;
                break;
            case typeMenu.Animation:
                //selectedButton.SetActive(true);
                n = colors;
                break;
            default:
                break;
        }


        for (int i = 0; i < CurrentListButtons.Count; i++)
        {
            Destroy(CurrentListButtons[i].button);
        }

        CurrentListButtons.Clear();




        //Revisa si tiene colores para habilitar el botón de color
        if (typeM == typeMenu.Selected && colors <= 1)
        {
            n -= 1;
        }



        if (n < minButtons)
        {
            n = n * Mathf.RoundToInt(Mathf.Ceil((float)minButtons / n));
        }

        if (n > 0)
        {
            int i = 0;

            //if(FindObjectOfType<TesterOrProduction>()._oracleEcommerce == TesterOrProduction.oracleEcommerce.no || (FindObjectOfType<TesterOrProduction>()._oracleEcommerce == TesterOrProduction.oracleEcommerce.yes && typeM == typeMenu.Selected))
            do
            {
                if (typeM == typeMenu.Selected || typeM == typeMenu.Standard)
                {

                    foreach (var b in buttons)
                    {
                        if (isButtonTypeEquals(b, typeM))
                        {
                            if ((b.Name == "Color" && colors > 1) || b.Name != "Color")
                            {
                                //if ((typeM == typeMenu.Selected && FindObjectOfType<TesterOrProduction>()._oracleEcommerce == TesterOrProduction.oracleEcommerce.yes && b.Oracle) || FindObjectOfType<TesterOrProduction>()._oracleEcommerce == TesterOrProduction.oracleEcommerce.no)
                                // if ((FindObjectOfType<config>().p_oracleEcommerce == config.oracleEcommerce.yes && b.Oracle) || (FindObjectOfType<config>().p_oracleEcommerce == config.oracleEcommerce.no && !b.Name.Contains("Oracle")))
                                // {
                                float angle = (i + ringPos) * Mathf.PI * 2f / n;
                                Vector3 newPos = new Vector3(Mathf.Cos(angle) * currentRadius, Mathf.Sin(angle) * currentRadius, 0);
                                GameObject go = Instantiate(b.button, newPos, Quaternion.identity, MenuUI.transform);
                                //Debug.Log("ACABE DE INSTANCIAR BTN");
                                go.transform.localPosition = newPos;
                                ButtonRing br = new ButtonRing();
                                br.button = go;
                                br.Name = b.Name;
                                br.Standard = b.Standard;
                                br.SelectedObject = b.SelectedObject;
                                br.Color = b.Color;
                                CurrentListButtons.Add(br);
                                i++;
                                // }
                            }
                        }
                        //print(i);
                    }
                }
                else
                {
                    for (int j = 0; j < colors; j++)
                    {
                        float angle = (i + ringPos) * Mathf.PI * 2f / n;
                        Vector3 newPos = new Vector3(Mathf.Cos(angle) * currentRadius, Mathf.Sin(angle) * currentRadius, 0);
                        GameObject go = Instantiate(colorPrefabButton, newPos, Quaternion.identity, MenuUI.transform);
                        go.transform.localPosition = newPos;
                        ButtonRing br = new ButtonRing();
                        br.button = go;
                        br.Color = true;
                        br.Name = producto.Caracteristicas[j].idCaracteristica.ToString();
                        if (producto.Caracteristicas[j].fotoTexture2DCaracteristica != null)
                        {
                            br.button.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(producto.Caracteristicas[j].fotoTexture2DCaracteristica, new Rect(0.0f, 0.0f, producto.Caracteristicas[j].fotoTexture2DCaracteristica.width, producto.Caracteristicas[j].fotoTexture2DCaracteristica.height), new Vector2(0.5f, 0.5f), 100.0f);
                        }
                        else
                        {
                            StartCoroutine(downloadColors(br, producto.Caracteristicas[j]));
                        }
                        CurrentListButtons.Add(br);

                        i++;
                    }
                }

            } while (i < n - 1); //Min de botones Evita que sólo tengamos 4 botones o menos en el menú, los repite para rellenar
        }



        if (CurrentListButtons.Count >= 8)
        {
            foreach (var b in CurrentListButtons)
            {
                b.button.transform.localScale = 0.9f * b.button.transform.localScale;
            }
        }
        else
        {
            foreach (var b in CurrentListButtons)
            {
                b.button.transform.localScale = Vector3.one;
            }
        }
    }

    IEnumerator downloadColors(ButtonRing btn, Caracteristica caracteristica)
    {
        using (UnityWebRequest wwwTexture = UnityWebRequestTexture.GetTexture(caracteristica.foto))
        {
            UnityWebRequestAsyncOperation asyncOperationTexture = wwwTexture.SendWebRequest();
            while (!wwwTexture.isDone)
            {
                yield return null;
   
            }
            Texture2D texture = DownloadHandlerTexture.GetContent(wwwTexture);
            caracteristica.fotoTexture2DCaracteristica = texture;
            btn.button.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

    }

    //int countColors(int id)
    //{
    //    //Producto producto = manager.p_objectSelected.GetComponent<ObjectBehaviour>().producto;

    //    return producto.Caracteristicas.Count;
    //}
    public void SelectColor(GameObject t)
    {
        foreach (var i in CurrentListButtons)
        {
            if (i.button == t)
            {
                //manager.p_objectSelected.GetComponent<ObjectBehaviour>().SetColor(i.Name);

            }
        }
    }
    int CountButtons(typeMenu t)
    {
        int n = 0;
        foreach (var i in buttons)
        {
            switch (t)
            {
                case typeMenu.Standard:
                    if (i.Standard)
                    {
                        n++;
                        //if (FindObjectOfType<TesterOrProduction>().p_oracleEcommerce == TesterOrProduction.oracleEcommerce.yes && i.Oracle)
                        //    n++;
                        //else
                        //    n++;
                    }
                    break;
                case typeMenu.Selected:
                    if (i.SelectedObject)
                    {
                        n++;
                    }
                    //{
                    //    if ((FindObjectOfType<config>().p_oracleEcommerce == config.oracleEcommerce.no && !i.Name.Contains("Oracle")) || FindObjectOfType<config>().p_oracleEcommerce == config.oracleEcommerce.yes)
                    //        n++;
                    //}
                    break;
                case typeMenu.Color:
                    //int colors = countColors(int.Parse(manager.objectSelected.name.Split('(')[0]));
                    //n = countColors(int.Parse(manager.p_objectSelected.name.Split('(')[0]));//--------------------APENAS
                    break;
                default:
                    break;
            }
        }
        //print(n);
        return n;
    }
    public void OpenColorMenu()
    {
        shrink = true;
        typeM = typeMenu.Color;

    }

    public void OpenAnimatorMenu()
    {
        shrink = true;
        typeM = typeMenu.Animation;

    }

    private bool isButtonTypeEquals(ButtonRing b, typeMenu t)
    {
        bool flag = false;
        switch (t)
        {
            case typeMenu.Standard:
                if (b.Standard)
                    flag = true;
                break;
            case typeMenu.Selected:
                if (b.SelectedObject)
                    flag = true;
                break;
            case typeMenu.Color:
                if (b.Color)
                    flag = true;
                break;
            default:
                break;
        }

        return flag;
    }

    [System.Serializable]
    public class ButtonRing
    {
        public GameObject button;
        public bool Standard;
        public bool SelectedObject;
        public bool Color;
        public string Name;
        public bool Oracle;
    }
    //------------------------------------------------------------------------------------------------
    public void LeaveObjectOrTexture()
    {
        switch (touchController.p_objectSelected.GetComponent<ObjectProperties>().itemType)
        {
            case ObjectProperties.ItemTypes.Object:
                touchController.p_objectSelected.layer = 11;//placed Objects
                touchController.p_objectSelected.GetComponent<ObjectProperties>().ChangeBoxColliderSizeBig();
                touchController.p_objectSelected.GetComponent<ObjectProperties>().instanciated = true;
                touchController.p_objectSelected = null;
                MessagesControllers.sharedInstance.CreateNotification("Confirmación", "Producto fijado correctamente", 2f, MessagesControllers.iconsNotifications.success);
                break;
            case ObjectProperties.ItemTypes.Texture:
                touchController.p_objectSelected.GetComponent<ObjectProperties>().instanciated = true;
                touchController.p_objectSelected = null;
                MessagesControllers.sharedInstance.CreateNotification("Confirmación", "Textura fijada correctamente", 2f, MessagesControllers.iconsNotifications.success);
                break;
        }

        if (TutorialControl.sharedInstance.IsPlaying()) //obtención de la variable que indica que el tutorial esta en ejecución 
        {
            FindObjectOfType<TutorialItem>().toDestroy = true; //Destrucción del tutorialItem
        }
    }
    public void EliminateObject()
    {
        Destroy(touchController.p_objectSelected);
    }


    public void StartToDuplicateObject()
    {
        _ = DuplicateObject();
    }

    private async UniTaskVoid DuplicateObject()
    {
        touchController.p_objectSelected.layer = 11;//placed Objects
        touchController.p_objectSelected.GetComponent<Rigidbody>().isKinematic = false;//controla si la fisica afecta el cuerpo rigido(false)
        touchController.p_objectSelected.GetComponent<ObjectProperties>().instanciated = false;
        
        int idProBodPre = touchController.p_objectSelected.GetComponent<ObjectProperties>().idProBodPre;
        int idSelected = touchController.p_objectSelected.GetComponent<ObjectProperties>().idSelected;

        //Ruta del servidor donde se encuentran los Assetbundles de los productos
        string path = "";
        await config.sharedInstance.urlArvispaceAssets(idProBodPre.ToString(), (x) => path = x);

        AssetBundle bundle = await Downloads.DownloadAssetBundleProduct(path);
        GameObject duplicatedObject = bundle.LoadAsset<GameObject>(idProBodPre.ToString());
        
        try
        {
            //Acceso a ObjectProperties
            ObjectProperties objProp = duplicatedObject.GetComponent<ObjectProperties>();
            objProp.idSelected = idSelected;
            objProp.idProBodPre = idProBodPre;
            try
            {
                //Autoincrement
                var idState = new AutoIncrement();
                objProp.UniqueId = idState.GenerateId();

            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al generar id autoincremental en DuplicateObject: {ex}");

            }
            touchController.p_objectSelected = Instantiate(duplicatedObject, new Vector3(0, -5f, 0), Quaternion.identity, touchController.GetFatherProducts().transform);
            MessagesControllers.sharedInstance.CreateNotification("Confirmación", "Producto duplicado correctamente", 2f, MessagesControllers.iconsNotifications.success);

        }
        catch(Exception ex)
        {

            Debug.LogError($"Error al generar el duplicado del inmueble:{ex.Message}");
        }
        finally
        {
            bundle.Unload(false);
        }
    }
}

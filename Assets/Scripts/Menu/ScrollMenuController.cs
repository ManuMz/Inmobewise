using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;
using DG.Tweening;
using UnityEngine.Events;

public class ScrollMenuController : MonoBehaviour
{
    public enum type { horizontal, vertical }

    [SerializeField] public type _type;
    [SerializeField] public float _offScreen;
    [HideInInspector] public ScrollRect _scrollRect;
    [SerializeField] public GameObject m_container;

    public Vector2 _currentPosition = Vector2.zero;


    private float timer = 0;
    [SerializeField] private float delay = 1f;
    private bool valueChanged = true;

    public rangeFromScroll _range = new rangeFromScroll();

    [SerializeField] /*private*/public List<GameObject> items = new List<GameObject>();


    [SerializeField] private bool isNecesaryCleanCache = false;
    [SerializeField] private float timerToClean = 0f;
    [SerializeField] private float timePerClear = 10f;
    public struct rangeFromScroll
    {
        public float startPoint;
        public float endPoint;
    }
    private void Awake()
    {

        //  poselement();

        if (!m_container)
            m_container = transform.GetChild(0).GetChild(0).gameObject;
    }

    private void Start()
    {
        //poselement();

        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.onValueChanged.AddListener(onScrollMove);
    }

    public List<GameObject> getChildFromContent()
    {

        List<GameObject> items = new List<GameObject>();
        for (int i = 0; i < m_container.transform.childCount; i++)
        {
            items.Add(m_container.transform.GetChild(i).gameObject);
        }

        //Debug.Log("items from");

        return items;
    }
    public int getChildLength()
    {
        return m_container.transform.childCount;
    }
    private void onScrollMove(Vector2 value)
    {
        _currentPosition = m_container.GetComponent<RectTransform>().anchoredPosition;
        if (Input.touches.Length <= 0 && !Input.GetMouseButtonDown(0))
        {
            if (!valueChanged)
            {
                timer = 0;
            }
            valueChanged = true;
            isNecesaryCleanCache = true;
        }
        else
        {
            timer = 0;
            valueChanged = false;

            isNecesaryCleanCache = false;
            timerToClean = 0;
        }
    }
    private void Update()
    {
        if (isNecesaryCleanCache)
        {
            if (timerToClean < timePerClear)
            {
                timerToClean += Time.deltaTime;
            }
            else
            {
                try
                {
                    //print(gameObject.name);
                    foreach (var item in items)
                    {
                        //print(item.gameObject.name + " : " + item.GetComponent<ScrollItem>()._isVisible.ToString());
                        if (!item.GetComponent<ScrollItem>()._isVisible)
                        {
                            item.GetComponent<ScrollItem>().cleanCache();
                        }
                    }
                    timerToClean = 0;
                    isNecesaryCleanCache = false;
                }
                catch (System.Exception)
                {
                    //print("Actualizando lista...");
                }
            }
        }
        if (!valueChanged)
            return;
        //print(gameObject.name + " :: " + timer);
        if (timer < delay)
        {
            timer += Time.deltaTime;
        }
        else
        {
            valueChanged = false;
            timer = 0;
            //  Debug.Log("a validar los subproductos");
            validate();
        }

    }

    public void onChangeContent()
    {

        items = getChildFromContent();

        //Debug.Log("entra a cambuar contenido " + items.Count + " --- " + items);

        if (items.Count > 0)
        {
            //print(gameObject.name + " :: " + items.Count) ;
            valueChanged = true;
            timer = 0;
            m_container.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        }
        else
        {
            valueChanged = false;
            timer = 0;
            //&System.GC.Collect();
        }


        //FindObjectOfType<MenuLoad>().toogleLoadMenu(false);
        // FindObjectOfType<MenuLoad>().toogleErrorMenu(false);

    }



    public void poselement()
    {

        m_container.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

    }




    public void HideElements(int index, float timerPerElement = .2f, UnityAction onComplete = null)
    {
        try
        {
            if (index >= 0)
            {
                m_container.transform.GetChild(index).transform.DOScale(0, timerPerElement).OnComplete(() => { HideElements(index - 1, timerPerElement); onComplete.Invoke(); });
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void ShowElements(int index = 0, float timerPerElement = .2f, UnityAction onComplete = null)
    {
       //Debug.Log("entra en el showelement de show");
        try
        {
            //Debug.Log("entra en el try de show: " + index + " --- " + m_container.transform.childCount);
            if (index < m_container.transform.childCount)
            {
                //Debug.Log("entra en el index del show");
                m_container.transform.GetChild(index).transform.DOScale(1, timerPerElement).OnComplete(() => { ShowElements(index + 1, timerPerElement); onComplete.Invoke(); });
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("mensaje desde show element: " + e.Message);
        }


        //FindObjectOfType<MenuLoad>().toogleLoadMenu(false);
       // FindObjectOfType<MenuLoad>().toogleErrorMenu(false);
        poselement();
    }

    public void ShowElements01(int index = 0, float timerPerElement = .2f, UnityAction onComplete = null)
    {
        //  Debug.Log("entra en el showelement de show 01");
        try
        {
            //    Debug.Log("entra en el try de show: " + index + " --- " + m_container.transform.childCount);
            if (index < m_container.transform.childCount)
            {
                //  Debug.Log("entra en el index del show");
                m_container.transform.GetChild(index).transform.DOScale(1, timerPerElement).OnComplete(() => { ShowElements01(index + 1, timerPerElement); onComplete.Invoke(); });
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("mensaje desde show element: " + e.Message);
        }


        //FindObjectOfType<MenuLoad>().toogleLoadMenu(false);
        //FindObjectOfType<MenuLoad>().toogleErrorMenu(false);
        poselement();
    }
    public void ShowElements2(int index = 0, float timerPerElement = .2f, UnityAction onComplete = null)
    {
        //Debug.Log("entra en el showelement de show");
        try
        {
            //  Debug.Log("entra en el try de show: "+index+" --- "+ m_container.transform.childCount);
            if (index < m_container.transform.childCount)
            {
                //Debug.Log("entra en el index del show");
                m_container.transform.GetChild(index).transform.DOScale(1, timerPerElement).OnComplete(() => { });
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("mensaje desde show element: " + e.Message);
        }

        //FindObjectOfType<MenuLoad>().toogleLoadMenu(false);
        //FindObjectOfType<MenuLoad>().toogleErrorMenu(true);
        poselement();
    }



    void validate()
    {
        List<GameObject> validates = validateRange();

        foreach (var item in items)
        {
            if (validates.Contains(item))
            {
                item.GetComponent<ScrollItem>()._isVisible = true;
            }
            else
            {
                item.GetComponent<ScrollItem>()._isVisible = false;
            }
        }

    }
    List<GameObject> validateRange()
    {
        try
        {
            List<GameObject> temp = new List<GameObject>();
            switch (_type)
            {
                case type.horizontal:
                    //Se multiplica por -1 por que necesitamos convertirlo a postivo
                    float width = _scrollRect.gameObject.GetComponent<RectTransform>().sizeDelta.x;
                    _range.startPoint = (_currentPosition.x * -1) - _offScreen;
                    _range.endPoint = ((_currentPosition.x * -1) + width) + _offScreen;
                    temp = items.Where(item => item.GetComponent<RectTransform>().anchoredPosition.x > _range.startPoint && item.GetComponent<RectTransform>().anchoredPosition.x < _range.endPoint).ToList();
                    break;
                case type.vertical:
                    float height = _scrollRect.gameObject.GetComponent<RectTransform>().sizeDelta.y;
                    _range.startPoint = _currentPosition.y - _offScreen;
                    _range.endPoint = _currentPosition.y + height + _offScreen;
                    //string temp = string.Format("start point:{0} end point: {1}", _range.startPoint,_range.endPoint);
                    //Debug.Log(temp);
                    temp = items.Where(item => (item.GetComponent<RectTransform>().anchoredPosition.y * -1) > _range.startPoint && (item.GetComponent<RectTransform>().anchoredPosition.y * -1) < _range.endPoint).ToList();
                    break;
                default:
                    break;
            }
            return temp;
        }
        catch (System.Exception)
        {
            //poselement();
            //Debug.Log("ha ocurrido un cambio de contenido");
            onChangeContent();
            ShowElements();
            // poselement();
            // FindObjectOfType<MenuLoad>().toogleLoadMenu(false);
            //FindObjectOfType<MenuLoad>().toogleErrorMenu(false);

            return new List<GameObject>();
        }
    }


}

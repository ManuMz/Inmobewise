using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public GameObject Panel_Modalidad;
    public GameObject scrolltutorials;
    public GameObject scrolltuto_Texturas_Pc;
    public GameObject scrolltutorials_Muebles_Cel;
    public GameObject scrolltuto_Texturas_Cel;

    public Button btn_pc;
    public Button btn_cel;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
        //GameObject.Find("BtnAyuda").GetComponent<Button>().interactable = true;
        //StartCoroutine("Wait_Time");
        
        //btn_pc.interactable = false;
        //scrolltutorials.transform.GetChild(0).transform.gameObject.SetActive(false);
    }
    public void Show_Modalidad()
    {
        Panel_Modalidad.GetComponent<RectTransform>().DOAnchorPosY(0, 1.5f);//1100
    }
    public void Hide_Modalidad()
    {
        Panel_Modalidad.GetComponent<RectTransform>().DOAnchorPosY(1100, 1.5f);//1100
        GameObject.Find("BtnAyuda").GetComponent<Button>().interactable = true;
    }

    //---------------------------- SCROLL TUTORIALES MUEBLES PC -------------------------------
    public void ShowScrollTutorials()
    {
        scrolltutorials.GetComponent<RectTransform>().DOAnchorPosY(0, 1.5f);//1100

        HideScrollTutorials_Texturas_PC();
        HideScrollTutorials_Muebles_CEL();
        HideScrollTutorials_Texturas_CEL();
    }
    public void HideScrollTutorials()
    {
        scrolltutorials.GetComponent<RectTransform>().DOAnchorPosY(1100, 1.5f);//1100
    }

    //---------------------------- SCROLL TUTORIALES TEXTURAS PC -------------------------------
    public void ShowScrollTutorials_Texturas_PC()
    {
        scrolltuto_Texturas_Pc.GetComponent<RectTransform>().DOAnchorPosY(0, 1.5f);//1100

        HideScrollTutorials();
        HideScrollTutorials_Muebles_CEL();
        HideScrollTutorials_Texturas_CEL();
    }
    public void HideScrollTutorials_Texturas_PC()
    {
        scrolltuto_Texturas_Pc.GetComponent<RectTransform>().DOAnchorPosY(1100, 1.5f);//1100
    }

    //---------------------------- SCROLL TUTORIALES MUEBLES CEL -------------------------------
    public void ShowScrollTutorials_Muebles_CEL()
    {
        scrolltutorials_Muebles_Cel.GetComponent<RectTransform>().DOAnchorPosY(0, 1.5f);//1100

        HideScrollTutorials();
        HideScrollTutorials_Texturas_PC();
        HideScrollTutorials_Texturas_CEL();
    }
    public void HideScrollTutorials_Muebles_CEL()
    {
        scrolltutorials_Muebles_Cel.GetComponent<RectTransform>().DOAnchorPosY(1100, 1.5f);//1100
    }

    //---------------------------- SCROLL TUTORIALES TEXTURAS CEL -------------------------------
    public void ShowScrollTutorials_Texturas_CEL()
    {
        scrolltuto_Texturas_Cel.GetComponent<RectTransform>().DOAnchorPosY(0, 1.5f);//1100

        HideScrollTutorials();
        HideScrollTutorials_Texturas_PC();
        HideScrollTutorials_Muebles_CEL();
    }
    public void HideScrollTutorials_Texturas_CEL()
    {
        scrolltuto_Texturas_Cel.GetComponent<RectTransform>().DOAnchorPosY(1100, 1.5f);//1100
    }

    IEnumerator Wait_Time()
    {
        yield return new WaitForSeconds(3f);
        Show_Modalidad();
    }
}

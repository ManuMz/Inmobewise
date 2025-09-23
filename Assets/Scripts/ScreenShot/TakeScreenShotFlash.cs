using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TakeScreenShotFlash : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Image>().DOFade(1, 0.2f).SetLoops(1, LoopType.Yoyo).OnComplete(() => CompleteAnim());
    }

    public void CompleteAnim()
    {
        //FindObjectOfType<Captura>().TakeAndSaveScreenShot(); ///crear el metodo
        Destroy(this.gameObject);
    }
}

using UnityEngine;

public class CanvasSafeAreacontroller : MonoBehaviour
{
    #region miembro estático Patrón de diseño Singleton
    public static CanvasSafeAreacontroller sharedInstance;
    #endregion


    public void OpenCanvasSafeArea()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<Canvas>().sortingOrder = 13;
    }

    public void CloseCanvasSafeArea()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
    }
}

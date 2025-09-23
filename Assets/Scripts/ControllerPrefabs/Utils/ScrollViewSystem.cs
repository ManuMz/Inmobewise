using UnityEngine;
using UnityEngine.UI;

public class ScrollViewSystem : MonoBehaviour
{
    private ScrollRect _scrollRect;

    [SerializeField] private ScrollButton _leftButton;
    [SerializeField] private ScrollButton _rigtButton;
    [SerializeField] private ScrollButton _bottomButton;
    [SerializeField] private ScrollButton _topButton;

    [SerializeField] private float scrollSpeed = 0.01f;
    void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    void Update()
    {
        if (_leftButton != null)
        {
            if (_leftButton.isDown)
            {
                ScrollLeft();
                _leftButton.isDown = false;
            }
        }
        if (_rigtButton != null)
        {
            if (_rigtButton.isDown)
            {
                ScrollRigth();
                _rigtButton.isDown = false;
            }
        }
        //if (_bottomButton != null)
        //{
        //    if (_bottomButton.isDown)
        //    {
        //        ScrollBottom();
        //    }
        //}
        //if (_topButton != null)
        //{
        //    if (_topButton.isDown)
        //    {
        //        ScrollTop();
        //    }
        //}
    }

    private void ScrollLeft()
    {
        if (_scrollRect != null)
        {
            if (_scrollRect.horizontalNormalizedPosition >= 0f)
            {
                _scrollRect.horizontalNormalizedPosition -= scrollSpeed;
            }
        }
    }
    private void ScrollRigth()
    {
        if (_scrollRect != null)
        {
            if (_scrollRect.horizontalNormalizedPosition <= 1f)
            {
                _scrollRect.horizontalNormalizedPosition += scrollSpeed;
            }
        }
    }
    //private void ScrollTop()
    //{
    //    if (_scrollRect != null)
    //    {
    //        if (_scrollRect.verticalNormalizedPosition <= 1f)
    //        {
    //            _scrollRect.verticalNormalizedPosition += scrollSpeed;
    //        }
    //    }
    //}
    //private void ScrollBottom()
    //{
    //    if (_scrollRect != null)
    //    {
    //        if (_scrollRect.verticalNormalizedPosition >= 0f)
    //        {
    //            _scrollRect.verticalNormalizedPosition -= scrollSpeed;
    //        }
    //    }
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TutorialItem : MonoBehaviour
{
    public bool isTouchable;
    public bool isEverPlayable;
    public bool isDesactivable;

    public bool toDestroy;
    [Range(0, 10)]
    public float delayToDestroy;
    public UnityAction beforeStart;
    public UnityAction afterStart;

    private TutorialControl tutorialControl;
    private float timer;

    private void Awake()
    {
        timer = 0;
        if (beforeStart != null)
        {
            beforeStart.Invoke();
        }
        else
        {
            //print("Sin acciones antes de empezar");   
        }

    }

    void Start()
    {
        tutorialControl = FindObjectOfType<TutorialControl>();
        setActiveTouchableComponents(isTouchable);

        //setParentConfig(isDesactivable);
    }

    void Update()
    {
        if (toDestroy)
        {
            timer += Time.deltaTime;
            if (timer > delayToDestroy)
            {
                tutorialControl.EmptyPanel();
                tutorialControl.DefaultConfig();
            }
        }
        //if (animacion.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animacion.IsInTransition(0))
        //{
        //    if (isLoop)
        //    {
        //        animacion.Rebind();
        //    }
        //    else
        //    {
        //        tutorialControl.emptyPanel(); tutorialControl.defaultConfig();
        //    }
        //}
    }

    private void OnDestroy()
    {
        if (afterStart != null)
        {
            afterStart.Invoke();
            afterStart = null;
        }
    }


    #region PUBLIC METHODS
    /// <summary>
    /// this desactive raycast of components from tutorial selected
    /// </summary>
    /// <param name="toggle">to active or desactive</param>
    public void setActiveTouchableComponents(bool toggle)
    {
        foreach (var item in gameObject.GetComponentsInChildren<Image>())
        {
            item.raycastTarget = toggle;
        }
        foreach (var item in gameObject.GetComponentsInChildren<Text>())
        {
            item.raycastTarget = toggle;
        }
    }

    /// <summary>
    /// This function config parent to close
    /// </summary>
    /// <param name="toogle"></param>
    public void setParentConfig(bool toogle)
    {
        GameObject panel = tutorialControl.Panel();
        panel.GetComponent<Image>().raycastTarget = toogle;

        if (toogle)
        {
            panel.GetComponent<Button>().onClick.RemoveAllListeners();
            panel.GetComponent<Button>().onClick.AddListener(() => { tutorialControl.EmptyPanel(); tutorialControl.DefaultConfig(); });
        }
        else
        {
            panel.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
    #endregion
}



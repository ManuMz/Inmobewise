using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollItem : MonoBehaviour
{
    private bool isVisible;
    public bool _isVisible {
        get { return isVisible; }
        set
        {
            isVisible = value;
            if (value)
            {
                onVisible();
            }
            else
            {
                onDisable();
            }
        }
    }
    private void Awake()
    {
        onAwake();
    }
    private void OnDestroy()
    {
        onDestroy();
    }
    public virtual void onDestroy() { }
    private void Start()
    {
        onStart();
    }

    public void cleanCache() {
        onCleanCache();
    }
    public virtual void onAwake() { }
    public virtual void onCleanCache() { }
    public virtual void onVisible() { }

    public virtual void onStart() { }

    public virtual void onDisable() { }
}

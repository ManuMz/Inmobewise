using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RingElement", menuName = "RingMenu/RingElement", order = 2)]
public class RingElement : ScriptableObject
{
    private string nameElement;
    private string iconElementURL;

    #region Variables para descarga de modelo
    private string idProBodPre;
    #endregion

    private Ring nextRing;

    public void  SetRingElements(string name, Ring options)
    {
        this.nameElement = name;
        this.nextRing = options;
    }

    public void SetRingElements(string name, string iconURL, Ring options)
    {
        this.nameElement = name;
        this.iconElementURL = iconURL;
        this.nextRing = options;
    }

    public string GetName()
    {
        return nameElement;
    }

    public string GetIconURL()
    {
        return iconElementURL;
    }

    public Ring GetNextRing()
    {
        return nextRing;
    }

    public void SetName(string newName)
    {
        nameElement = newName;
    }

    public void setIconURL(string newIconURL)
    {
        iconElementURL = newIconURL;
    }

    public void SetNextRing(Ring newRing)
    {
        nextRing = newRing;
    }
}

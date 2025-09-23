using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[CreateAssetMenu(fileName = "Ring", menuName = "RingMenu/Ring", order = 1)]
public class Ring : ScriptableObject
{
    [SerializeField]
    private List<RingElement> elements = new List<RingElement>();

    public Ring(List<RingElement> ringElements)
    {
        elements = ringElements;
    }

    public List<RingElement> GetElements()
    {
        return elements;
    }

    public void SetElements(List<RingElement> ringElements)
    {
        elements = ringElements;
    }
}

using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ProductoData
{
    public int IdProBodPre;
    public int idCaracteristica;
    public List<Clone> clones = new();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tipo_Inmueble
{
    public int Id_Tipo_Inmueble;
    public string tipo_Inmueble;//cambiar a minuscula

    public List<Inmuebles> Inmuebles = new List<Inmuebles>();
}

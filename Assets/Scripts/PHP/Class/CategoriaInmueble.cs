using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CategoriaInmueble
{
    public int Id_Tipo_Publicacion;
    public string Tipo_Publicacion;

    public List<Tipo_Inmueble> Tipo_Inmueble = new List<Tipo_Inmueble>();

}

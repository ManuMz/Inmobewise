using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Caracteristica
{
    public int idCaracteristica;
    public string precio;
    public string tipoCaracteristica;
    public Texture2D fotoTexture2DCaracteristica;
    public string foto;
    public string idProBodPre;
    public string statusOferta;
    public string existencia;
    public string cantidad;
    public string codigo;
    public string precioOferta;
    public string codigoBarra;
    public string item;
    public string itemID;
    public string numeroDescargas;
    public List<Imagen> imagenes = new List<Imagen>();
    public List<Talla> talla = new List<Talla>();
    public string idGenero;
    public string genero;
    public string idOrientacion;
    public string nombreOrientacion;
}

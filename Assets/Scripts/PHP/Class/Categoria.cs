using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Categoria
{
    public string nombreCategoria;
    public string foto;
    public int idCategoria;
    public string imagenFondo;

    public Texture2D FotoTexture2DCategoria;
    public Texture2D FotoTexture2DImagenFondo;
    public List<SubCategoria> SubCategorias = new List<SubCategoria>();
}

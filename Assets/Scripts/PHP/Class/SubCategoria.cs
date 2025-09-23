using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubCategoria
{
    public int idProducto;
    public string nombreProducto;
    public string idCategoria;
    public string activo;
    public string foto;

    public Texture2D FotoTexture2DSubCategoria;
    public List<Producto> Productos = new List<Producto>();
}

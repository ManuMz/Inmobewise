using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Producto
{
    public int idProBodPre;
    public string cantidadMayoreo;
    public string descripcion;
    public string /*descripcionLarga*/ descripcionProducto;
    public string activos;
    public string codigoBarra;
    public string alto;
    public string ancho;
    public string largo;
    public int segmentacion; //
    public string idEmpresa;
    public string nombreEmpresa;
    public string imagenEmpresa;
    public string /*nombreSubcategoria*/nombreProducto;
    public string nombreCategoria;
    public string idMarca;
    public string nombreMarca;
    
   
    public List<Caracteristica> Caracteristicas = new List<Caracteristica>();

    public SubCategoria subCategoria;
}

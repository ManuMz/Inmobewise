using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Inmuebles 
{
    /*
    public string idInmueble;
    public string nombre;
    public string descripcion;
    public string terreno;
    public string construccion;
    public string latitud;
    public string longitud;
    public string precio;
    public string imagenReferencia;
    public Texture2D FotoTexture2DImagenReferencia;
    public string estatus;
    public string solicitudModelado;
    public string idAgente;
    public string apellidos;
    public string correo;
    public string telefono;
    public List<ImagenInmu> imagenes = new List<ImagenInmu>();
    */
    public string Id_Inmueble;
    public string Codigo_Inmueble;
    public string Nombre_Inmueble;
    public string Descripcion_Inmueble;
    public string Calle;
    public string Num_Ext;
    public string Num_Int;
    public string Terreno_M2;
    public string Construccion_M2;
    public string Recamara;
    public string Bano;
    public string Cocina_Integral;
    public string Num_Pisos;
    public string Antiguedad;
    public string Acabados;
    public string Alberca;
    public string Jardin;
    public string Gimnasio;
    public string Roof_Garden;
    public string Estacionamiento;
    public string latitud;
    public string longitud;
    //public string Codigo;

    public List<Pictures> Pictures = new List<Pictures>();

    public Tipo_Inmueble tipo_Inmueble;//Para atraer su subcategoria
}

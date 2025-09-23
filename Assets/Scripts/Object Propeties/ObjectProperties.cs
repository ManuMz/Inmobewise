using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Linq;
using System;
using Unity.Burst.CompilerServices;
using UnityEngine.Rendering;
//using static UnityEditor.Experimental.GraphView.GraphView;


public class ObjectProperties : MonoBehaviour
{
    public bool is_obj_Ceiling; //Define si es un objeto que va en el techo
    public bool is_picture;// Define si es un cuadro
    public int idSelected = -1; // ID seleccionado
    
    [Header("IDENTICADOR ÚNICO (diferencia de clones)")]
    [SerializeField] private int _uniqueId;


    private float moveSpeed = 4; // velocidad de movimiento 
    public bool isSelected;//Boleano que cambia cuando el objeto esta selecionado o deseleccionado
    private TouchController touchController;//Referencia a TouchController
    private Rigidbody rb; //hace referencia al peso (masa) que contiene cada objeto 
    public bool placed; // booleano que indica si esta colocado el objeto
    public bool validePlaced = false; // booleano que valida que el objeto cuenta con una posición/ ubicación
    public bool isDeleting = false; // booleano que valida si el objeto es eliminado
    private bool isShowingUp = false; // booleano que indica si se esta mostrando el objeto
    private float animationTime = 0; //tiempo de ejecución de la animación
    public float initScale; // escala inicial
    public AnimCurvesObjects animCurves; // Referencia al ScriptableObject AnimCurvesObjects

    [SerializeField] List<ObjectProperties> childs = new List<ObjectProperties>(); // lista de hijos que contienen  ObjectProperties
    public List<Caracteristica> caracteristicas; // lista de las caracteristicas con las que cuenta el objeto 
    public List<Renderer> renderers = new List<Renderer>(); // lista de renders 
    [HideInInspector]
    public Producto producto; // Referencia a la clase Producto
    private Vector3 posChild; //Hace referencia a la posición del objeto hjo

    private Ray rayObjectProperties;
    private RaycastHit hit;

    QueriesController queriesController;
    [SerializeField] private GameObject gizmoObject;
    public enum ItemTypes
    {
        Object,
        Texture
    }
    public enum State { Selected, Moving, Rotating };
    public State state;

    public LayerMask objectLayer;
    public int idProBodPre;

    //bool isObject;  Define si se trata de un objeto, de lo contrario se trata de una textura
    public ItemTypes itemType; //referencia al enum ItemType

    public Material mat;

    public bool instanciated;

    public bool overlapObj;

    public bool touchingTheWall = false;

    private float bounceForce = 200f;
    private float range = 500f;

    #region BoxCollider Zice
    [Header("Valores Default")]
    [SerializeField] private float defaultCenterY; //valor de Center en y al cual deseas disminuir el boxCollider
    [SerializeField] private float defaultSizeY; //valor de Size en y al cual deseas disminuir el boxCollider

    private BoxCollider objectBoxCollider;//Variable que almacena el componente boxCollider para su manipulación 
    private float interactionCenterY;//valor de Center en y para interacción
    private float interactionSizeY;//valor de Size en y para interacción
    #endregion

    private void Awake()
    {
        objectBoxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        touchController = FindObjectOfType<TouchController>();
        isShowingUp = true;
        queriesController = FindObjectOfType<QueriesController>();

        SetRender();
        InitMaterials();

        interactionCenterY = objectBoxCollider.center.y;
        interactionSizeY = objectBoxCollider.size.y;
    }
    public GameObject GismoObject()
    {
        return gizmoObject;
    }
    /*private void OnCollisionEnter(Collision collision)
    {
        // El objeto que ha colisionado
        GameObject otherObject = collision.gameObject;
        // Obtener la capa (layer) del objeto con el que colisiona
        int layer = otherObject.layer;

        // Mostrar el nombre del objeto y su capa
        //Debug.Log("Colisionó con: " + otherObject.name + " en la capa: " + LayerMask.LayerToName(layer));

        if (layer == 8) //wall
        {
            touchingTheWall = true;
            Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
            //Debug.Log("Obtuve el componente rigidbody del gameobject:" + this.gameObject);
            //Vector3 reboundForce = collision.contacts[0].normal * 100f;
            Vector3 reboundForce = transform.position * 200f;
            rb.AddForce(reboundForce, ForceMode.Impulse);
        }
    }*/

    /*private void OnCollisionEnter(Collision collision)
    {
        // El objeto que ha colisionado
        GameObject otherObject = collision.gameObject;
        // Obtener la capa (layer) del objeto con el que colisiona
        int layer = otherObject.layer;
        if (layer == 8) //wall
        {
            touchingTheWall = true;
        }
    }*/

    /*private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            touchingTheWall = false;
        }
    }*/

    public int IdProBodPre
    {
        get { return idProBodPre; } 
        set {  idProBodPre = value; }
    }
    //Modificador de acceso
    public int UniqueId
    {
        get { return _uniqueId; }
        set { _uniqueId = value; }
    }
    private void Start()
    {
        idProBodPre = int.Parse(gameObject.name.Split('(')[0]);
    }

    private void Update()
    {
        if (touchController /*&& placed*/)
        {
            if (touchController.p_objectSelected == this.gameObject)
            {
                //Selecciona
                if (!isSelected)
                {
                    SelecctionObject();
                }
            }
            else
            {
                if (isSelected)
                {
                    DeselectionObject();
                }
            }
        }
        if (touchController.p_objectSelected != this.gameObject && !placed && validePlaced)
        {
            //SetObjectCreated();
        }
        else if (touchController.p_objectSelected != this.gameObject && !placed && !validePlaced)
        {
            isDeleting = true;
          
        }
        if (isShowingUp)
        {
            //SpawnObject();//Para que el objeto aparezca de 0 a 1 en escala
        }
        else if (isDeleting)
        {
            DeleteThis();
        }

        //Bajar al piso, Si el layer del objeto es 9 (piso)
        if (state != State.Moving && Vector3.Distance(posChild, transform.GetChild(0).localPosition) > 0.005f && (objectLayer.value & (1 << 9)) != 0)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, posChild, Time.deltaTime * 4);
        }
        
        RaycastHit[] hits;
        rayObjectProperties = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, rayObjectProperties.direction * range, Color.red, 5f);
        hits = Physics.RaycastAll(rayObjectProperties);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.GetComponent<ObjectProperties>() && Physics.Raycast(Camera.main.transform.position, rayObjectProperties.direction, out hit, range, (1 << 8)))
            {
                touchingTheWall = true;
                //Debug.Log("Estoy tocando la pared... ");
            }
            else
            {
                touchingTheWall = false;
            }
        }
    }

    /// <summary>
    /// Move Element
    /// </summary>
    /// <param name="pos"></param>
    public void MoveTo(Vector3 pos)
    {
        transform.position = Vector3.Lerp(transform.position, pos, moveSpeed * Time.deltaTime);
        Levitate();
    }

    public void MoveAway(Vector3 pos)
    {
        Vector3 oppositeDirection = transform.position + (transform.position - pos);
        transform.position = Vector3.Lerp(transform.position, oppositeDirection, moveSpeed * Time.deltaTime);
        Levitate();
    }

    public void MoveAwayWithPhysics(Vector3 pos)
    {
        Debug.Log("Ingrese a MoveAwayPhysics");

        //Vector3 oppositeDirection = transform.position + (transform.position - pos);
        //transform.position = Vector3.Lerp(transform.position, oppositeDirection, moveSpeed * Time.deltaTime);
        //rb.AddForce(oppositeDirection * bounceForce, ForceMode.Force);
        Vector3 direction = (transform.position - pos).normalized;
        rb.MovePosition(transform.position + moveSpeed * Time.deltaTime * direction);


        //Vector3 direction = transform.position + (transform.position - pos).normalized;
        //transform.position = Vector3.Lerp(transform.position, pos, moveSpeed * Time.deltaTime);
        //Vector3 direction = transform.position;

        //transform.position = Vector3.Lerp(transform.forward,pos, moveSpeed * Time.deltaTime);
        //Vector3 direction = pos += transform.forward * moveSpeed * Time.deltaTime;
        //rb.AddForce(direction.forward * bounceForce, ForceMode.Force);

    }


    /// <summary>
    /// Elevamos un poco los objetos
    /// </summary>

    public void Levitate()
    {
        transform.GetChild(0).transform.localPosition = new Vector3(posChild.x, posChild.y + (0.1f), posChild.z);
        for (int i = 0; i < childs.Count; i++)
            childs[i].Levitate();
    }
    
    public void SetColorbyID(int t)
    {
        idSelected = t;
        ChangeMaterial();
        //Debug.Log("idSelected actual: " + idSelected);
    }

    public void SetColorbyID()
    {
        ChangeMaterial();

    }

    /// <summary>
    /// Cambio de característica en el mueble desde UI RING MENU
    /// </summary>
    /// <param name="t"></param>
    public void SetColor(string t)
    {
        int index = 0;
        int oldIndex = idSelected;


        if (int.TryParse(t, out index))
        {

            //isBouncing = true;
            idSelected = caracteristicas.Find(c => c.id == index).id;
            ChangeMaterial();
        }


    }

    void SetRender()
    {
        var rendss = GetComponentsInChildren<Renderer>();
        var block1 = new MaterialPropertyBlock();
        foreach (var i in rendss)
        {

            foreach (var car in caracteristicas) //Recorre la lista de Catracteristicas
            {
                foreach (var x in car.normal) //Recore la lista "normal" que obtiene los MATERIALES
                {
                    if (i.sharedMaterials.Length > 0) //Verifica si un objeto (i) tiene almenos un metrial asignado 
                    {
                        foreach (var sh in i.sharedMaterials)// Recorre cada material compartido en el objeto i
                        {
                            if (x == sh) // Verifica si el material actual es igual al material x
                            {
                                if (!renderers.Contains(i)) // Verifica si la lista 'renderers' ya no contiene el objeto i
                                    renderers.Add(i); // Si no lo contiene, lo añade a la lista 'renderers'

                            }
                        }
                    }
                    else
                    {
                        if (x == i.sharedMaterial)
                        {
                            if (!renderers.Contains(i))
                                renderers.Add(i);
                        }
                    }
                }
            }
        }
        /*
        var block = new MaterialPropertyBlock();
        block.SetTexture("_BaseMap", manager.texturesGizmos[0]);
        gizmoObject.GetComponent<Renderer>().SetPropertyBlock(block);
        */
    }

    /// <summary>
    /// Función para cambiar el material en cada render del objeto
    /// </summary>
    void ChangeMaterial()
    {
        //int index = -1;
        int indexSelected = 0;
        if (idSelected != 0)
        {
            indexSelected = caracteristicas.FindIndex(c => c.id == idSelected);//Busca el primer elemento en la lista caracteristicas que tenga un id igual a idSelected
        }
        else
        {
            indexSelected = 0;
            //print("Index not found: " + idSelected);
        }

        foreach (var r in renderers)
        {
            var intMaterials = r.sharedMaterials;
            for (int i = 0; i < r.sharedMaterials.Length; i++)
            {
                for (int l = 0; l < caracteristicas.Count; l++)
                {
                    for (int j = 0; j < caracteristicas[l].normal.Length; j++)
                    {
                        if (intMaterials[i] == caracteristicas[l].normal[j])
                        {
                            intMaterials[i] = caracteristicas[indexSelected].normal[j];
                        }
                    }
                }
            }
            r.materials = intMaterials;
        }
    }

    public void InitMaterials()
    {
        Caracteristica ca = new Caracteristica();
        ca.normal = new Material[caracteristicas[0].normal.Length];

        Caracteristica cr = new Caracteristica();
        cr.normal = new Material[caracteristicas[0].normal.Length];
        /*
        for (int i = 0; i < caracteristicas[0].normal.Length; i++)
        {
            ca.normal[i] = Material.Instantiate(FindObjectOfType<ManagerColors>().ok);
            cr.normal[i] = Material.Instantiate(FindObjectOfType<ManagerColors>().error);
        }
        */
        caracteristicas.Add(ca);
        caracteristicas.Add(cr);
    }

    [System.Serializable]
    public class Caracteristica
    {
        public int id;
        public Material[] normal;
      
    }

    public virtual void SetObjectCreated()
    {
        //FindObjectOfType<VibrationManager>().PlayVibration("success");
        placed = true;
        SetColorbyID(idSelected);
        validePlaced = true;

    }

    /// <summary>
    /// Función para seleccionar
    /// </summary>
    private void SelecctionObject()
    {
        placed = true;//Lalo Agrego
        isSelected = true;
        rb.isKinematic = false;
        SetState(State.Selected);
        ChangeBoxColliderSize(interactionCenterY, interactionSizeY);
        //conseguir subcategoria partir de los datos del producto

        SubCategoria lastSubcat01 = queriesController.getlastsubcatego(producto.idProBodPre, producto.idEmpresa, producto.nombreProducto/*nombreSubcategoria*/); ;
        
        //MenuRemplace.SetLastSubcategory(null);
        //MenuRemplace.SetLastSubcategory(producto.subCategoria);

        MenuRemplace.lastSubcategory = null;
        //MenuRemplace.lastSubcategory = producto.subCategoria;
        MenuRemplace.lastSubcategory = lastSubcat01;
        if (gizmoObject != null)
        {
            gizmoObject.SetActive(true); 
        }
    }

    /// <summary>
    /// Función para deseleccionar
    /// </summary>
    private void DeselectionObject()
    {
        isSelected = false;
        rb.isKinematic = true;
        if (gizmoObject != null)
        {
            gizmoObject.SetActive(false);
        }
    }

    /// <summary>
    /// Animación para hacerlo crecer, se ejecuta en el instanciado
    /// </summary>
    void SpawnObject()
    {
        if (isShowingUp)
        {
            if (animationTime >= 1)
            {
                transform.localScale = Vector3.one * initScale;
                animationTime = 0;
                isShowingUp = false;
            }
            else
            {
                animationTime += (Time.deltaTime / animCurves.scaleDuration);
                transform.localScale = Vector3.one * animCurves.m_scaleCurve.Evaluate(animationTime);
            }

        }
    }

    /// <summary>
    /// Animación para eliminar el objeto
    /// </summary>
    void DeleteThis()
    {
        if (animationTime >= 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            animationTime += (Time.deltaTime / animCurves.scaleDuration);
            transform.localScale = Vector3.one * initScale * animCurves.m_scaleOffCurve.Evaluate(animationTime);
        }
    }

    /// <summary>
    /// Agrega todos los componentes de renderers a una lista
    /// </summary>


    public void SetState(State s) {
        state = s;

        switch (state)
        {
            case State.Selected:
                break;
            case State.Moving:
                break;
            case State.Rotating: 
                break;
        }
    }

    /// <summary>
    /// Cambio de los valores Center y Size del BoxCollider del objeto
    /// </summary>
    /// <param name="centerY"></param>
    /// <param name="sizeY"></param>
    private void ChangeBoxColliderSize(float centerY, float sizeY)
    {
        objectBoxCollider.center = new Vector3(objectBoxCollider.center.x, centerY, objectBoxCollider.center.z);
        objectBoxCollider.size = new Vector3(objectBoxCollider.size.x, sizeY, objectBoxCollider.size.z);
    }

    public void ChangeBoxColliderSizeBig()
    {
        ChangeBoxColliderSize(defaultCenterY, defaultSizeY);
    }

    public void Increment()
    {

    }

}


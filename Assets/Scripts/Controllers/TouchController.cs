using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
//using UnityEditor.Rendering;
using DG.Tweening;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.VisualScripting;
//using static UnityEditor.PlayerSettings;

public class TouchController : MonoBehaviour
{
    #region Variables publicas
    public static TouchController sharedInstance;
    #endregion

    #region Variables privadas
    [Header("COMPORTAMIENTO DE OBJETO ACTUAL (VISUALIZACION)")]
    /// <summary>
    /// This variable change when product is selected
    /// </summary>
    [SerializeField] private GameObject objectSelected;

    [SerializeField] private LayerMask activeLayer;
    [SerializeField] private float topYPos = 0;
    [SerializeField] private GameObject fatherProducts;

    private Vector3 cached_p_ObjecSelected_Rotation;
    private RaycastHit hit;
    private bool ray;
    Touch[] touches;
    [SerializeField] private UIRingMenu uIRingMenu;
    [SerializeField] private InputController inputController;
    [SerializeField] private GameObject indicator;
    private Vector3 difPositionMoving;
    [SerializeField] private float lastRotationPlayer;

    //Variables para la rotacion
    static float currentTime;
    bool isFirstFrameWithTwoTouches;
    float cachedTouchAngle;
    float cachedTouchDistance;
    float cachedAugmentationScale;
    const float ScaleRangeMin = 0.1f;
    const float ScaleRangeMax = 2.0f;

    bool isovertutorials = false;
    bool isovertutorials_2 = false;

    private static PointerEventData pointerEventData;
    private float mSpeed = 5;

    private float pushForce = 10f;
    private float maxDistance = 2f;
    #endregion


    #region Events

    public EventHandler OnChangeObjectedSelected;
    #endregion

    public GameObject GetFatherProducts()
    {
        return fatherProducts;
    }

    // Modificador de acceso con la propiedad p_objectSelected
    public GameObject p_objectSelected
    {
        get
        {
            return objectSelected;
        }
        set
        {   if (objectSelected!= value)
            {

            }
            objectSelected = value; 
        }
    }
    public RaycastHit p_hit
    {
        get
        {
            return hit;
        }
        set
        {
            hit = value;    
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        if (p_objectSelected)//Pone el objeto actual en estado selecionado
        {
            p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Selected);
            indicator.SetActive(true);//Activa el indicador
            //difPositionMoving = Vector3.zero;
        }
        else
        {
            indicator.SetActive(false);//Desactiva el indicardor
        }
        if (p_objectSelected && TutorialControl.sharedInstance.IsPlaying() == false) //Para Instanciar los titoriales/Objeto/Textura
        {
            if (p_objectSelected.GetComponent<ObjectProperties>())
            {
                switch (p_objectSelected.GetComponent<ObjectProperties>().itemType)//Revisamos si el producto es Objeto o Textura
                {
                    case ObjectProperties.ItemTypes.Object:
                        if (isovertutorials == false)
                        {
                            TutorialControl.sharedInstance.InstatiateTutorial(tutorialItems.touchFloor);//PRIMER TUTURIAL
                        }
                        break;
                    case ObjectProperties.ItemTypes.Texture:
                        if (isovertutorials_2 == false)
                        {
                            TutorialControl.sharedInstance.InstatiateTutorial(tutorialItems.touchTexture);//PRIMER TUTURIAL
                            isovertutorials_2 = true;
                        }
                        break;
                }
            }
        }
        //Verifica que el toque o click no interfiera en el algun canvas o interfaz de UI
        if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && !uIRingMenu.moveEnable && !inputController.isMoving && !inputController.isRotating && !IsPointerOverUIObject())
        {
            HandleInput();
        }
        //Eliminar el tutorial de rotar en pc
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (TutorialControl.sharedInstance.IsPlaying() && TutorialControl.sharedInstance.CurrentTutorial() == tutorialItems.rotateObject)
            {
                FindObjectOfType<TutorialItem>().toDestroy = true;
            }
        }
        if (p_objectSelected != null && !p_objectSelected.GetComponent<ObjectProperties>().placed)//si placed es false
        {
            Debug.Log("Ingrese a SetObjectCreated");
            SetObjectCreated();
        }

    }

    /// <summary>
    /// Establece el objeto para el cambio de color, si es que lo hay
    /// </summary>
    public void SetObjectCreated() {
        p_objectSelected.GetComponent<ObjectProperties>().SetObjectCreated();
    }

    /// <summary>
    /// El p_objectselected es un objeto
    /// </summary>
    void is_Object()
    {
        if (p_objectSelected.GetComponent<ObjectProperties>().instanciated == false)
        {
            if (p_hit.collider && TutorialControl.sharedInstance.CurrentTutorial() == tutorialItems.touchFloor)
            {
                TutorialControl.sharedInstance.InstatiateTutorial(tutorialItems.moveObject);//segundo_tutorial
            }

            RaycastHit[] hits;
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Camera.main.transform.position, rayo.direction * 500f, Color.white, 5f);
            hits = Physics.RaycastAll(rayo);

            for (int i = 0; i < hits.Length; i++)
            {
                if (Physics.Raycast(Camera.main.transform.position, rayo.direction, out hit, 500f, activeLayer | 1 << 12 /*11*/))//activelayer = la capa que toque y que le corresponde el producto
                {
                    //REVISAR COLOCACION DE UN CUADRO
                    
                    if (p_objectSelected.GetComponent<ObjectProperties>().is_picture)
                    {
                        //p_objectSelected.transform.LookAt(new Vector3(transform.position.x, p_objectSelected.transform.position.y, transform.position.z));
                        //p_objectSelected.transform.LookAt(new Vector3(p_objectSelected.transform.position.x, p_objectSelected.transform.position.y, transform.position.z));
                        //p_objectSelected.transform.LookAt(transform);
                        
                        Vector3 direction = transform.position - p_objectSelected.transform.position;
                        direction.y = 0;// Mantener el objeto alineado solo sobre el eje Y
                        p_objectSelected.transform.rotation = Quaternion.LookRotation(direction);
                        
                    }
                    
                    //-------------  
                    //--------------Obtiene la rotacion en Y del payer---------------
                    // Obtener la rotación en Y en el rango [0, 360]
                    float rotacionY = transform.eulerAngles.y;
                    // Convertir el ángulo a un rango de [-180, 180]
                    if (rotacionY > 180)
                    {
                        rotacionY -= 360;
                    }
                    // Actualizar la variable con la rotación ajustada
                    lastRotationPlayer = rotacionY;
                    //-----------------------------------------------------------------
                    // Establecer la nueva rotación en el eje Y
                    float newRotationY = lastRotationPlayer;
                    // Obtener la posición actual
                    Vector3 currentPosition = p_objectSelected.transform.position;
                    // Establecer la nueva rotación solo en el eje Y
                    //p_objectSelected.transform.rotation = Quaternion.Euler(currentPosition.x, newRotationY, currentPosition.z);

                    p_objectSelected.transform.position = p_hit.point;
                    p_objectSelected.transform.rotation = Quaternion.Euler(p_objectSelected.transform.rotation.eulerAngles.x, newRotationY, p_objectSelected.transform.rotation.eulerAngles.z);  // Cambiar solo la rotación en Y


                    //Debug.Log("ESTOY RECOLOCANDO EL OBJ");
                    p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Selected);
                    
                    if (p_objectSelected.transform.position.y > topYPos)//rectificación de pocisionemiento del objeto en Y
                    {
                        topYPos = p_objectSelected.transform.position.y;
                    }
                    p_objectSelected.GetComponent<ObjectProperties>().instanciated = true;
                    //---------------
                }
            }
        }
        else if (p_objectSelected.GetComponent<ObjectProperties>().instanciated == true && Input.GetMouseButton(0))//Pc
        {
            if (TutorialControl.sharedInstance.CurrentTutorial() == tutorialItems.moveObject)
            {
                TutorialControl.sharedInstance.InstatiateTutorial(tutorialItems.rotateObject);//tercer_tutorial
                isovertutorials = true;
            }

            p_objectSelected.layer = 0;

            RaycastHit[] hits;
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Camera.main.transform.position, rayo.direction * 500f, Color.white, 5f);
            hits = Physics.RaycastAll(rayo);

            for (int i = 0; i < hits.Length; i++)
            {
                //Verifica que el objeto seleccionado que va a mover esta en estado seleccionado y que no toque algun objeto que ya este puesto (11,placedObjects)
                if (p_objectSelected.GetComponent<ObjectProperties>().state == ObjectProperties.State.Selected && p_hit.collider.gameObject.layer != 11)
                {
                    if (hits[i].collider.GetComponent<ObjectProperties>() && Physics.Raycast(Camera.main.transform.position, rayo.direction, out hit, 500f, activeLayer | 1 << 12 /*11*/))//activelayer = la capa que toque y que le corresponde el producto
                    {
                        //REVISAR COLOCACION DE UN CUADRO
                        if (p_objectSelected.GetComponent<ObjectProperties>().is_picture)
                        {
                            //p_objectSelected.transform.LookAt(new Vector3(transform.position.x, p_objectSelected.transform.position.y, transform.position.z));
                            //p_objectSelected.transform.LookAt(new Vector3(p_objectSelected.transform.position.x, p_objectSelected.transform.position.y, p_objectSelected.transform.position.z));
                            //p_objectSelected.transform.LookAt(transform);
                            Vector3 direction = p_objectSelected.transform.position - transform.position;
                            direction.y = 0;// Mantener el objeto alineado solo sobre el eje Y
                            p_objectSelected.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

                            //-------------------------Movimiento del objeto si el usuario toca la base del mismo
                            BoxCollider boxCollider = p_objectSelected.GetComponent<BoxCollider>();
                            Vector3 basePosition = p_objectSelected.transform.position - new Vector3(0, boxCollider.size.y / 2, 0);

                            if (Vector3.Distance(p_hit.point, basePosition) < boxCollider.size.y / 1)
                            {
                                p_objectSelected.transform.position = p_hit.point;
                            }

                            p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Moving);

                            if (p_objectSelected.transform.position.y > topYPos)//rectificación de pocisionamiento del objeto en Y
                            {
                                topYPos = p_objectSelected.transform.position.y;
                            }
                        }
                        //Si el layer del objeto es 9 (piso / floor) y colisiona con pared
                        if (((p_objectSelected.GetComponent<ObjectProperties>().objectLayer.value & (1 << 9)) != 0)
                           && p_objectSelected.GetComponent<ObjectProperties>().touchingTheWall)
                        {
                            //Debug.Log("Estoy tocando 1");

                            Rigidbody rb = p_objectSelected.GetComponent<Rigidbody>();
                            //rb.isKinematic = false;
                            //rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                            
                            Vector3 positionChange = new Vector3(p_objectSelected.transform.forward.x, 0, p_objectSelected.transform.forward.z);

                            rb.MovePosition(p_objectSelected.transform.position + mSpeed * Time.deltaTime * positionChange);

                            //rb.useGravity = false;
                            //BoxCollider boxCollider = p_objectSelected.GetComponent<BoxCollider>();
                            //Vector3 direction = (p_objectSelected.transform.position - transform.position).normalized;
                            //Vector3 basePosition = (p_objectSelected.transform.position - new Vector3(0, boxCollider.size.y / 2, 0)).normalized;
                          
                            /*if (Vector3.Distance(p_hit.point, basePosition) < boxCollider.size.y / 1)
                            {
                                p_objectSelected.transform.position = p_hit.point;
                            }*/
                         
                            //--------------------------Pasa valor a difPositionMoving para elevar el objeto----------------------
                            //Transform augmentationObject = null;
                            /*if (p_objectSelected)// p_objectselected lleno
                            {
                                augmentationObject = p_objectSelected.transform;
                            }*/

                            /*if (difPositionMoving == Vector3.zero)
                            {
                                difPositionMoving = augmentationObject.transform.position - p_objectSelected.GetComponent<ObjectProperties>().GismoObject().transform.position;
                                difPositionMoving.y = 0;
                            }*/

                            //p_objectSelected.GetComponent<ObjectProperties>().MoveAwayWithPhysics(p_objectSelected.GetComponent<ObjectProperties>().GismoObject().transform.position);

                            /*if (Physics.Raycast(rayo, out hit, maxDistance))
                            {
                                
                                if (hit.collider.gameObject.TryGetComponent<Rigidbody>(out var rb))
                                {
                                    Debug.Log("RB NO NULO");
                                    Vector3 forceDirection = hit.point - p_objectSelected.transform.position;
                                    forceDirection.Normalize();

                                    rb.AddForce(forceDirection * pushForce, ForceMode.Impulse);
                                }
                            }*/

                        }
                        //Si el layer del objeto es 9 (piso)
                        else if ((p_objectSelected.GetComponent<ObjectProperties>().objectLayer.value & (1 << 9)) != 0
                            && !p_objectSelected.GetComponent<ObjectProperties>().touchingTheWall) //MOVIMIENTO DEL OBJETO 
                        {
                            //Debug.Log(" Estoy tocando 2");
                            //Rigidbody rb = p_objectSelected.GetComponent<Rigidbody>();
                            //rb.isKinematic = false;
                            //rb.interpolation = RigidbodyInterpolation.Interpolate;
                            //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        
                            //--------------------------Pasa valor a difPositionMoving para elevar el objeto----------------------
                            Transform augmentationObject = null;
                            if (p_objectSelected)// p_objectselected lleno
                            {
                                augmentationObject = p_objectSelected.transform;
                            }

                            if (difPositionMoving == Vector3.zero)
                            {
                                difPositionMoving = augmentationObject.transform.position - p_objectSelected.GetComponent<ObjectProperties>().GismoObject().transform.position;
                                difPositionMoving.y = 0;
                            }
                            //------------------------------------------------------------------------------------------------
                            p_objectSelected.GetComponent<ObjectProperties>().MoveTo(p_objectSelected.GetComponent<ObjectProperties>().GismoObject().transform.position + difPositionMoving);

                            //-------------------------Movimiento del objeto si el usuario toca la base del mismo
                            BoxCollider boxCollider = p_objectSelected.GetComponent<BoxCollider>();
                            Vector3 basePosition = p_objectSelected.transform.position - new Vector3(0, boxCollider.size.y / 2, 0);

                            if (Vector3.Distance(p_hit.point, basePosition) < boxCollider.size.y / 1)
                            {
                                p_objectSelected.transform.position = p_hit.point;
                            }

                            p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Moving);

                            if (p_objectSelected.transform.position.y > topYPos)//rectificación de pocisionemiento del objeto en Y
                            {
                                topYPos = p_objectSelected.transform.position.y;
                            }
                            //-------------Estoy moviendome en el piso y toque una pared---------------
                            /*if (p_objectSelected.GetComponent<ObjectProperties>().touchingTheWall)
                            {
                                Debug.Log("Estoy en el piso y tocando una pared");
                            
                            }*/
                        }
                        //Si el layer del objeto es 10 (techo / ceiling) 
                        else if ((p_objectSelected.GetComponent<ObjectProperties>().objectLayer.value & (1 << 10)) != 0)
                        {
                            //Debug.Log(" Estoy tocando TECHO");
                            //-------------------------Movimiento del objeto si el usuario toca la base del mismo
                            BoxCollider boxCollider = p_objectSelected.GetComponent<BoxCollider>();
                            Vector3 basePosition = p_objectSelected.transform.position - new Vector3(0, boxCollider.size.y / 2, 0);

                            if (Vector3.Distance(p_hit.point, basePosition) < boxCollider.size.y / 1)
                            {
                                p_objectSelected.transform.position = p_hit.point;
                            }

                            p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Moving);

                            if (p_objectSelected.transform.position.y > topYPos)//rectificación de pocisionamiento del objeto en Y
                            {
                                topYPos = p_objectSelected.transform.position.y;
                            }
                        }

                        //Si el layer del objeto es 9 (piso)
                            /*if ((p_objectSelected.GetComponent<ObjectProperties>().objectLayer.value & (1 << 9)) != 0)
                            {
                                Rigidbody rb = p_objectSelected.GetComponent<Rigidbody>();
                                rb.isKinematic = false;
                                rb.interpolation = RigidbodyInterpolation.None;
                                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                                // Obtener la posición del GismoObject

                                Vector3 gismoPosition = p_objectSelected.GetComponent<ObjectProperties>().GismoObject().transform.position;

                                // Calcular difPositionMoving si es Vector3.zero
                                if (difPositionMoving == Vector3.zero)
                                {
                                    difPositionMoving = rb.position - gismoPosition;
                                    difPositionMoving.y = 0; // Mantener la altura constante
                                }
                                // Calcular la nueva posición
                                Vector3 newPosition = gismoPosition + difPositionMoving;

                                // Mover el Rigidbody a la nueva posición utilizando MovePosition
                                rb.MovePosition(Vector3.Lerp(rb.position, newPosition, mSpeed * Time.deltaTime));

                                p_objectSelected.GetComponent<ObjectProperties>().Levitate();

                                //-------------------------Movimiento del objeto si el usuario toca la base del mismo
                                BoxCollider boxCollider = p_objectSelected.GetComponent<BoxCollider>();
                                Vector3 basePosition = rb.position - new Vector3(0, boxCollider.size.y / 2, 0);

                                if (Vector3.Distance(hit.point, basePosition) < boxCollider.size.y / 1)
                                {
                                    //p_objectSelected.transform.position = hit.point;
                                    rb.position = hit.point;
                                }
                                //_--------------------------------------------------------------
                                p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Moving);

                                if (p_objectSelected.transform.position.y > topYPos)//rectificación de pocisionemiento del objeto en Y
                                {
                                    topYPos = p_objectSelected.transform.position.y;
                                }
                            }*/
                    }
                }   
            }
        }
        else if (p_objectSelected.GetComponent<ObjectProperties>().instanciated == true && Input.GetTouch(0).phase == TouchPhase.Moved)//Movil
        {
            if (TutorialControl.sharedInstance.CurrentTutorial() == tutorialItems.moveObject)
            {
                TutorialControl.sharedInstance.InstatiateTutorial(tutorialItems.rotateObject);//tercer_tutorial
            }

            p_objectSelected.layer = 0;

            RaycastHit[] hits;
            Ray rayo = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            Debug.DrawRay(Camera.main.transform.position, rayo.direction * 500f, Color.white, 5f);
            hits = Physics.RaycastAll(rayo);

            for (int i = 0; i < hits.Length; i++)
            {
                //Verifica que el objeto seleccionado que va a mover esta en estado seleccionado y que no toque algun objeto que ya este puesto (11,placedObjects)
                if (p_objectSelected.GetComponent<ObjectProperties>().state == ObjectProperties.State.Selected && p_hit.collider.gameObject.layer != 11)
                {
                    if (hits[i].collider.GetComponent<ObjectProperties>() && Physics.Raycast(Camera.main.transform.position, rayo.direction, out hit, 500f, activeLayer | 1 << 10 /*11*/))
                    {
                        //REVISAR COLOCACION DE UN CUADRO
                        if (p_objectSelected.GetComponent<ObjectProperties>().is_picture)
                        {
                            //p_objectSelected.transform.LookAt(new Vector3(transform.position.x, p_objectSelected.transform.position.y, transform.position.z));
                            Vector3 direction = p_objectSelected.transform.position - transform.position;
                            direction.y = 0;// Mantener el objeto alineado solo sobre el eje Y
                            p_objectSelected.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

                            //-------------------------Movimiento del objeto si el usuario toca la base del mismo
                            BoxCollider boxCollider = p_objectSelected.GetComponent<BoxCollider>();
                            Vector3 basePosition = p_objectSelected.transform.position - new Vector3(0, boxCollider.size.y / 2, 0);

                            if (Vector3.Distance(p_hit.point, basePosition) < boxCollider.size.y / 1)
                            {
                                p_objectSelected.transform.position = p_hit.point;
                            }

                            p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Moving);

                            if (p_objectSelected.transform.position.y > topYPos)//rectificación de pocisionamiento del objeto en Y
                            {
                                topYPos = p_objectSelected.transform.position.y;
                            }
                        }

                        //Si el layer del objeto es 9 (piso / floor) y colisiona con pared
                        if (((p_objectSelected.GetComponent<ObjectProperties>().objectLayer.value & (1 << 9)) != 0)
                           && p_objectSelected.GetComponent<ObjectProperties>().touchingTheWall)
                        {
                            Debug.Log("Estoy tocando 1");
                            Rigidbody rb = p_objectSelected.GetComponent<Rigidbody>();
                            rb.isKinematic = false;
                            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                            Vector3 positionChange = new Vector3(p_objectSelected.transform.forward.x, 0, p_objectSelected.transform.forward.z);

                            rb.MovePosition(p_objectSelected.transform.position + mSpeed * Time.deltaTime * positionChange);

                        }
                        //Si el layer del objeto es 9 (piso)
                        else if ((p_objectSelected.GetComponent<ObjectProperties>().objectLayer.value & (1 << 9)) != 0
                            && !p_objectSelected.GetComponent<ObjectProperties>().touchingTheWall) //MOVIMIENTO DEL OBJETO 
                        {
                            Debug.Log(" Estoy tocando 2");
                            //Rigidbody rb = p_objectSelected.GetComponent<Rigidbody>();
                            //rb.isKinematic = false;
                            //rb.interpolation = RigidbodyInterpolation.Interpolate;
                            //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                            //--------------------------Pasa valor a difPositionMoving para elevar el objeto----------------------
                            Transform augmentationObject = null;
                            if (p_objectSelected)// p_objectselected lleno
                            {
                                augmentationObject = p_objectSelected.transform;
                            }

                            if (difPositionMoving == Vector3.zero)
                            {
                                difPositionMoving = augmentationObject.transform.position - p_objectSelected.GetComponent<ObjectProperties>().GismoObject().transform.position;
                                difPositionMoving.y = 0;
                            }
                            //------------------------------------------------------------------------------------------------
                            p_objectSelected.GetComponent<ObjectProperties>().MoveTo(p_objectSelected.GetComponent<ObjectProperties>().GismoObject().transform.position + difPositionMoving);

                            //-------------------------Movimiento del objeto si el usuario toca la base del mismo
                            BoxCollider boxCollider = p_objectSelected.GetComponent<BoxCollider>();
                            Vector3 basePosition = p_objectSelected.transform.position - new Vector3(0, boxCollider.size.y / 2, 0);

                            if (Vector3.Distance(p_hit.point, basePosition) < boxCollider.size.y / 1)
                            {
                                p_objectSelected.transform.position = p_hit.point;
                            }

                            p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Moving);

                            if (p_objectSelected.transform.position.y > topYPos)//rectificación de pocisionemiento del objeto en Y
                            {
                                topYPos = p_objectSelected.transform.position.y;
                            }
                            //-------------Estoy moviendome en el piso y toque una pared---------------
                            /*if (p_objectSelected.GetComponent<ObjectProperties>().touchingTheWall)
                            {
                                Debug.Log("Estoy en el piso y tocando una pared");
                            
                            }*/
                        }
                        //Si el layer del objeto es 10 (techo / ceiling) 
                        else if ((p_objectSelected.GetComponent<ObjectProperties>().objectLayer.value & (1 << 10)) != 0)
                        {
                            Debug.Log(" Estoy tocando TECHO");
                            //-------------------------Movimiento del objeto si el usuario toca la base del mismo
                            BoxCollider boxCollider = p_objectSelected.GetComponent<BoxCollider>();
                            Vector3 basePosition = p_objectSelected.transform.position - new Vector3(0, boxCollider.size.y / 2, 0);

                            if (Vector3.Distance(p_hit.point, basePosition) < boxCollider.size.y / 1)
                            {
                                p_objectSelected.transform.position = p_hit.point;
                            }

                            p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Moving);

                            if (p_objectSelected.transform.position.y > topYPos)//rectificación de pocisionamiento del objeto en Y
                            {
                                topYPos = p_objectSelected.transform.position.y;
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// El p_objectselected es una textura
    /// </summary>
    void is_Texture()
    {
        ray = Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, out hit, 50f);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 50f /*500f*/, Color.blue, 5f);
        if (ray)
        {
            if (p_hit.collider.gameObject.layer == p_objectSelected.layer /*|| hit.collider.gameObject.layer == 10*/)
            {
                if (TutorialControl.sharedInstance.IsPlaying() && TutorialControl.sharedInstance.CurrentTutorial() == tutorialItems.touchTexture)
                {
                    FindObjectOfType<TutorialItem>().toDestroy = true;
                }
                // Cambiar el material al objeto golpeado
                p_hit.collider.gameObject.GetComponent<MeshRenderer>().material = p_objectSelected.GetComponent<ObjectProperties>().mat;
            } 
            
            // 8 wall(pared), 9floor(piso), 10 celling()techo.
            if (p_hit.collider.gameObject.layer != 8 && p_hit.collider.gameObject.layer != 9 && p_objectSelected.layer != 9)//para que el objeto de pared tambein pinte techos, pero no pisos
            {
                if (TutorialControl.sharedInstance.IsPlaying() && TutorialControl.sharedInstance.CurrentTutorial() == tutorialItems.touchTexture)
                {
                    FindObjectOfType<TutorialItem>().toDestroy = true;
                }
                // Cambiar el material al objeto golpeado
                p_hit.collider.gameObject.GetComponent<MeshRenderer>().material = p_objectSelected.GetComponent<ObjectProperties>().mat;
            }
            
        }
    }

    void HandleInput()
    {
        Transform augmentationObject = null;
        if (p_objectSelected)// p_objectselected lleno
        {
            activeLayer = p_objectSelected.GetComponent<ObjectProperties>().objectLayer;
            augmentationObject = p_objectSelected.transform;
        }

        if (TutorialControl.sharedInstance.IsPlaying() && TutorialControl.sharedInstance.CurrentTutorial() == tutorialItems.welcome)
        {
            FindObjectOfType<TutorialItem>().toDestroy = true;
        }

        Ray ray;
        if (Input.touchCount == 1)//Muebe el objeto
        {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            Debug.DrawRay(Camera.main.transform.position, ray.direction * 500f, Color.black, 5f);
            bool raycastHit = Physics.Raycast(Camera.main.transform.position, ray.direction, out hit, 500f, activeLayer | 1 << 11);

            if (p_objectSelected != null)// p_objectselected lleno
            {
                if (p_objectSelected.GetComponent<ObjectProperties>())
                {
                    switch (p_objectSelected.GetComponent<ObjectProperties>().itemType)//Revisamos si el producto es Objeto o Textura
                    {
                        case ObjectProperties.ItemTypes.Object:
                            is_Object();
                            break;
                        case ObjectProperties.ItemTypes.Texture:
                            is_Texture();
                            break;
                    }
                }

            }
            else // p_objectSelected vacio
            {
                if (p_hit.collider.gameObject.layer == 11 && p_hit.collider.gameObject.GetComponent<ObjectProperties>())
                {
                    p_objectSelected = p_hit.collider.gameObject;
                    is_Object();
                }
            }
        }  //CONTROLADOR DE MOVIL
        else if (Input.touchCount == 2)//Rotar el objeto
        {
            if (TutorialControl.sharedInstance.IsPlaying() && TutorialControl.sharedInstance.CurrentTutorial() == tutorialItems.rotateObject)
            {
                FindObjectOfType<TutorialItem>().toDestroy = true;
            }

            this.touches = Input.touches;
            currentTime = 0;
            float currentTouchDistance = Vector2.Distance(this.touches[0].position, this.touches[1].position);
            float diff_y = this.touches[0].position.y - this.touches[1].position.y;
            float diff_x = this.touches[0].position.x - this.touches[1].position.x;
            float currentTouchAngle = Mathf.Atan2(diff_y, diff_x) * Mathf.Rad2Deg;

            if (this.isFirstFrameWithTwoTouches)
            {
                this.cachedTouchDistance = currentTouchDistance;
                this.cachedTouchAngle = currentTouchAngle;
                this.isFirstFrameWithTwoTouches = false;
            }

            float angleDelta = currentTouchAngle - this.cachedTouchAngle;
            float scaleMultiplier = (currentTouchDistance / this.cachedTouchDistance);
            float scaleAmount = this.cachedAugmentationScale * scaleMultiplier;
            float scaleAmountClamped = Mathf.Clamp(scaleAmount, ScaleRangeMin, ScaleRangeMax);

            Vector3 rot = Vector3.zero;
            if (p_objectSelected.GetComponent<ObjectProperties>().is_picture) //APLICA LA ROTACION EN EL EJE Z
            {
                //rot = this.cached_p_ObjecSelected_Rotation - new Vector3(0, 0, -angleDelta * 3f);
                rot = augmentationObject.localEulerAngles;  // Obtiene la rotación actual local
                rot.z -= angleDelta * 0.2f;  // Solo ajusta el componente Z local
            }
            else//APLICA LA ROTACION EN EL EJE Y
            {
                rot = this.cached_p_ObjecSelected_Rotation - new Vector3(0, -angleDelta * 3f, 0);
            }
            
            augmentationObject.localEulerAngles = rot;
            p_objectSelected.GetComponent<ObjectProperties>().SetState(ObjectProperties.State.Rotating);

        }
        else if (Input.GetMouseButton(0)) //CONTROLADOR DE PC
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool raycastHit = Physics.Raycast(Camera.main.transform.position, ray.direction, out hit, 500f, activeLayer | 1 << 11);

            if (p_objectSelected != null)// p_objectselected lleno
            {
                if (p_objectSelected.GetComponent<ObjectProperties>())//Revisamos si el producto es Objeto o Textura
                {
                    switch (p_objectSelected.GetComponent<ObjectProperties>().itemType)
                    {
                        case ObjectProperties.ItemTypes.Object:
                            is_Object();
                            break;
                        case ObjectProperties.ItemTypes.Texture:
                            is_Texture();
                            break;
                    }
                }
            }
            else
            {
                if (p_hit.collider.gameObject.layer == 11 && p_hit.collider.gameObject.GetComponent<ObjectProperties>())
                {
                    p_objectSelected = p_hit.collider.gameObject;
                    is_Object();
                }
            }
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Verifica si se encuentra sobre un objeto de interfaz de usuario
    /// </summary>
    /// <returns></returns>
    private bool IsPointerOverUIObject()
    {
        if (pointerEventData == null)
        {
            pointerEventData = new PointerEventData(EventSystem.current);
        }

        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        return results.Count > 0;
    }
}

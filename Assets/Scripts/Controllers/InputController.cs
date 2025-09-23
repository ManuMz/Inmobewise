using UnityEngine;

public class InputController : MonoBehaviour
{
    #region Variables privadas
    [SerializeField] private CharacterController CharacterController;//---------------Revisar si dejarlo publico.......(1)
    [SerializeField] private Joystick joystickMove; //Yostick para mover el jugador.
    [SerializeField] private Joystick joystickGiro;//Yostick para rotar el jugador.
    [SerializeField] private Transform player; //Referencia al objeto de jugador.
    [SerializeField] private float speed = 10f; //Velocidad del jugador

    [SerializeField] private float speedGiro = 0.5f;//Velocidad del giro
    [SerializeField] private float gravity = 9.8f;//Gravedad aplicada

    public bool isMoving = false;
    public bool isRotating = false;

    Transform cam;
    float x;
    float z;
    Vector3 move;
    float rotateV;
    float rotateH;
    #endregion


    private void Start()
    {
        cam = Camera.main.transform;
    }
    private void Update()
    {
        Move();
        Rotate();
    }
    /// <summary>
    /// Mueve el jugador
    /// </summary>
    void Move()
    {
        x = joystickMove.Horizontal + Input.GetAxis("Horizontal");
        z = joystickMove.Vertical + Input.GetAxis("Vertical");

        move = player.right * x + player.forward * z;

        SetGravity();

        isMoving = x != 0 || z != 0;

        CharacterController.Move(move * speed * Time.deltaTime);
    }

    /// <summary>
    /// Rota el jugador
    /// </summary>
    void Rotate()
    {
        rotateH = joystickGiro.Horizontal * speedGiro;
        rotateV = -(joystickGiro.Vertical * speedGiro);
        cam.Rotate(rotateV, 0, 0);
        player.Rotate(0, rotateH, 0);

        isRotating = Mathf.Abs(rotateH) > 0.1f || Mathf.Abs(rotateV) > 0.1f;
    }
    /// <summary>
    /// Establece la gravedad en el eje Y
    /// </summary>
    void SetGravity()
    {
        move.y = -gravity;
    }
}


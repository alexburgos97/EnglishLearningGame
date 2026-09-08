using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movimiento")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float rotationSpeed = 120f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Referencias")]
    [Tooltip("Cámara que sigue al jugador desde atrás. Debe ser hija del jugador para seguir su giro automáticamente.")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private float currentWalkSpeed;
    private float currentRunSpeed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        controller = GetComponent<CharacterController>();
        currentWalkSpeed = walkSpeed;
        currentRunSpeed = runSpeed;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float turn = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        // Controles clásicos: A/D giran al jugador, W/S avanzan/retroceden en la dirección a la que mira.
        transform.Rotate(Vector3.up, turn * rotationSpeed * Time.deltaTime);

        bool grounded = controller.isGrounded;
        if (grounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;

        bool running = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float speed = running ? currentRunSpeed : currentWalkSpeed;

        controller.Move(transform.forward * forward * speed * Time.deltaTime);

        if (grounded && Input.GetButtonDown("Jump"))
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    /// <summary>Usado por BridgeSpeedBoost y mecánicas similares que antes reescribían la velocidad del avatar de Spatial.</summary>
    public void SetSpeed(float newWalkSpeed, float newRunSpeed)
    {
        currentWalkSpeed = newWalkSpeed;
        currentRunSpeed = newRunSpeed;
    }

    public void ResetSpeed()
    {
        currentWalkSpeed = walkSpeed;
        currentRunSpeed = runSpeed;
    }

    /// <summary>Reposiciona al jugador de forma segura para un CharacterController (reemplaza a SpatialBridge...avatar.SetPositionRotation).</summary>
    public void Teleport(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        verticalVelocity = Vector3.zero;
        controller.enabled = true;
    }
}

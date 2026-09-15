using System.Collections;
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

    [Header("Escalones (escaleras con MeshCollider)")]
    [Tooltip("CharacterController.stepOffset no funciona de forma confiable contra MeshCollider no convexos (como escaleras modeladas a mano) — la cápsula se traba en el canto de cada escalón sin importar el Slope Limit. Este raycast asistido sube al jugador un escalón cuando detecta un obstáculo bajo con vía libre por encima.")]
    public bool enableStepAssist = true;
    [Tooltip("Altura máxima de escalón que se sube automáticamente.")]
    public float maxStepHeight = 0.4f;
    [Tooltip("Distancia hacia adelante del raycast que detecta el escalón.")]
    public float stepCheckDistance = 0.5f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private float currentWalkSpeed;
    private float currentRunSpeed;
    private float originalJumpHeight;
    private Coroutine jumpBoostCoroutine;

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

        Vector3 moveDir = transform.forward * forward;

        if (enableStepAssist && grounded && Mathf.Abs(forward) > 0.01f)
            HandleStepClimbing(moveDir);

        controller.Move(moveDir * speed * Time.deltaTime);

        if (grounded && Input.GetButtonDown("Jump"))
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Empuja al jugador un escalón hacia arriba cuando hay un obstáculo bajo (el canto de un
    /// escalón) pero vía libre por encima de esa altura, en vez de dejar que se trabe contra el
    /// borde. Necesario porque stepOffset del CharacterController no se aplica de forma confiable
    /// contra escaleras con MeshCollider no convexo.
    /// </summary>
    private void HandleStepClimbing(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude < 0.0001f) return;
        Vector3 dir = moveDirection.normalized;

        Vector3 lowerOrigin = transform.position + Vector3.up * 0.05f;
        Vector3 upperOrigin = transform.position + Vector3.up * maxStepHeight;

        bool blockedLow = Physics.Raycast(lowerOrigin, dir, stepCheckDistance);
        bool blockedHigh = Physics.Raycast(upperOrigin, dir, stepCheckDistance);

        if (blockedLow && !blockedHigh)
            controller.Move(Vector3.up * maxStepHeight);
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

    /// <summary>Usado por JumpBoost: eleva jumpHeight temporalmente y lo revierte solo después de la duración indicada, sin depender de que el jugador siga tocando ningún trigger.</summary>
    public void ApplyJumpBoost(float boostedJumpHeight, float duration)
    {
        if (jumpBoostCoroutine == null)
            originalJumpHeight = jumpHeight;
        else
            StopCoroutine(jumpBoostCoroutine);

        jumpHeight = boostedJumpHeight;
        jumpBoostCoroutine = StartCoroutine(JumpBoostCountdown(duration));
    }

    private IEnumerator JumpBoostCountdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        jumpHeight = originalJumpHeight;
        jumpBoostCoroutine = null;
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

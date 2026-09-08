using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.2f, 0f);

    [Header("Distancia")]
    public float distance = 5f;
    public float minDistance = 1.2f;
    public float followSmoothTime = 0.12f;

    [Header("Orbita con el mouse")]
    public float mouseSensitivity = 3f;
    public float minPitch = 5f;
    public float maxPitch = 60f;
    public float startPitch = 15f;

    [Header("Colisión de cámara")]
    [Tooltip("Capas contra las que la cámara evita atravesar terreno/objetos.")]
    public LayerMask collisionMask = ~0;

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    void Start()
    {
        pitch = startPitch;
        if (target != null)
            yaw = target.eulerAngles.y;
    }

    void Update()
    {
        HandleCursorLock();

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + offset;
        Vector3 desiredPosition = pivot - rotation * Vector3.forward * distance;

        float finalDistance = distance;
        if (Physics.Linecast(pivot, desiredPosition, out RaycastHit hit, collisionMask, QueryTriggerInteraction.Ignore))
            finalDistance = Mathf.Clamp(hit.distance, minDistance, distance);

        desiredPosition = pivot - rotation * Vector3.forward * finalDistance;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, followSmoothTime);
        transform.LookAt(pivot);
    }

    private void HandleCursorLock()
    {
        bool clickOnGame = Input.GetMouseButtonDown(0)
            && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject());

        if (clickOnGame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

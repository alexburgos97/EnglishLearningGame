using UnityEngine;

public class BridgeBlock : MonoBehaviour
{
    [Header("Posición destino en el puente")]
    public Transform bridgePosition;

    [HideInInspector] public bool triggerActive = false;

    private bool isPlaced = false;
    private bool isMoving = false;
    private Vector3 targetPos;
    private Vector3 originalPos;
    private float moveSpeed = 3f;

    void Start()
    {
        originalPos = transform.position;
    }

    public void ActivateTrigger()
    {
        triggerActive = true;
    }

    public void MoveToPosition()
    {
        if (isPlaced) return;
        isPlaced = true;
        isMoving = true;
        targetPos = bridgePosition.position;
    }

    public void ResetPosition()
    {
        isPlaced = false;
        isMoving = false;
        transform.position = originalPos;
        triggerActive = false;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            transform.position = targetPos;
            isMoving = false;
        }
    }
}
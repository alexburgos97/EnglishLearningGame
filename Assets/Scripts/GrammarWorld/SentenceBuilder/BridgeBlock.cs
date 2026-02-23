using UnityEngine;

public class BridgeBlock : MonoBehaviour
{
    [Header("Posición destino en el puente")]
    public Transform bridgePosition;
    // Aquí arrastrarás el GameObject vacío que marca
    // dónde debe quedar este bloque en el puente

    [HideInInspector] public bool triggerActive = false;

    private bool isPlaced = false;
    private bool isMoving = false;
    private Vector3 targetPos;
    private float moveSpeed = 3f;

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
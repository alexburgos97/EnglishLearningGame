using UnityEngine;


public class RespawnPoint : MonoBehaviour
{
    public Transform puntoReaparicion;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // respawn
        if (PlayerController.Instance != null)
            PlayerController.Instance.Teleport(puntoReaparicion.position, PlayerController.Instance.transform.rotation);
    }
}
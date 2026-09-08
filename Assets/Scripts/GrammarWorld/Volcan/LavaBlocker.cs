using UnityEngine;


public class LavaBlocker : MonoBehaviour
{
    public Transform puntoReaparicion;
    private float lastTeleportTime = 0f;
    private bool isCooled = false;

    void OnTriggerEnter(Collider other)
    {
        if (isCooled) return;
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastTeleportTime < 2f) return;
        lastTeleportTime = Time.time;

        // respawn
        if (PlayerController.Instance != null)
            PlayerController.Instance.Teleport(puntoReaparicion.position, PlayerController.Instance.transform.rotation);
        Debug.Log(
            "Complete both challenges first!");
    }

    public void CoolDown()
    {
        isCooled = true;
    }
}
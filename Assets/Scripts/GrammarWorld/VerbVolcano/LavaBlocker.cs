using UnityEngine;
using SpatialSys.UnitySDK;

public class LavaBlocker : MonoBehaviour
{
    public Transform puntoReaparicion;
    private float lastTeleportTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        // Evitar teleports repetidos
        if (Time.time - lastTeleportTime < 2f) return;
        lastTeleportTime = Time.time;

        SpatialBridge.actorService.localActor.avatar.position =
            puntoReaparicion.position;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Complete the challenge first!");
    }
}
using UnityEngine;
using SpatialSys.UnitySDK;

public class LavaBlocker : MonoBehaviour
{
    public Transform puntoReaparicion;
    private float lastTeleportTime = 0f;
    private bool isCooled = false;

    void OnTriggerEnter(Collider other)
    {
        if (isCooled) return;
        if (Time.time - lastTeleportTime < 2f) return;
        lastTeleportTime = Time.time;

        SpatialBridge.actorService.localActor.avatar.position =
            puntoReaparicion.position;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Complete both challenges first!");
    }

    public void CoolDown()
    {
        isCooled = true;
    }
}
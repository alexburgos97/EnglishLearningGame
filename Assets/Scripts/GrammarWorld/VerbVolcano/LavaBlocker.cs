using UnityEngine;
using SpatialSys.UnitySDK;

public class LavaBlocker : MonoBehaviour
{
    public Transform puntoReaparicion;

    public void OnTriggerEnter(Collider other)
    {
        SpatialBridge.actorService.localActor.avatar.position = 
            puntoReaparicion.position;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Complete the challenge first!");
    }
}
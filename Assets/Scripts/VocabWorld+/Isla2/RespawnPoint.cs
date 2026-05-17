using UnityEngine;
using SpatialSys.UnitySDK;

public class RespawnPoint : MonoBehaviour
{
    public Transform puntoReaparicion;

    void OnTriggerEnter(Collider other)
    {
        SpatialBridge.actorService.localActor.avatar.position =
            puntoReaparicion.position;
    }
}
using UnityEngine;
using SpatialSys.UnitySDK;

public class RespawnTrigger : MonoBehaviour
{
    [Header("Arrastra aquí tu Punto de Reaparicion")]
    public Transform puntoInicio;

    public void Teletransportar()
    {
        if (puntoInicio != null)
        {
            // Nueva función correcta del SDK de Spatial para teletransportar
            SpatialBridge.actorService.localActor.avatar.SetPositionRotation(puntoInicio.position, puntoInicio.rotation);
            
            SpatialBridge.coreGUIService.DisplayToastMessage("Watch your step! Let's try again.");
        }
    }
}
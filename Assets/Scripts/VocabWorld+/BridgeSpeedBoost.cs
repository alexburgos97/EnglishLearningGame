using UnityEngine;
using SpatialSys.UnitySDK;

public class BridgeSpeedBoost : MonoBehaviour
{
    public float walkSpeed = 8f;
    public float runSpeed = 16f;

    private float defaultWalkSpeed = 4f;
    private float defaultRunSpeed = 8f;

    public void OnEnterBridge()
    {
        float currentWalk = SpatialBridge.actorService.localActor.avatar.walkSpeed;
        float currentRun = SpatialBridge.actorService.localActor.avatar.runSpeed;
    
        SpatialBridge.coreGUIService.DisplayToastMessage(
        "Default Walk: " + currentWalk + " Run: " + currentRun);


        SpatialBridge.actorService.localActor.avatar.walkSpeed = walkSpeed;
        SpatialBridge.actorService.localActor.avatar.runSpeed = runSpeed;
    }

    public void OnExitBridge()
    {
        SpatialBridge.actorService.localActor.avatar.walkSpeed = defaultWalkSpeed;
        SpatialBridge.actorService.localActor.avatar.runSpeed = defaultRunSpeed;
    }
}

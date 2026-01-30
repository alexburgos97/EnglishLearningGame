using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarMecahincs : MonoBehaviour
{
    private IAvatar avatar;

    private void Start()
    {
        avatar = SpatialBridge.actorService.localActor.avatar;
    }

    public void RespawnAvatar()
    {
        avatar.Respawn();
    }
}

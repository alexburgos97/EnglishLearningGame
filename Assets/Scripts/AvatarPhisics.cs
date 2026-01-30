using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPhisics : MonoBehaviour
{
    private IAvatar avatar;
    private float defaultJumpHeight;
    private float defaultRunSpeed;
    [SerializeField] private float runningTime = 5f;
    [SerializeField] private float jumpTime = 10f;

    private void Start()
    {
        avatar = SpatialBridge.actorService.localActor.avatar;
        defaultRunSpeed = avatar.runSpeed;
        defaultJumpHeight = avatar.jumpHeight;
    }

    public void AddRunSpeed(float speed)
    {
        avatar.runSpeed = speed;
        StartCoroutine(SetRunningSpeed());
    }

    public void AddJumpHeight(float height)
    {
        avatar.jumpHeight = height;
        StartCoroutine(SetJumpHeight());
    }

    IEnumerator SetRunningSpeed()
    {
        yield return new WaitForSeconds(runningTime);
        avatar.runSpeed = defaultRunSpeed;
    }

    IEnumerator SetJumpHeight()
    {
        yield return new WaitForSeconds(jumpTime);
        avatar.jumpHeight = defaultJumpHeight;
    }
}

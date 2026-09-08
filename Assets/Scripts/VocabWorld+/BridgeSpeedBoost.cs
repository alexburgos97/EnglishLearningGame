using UnityEngine;


public class BridgeSpeedBoost : MonoBehaviour
{
    public float walkSpeed = 8f;
    public float runSpeed = 16f;

    private float defaultWalkSpeed = 4f;
    private float defaultRunSpeed = 8f;

    public void OnEnterBridge()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.SetSpeed(walkSpeed, runSpeed);
    }

    public void OnExitBridge()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.SetSpeed(defaultWalkSpeed, defaultRunSpeed);
    }
}

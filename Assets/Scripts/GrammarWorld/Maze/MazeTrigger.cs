using UnityEngine;
using SpatialSys.UnitySDK;

public class MazeTrigger : MonoBehaviour
{
    public int stepIndex = 0;

    public void OnPlayerEnter()
    {
        MazeManager.Instance.OnTriggerReached(stepIndex);
    }
}
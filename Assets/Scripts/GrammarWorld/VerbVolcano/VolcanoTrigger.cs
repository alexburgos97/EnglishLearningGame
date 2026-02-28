using UnityEngine;
using SpatialSys.UnitySDK;

public class VolcanoTrigger : MonoBehaviour
{
    public bool isPresentSimple = true;
    private bool activated = false;

    public void Activar()
    {
        if (activated) return;
        activated = true;

        if (isPresentSimple)
            VolcanoQuizManager.Instance.TryStartPresentSimple();
        else
            VolcanoQuizManager.Instance.TryStartPastSimple();

        GetComponent<SpatialTriggerEvent>().enabled = false;
    }
}
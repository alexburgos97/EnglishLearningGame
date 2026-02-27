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
        VolcanoQuizManager.Instance.ShowStartPanel(isPresentSimple);
        GetComponent<SpatialTriggerEvent>().enabled = false;
    }
}

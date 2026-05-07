using UnityEngine;
using SpatialSys.UnitySDK;

public class VolcanoTrigger : MonoBehaviour
{
    public bool isPresentSimple = true;
    private bool activated = false;

    public void Activar()
    {
        if (isPresentSimple)
        {
            if (activated) return;
            activated = true;
            VolcanoQuizManager.Instance.TryStartPresentSimple();
            GetComponent<SpatialTriggerEvent>().enabled = false;
        }
        else
        {
            // Pasado Simple solo se desactiva cuando PS está completado
            bool psCompleted = VolcanoQuizManager.Instance.PresentSimpleCompleted;
            if (!psCompleted)
            {
                VolcanoQuizManager.Instance.TryStartPastSimple();
                return; // No desactiva el trigger
            }
            if (activated) return;
            activated = true;
            VolcanoQuizManager.Instance.TryStartPastSimple();
            GetComponent<SpatialTriggerEvent>().enabled = false;
        }
    }
}
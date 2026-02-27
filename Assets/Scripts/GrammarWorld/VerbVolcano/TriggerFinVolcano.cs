using UnityEngine;

public class TriggerFinVolcano : MonoBehaviour
{
    private bool activated = false;

    public void Activar()
    {
        if (activated) return;
        activated = true;
        GameProgressManager.Instance.AwardVerbMasterMedal();
    }
}

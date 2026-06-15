using UnityEngine;

public class TriggerFinVolcano : MonoBehaviour
{
    [Header("Medalla")]
    public GameObject medallaSprite;

    private bool activated = false;

    public void Activar()
    {
        if (activated) return;
        activated = true;
        GameProgressManager.Instance.AwardVerbMasterMedal();

        // Desactivar la medalla de la escena
        Invoke(nameof(DesactivarMedalla), 2f);
    }

    private void DesactivarMedalla()
    {
        if (medallaSprite != null)
            medallaSprite.SetActive(false);
    }
}
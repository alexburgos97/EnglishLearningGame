using UnityEngine;
using SpatialSys.UnitySDK; // Asegurando compatibilidad con Spatial

public class VolcanoQuizManager : MonoBehaviour
{
    [Header("Estado del Volcán")]
    public bool presentSimpleCompleted = false;
    public bool pastSimpleCompleted = false;

    [Header("Conexiones")]
    [Tooltip("Arrastra aquí el GameObject de la Lava que bloquea el paso")]
    public GameObject lavaBlockerCollider; 
    [Tooltip("Arrastra aquí el objeto que tiene el script VolcanoUIManager")]
    public VolcanoUIManager uiManager;

    // Este método lo llamarán las plataformas/rocas al responder correctamente
    public void PlatformCompleted(bool isPresentSimplePlatform)
    {
        if (isPresentSimplePlatform)
        {
            presentSimpleCompleted = true;
            if (uiManager != null) 
                uiManager.ShowVolcanoMessage("Present Simple Clear! Now go to the Past Simple platform.");
                
            Debug.Log("Volcano: Present Simple Completado.");
        }
        else
        {
            // Verificación de seguridad: ¿Intentó hacer el pasado sin el presente?
            if (!presentSimpleCompleted)
            {
                if (uiManager != null) 
                    uiManager.ShowVolcanoMessage("You must complete the Present Simple platform first!");
                return;
            }

            pastSimpleCompleted = true;
            if (uiManager != null) 
                uiManager.ShowVolcanoMessage("Past Simple Clear! The lava is gone!");
                
            Debug.Log("Volcano: Past Simple Completado.");
            UnlockVolcanoCrossing();
        }
    }

    private void UnlockVolcanoCrossing()
    {
        // Solo si ambos están completos, apagamos el bloqueador de lava
        if (presentSimpleCompleted && pastSimpleCompleted)
        {
            if (lavaBlockerCollider != null)
            {
                lavaBlockerCollider.SetActive(false);
            }
        }
    }
}
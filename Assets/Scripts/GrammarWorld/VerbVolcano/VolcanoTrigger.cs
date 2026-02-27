using UnityEngine;
using SpatialSys.UnitySDK;

public class VolcanoTrigger : MonoBehaviour
{
    [Tooltip("Arrastra aquí el objeto que tiene el VolcanoUIManager de ESTA plataforma")]
    public VolcanoUIManager platformUI;

    public void Activar()
    {
        // 1. Esto nos dirá si el Spatial Trigger Event sí funciona
        SpatialBridge.coreGUIService.DisplayToastMessage("¡PISASTE EL TRIGGER!");

        if (platformUI != null)
        {
            if (platformUI.centralManager == null)
            {
                // 2. Esto nos dirá si falta el Cerebro
                SpatialBridge.coreGUIService.DisplayToastMessage("ERROR: Falta el Central Manager en la UI");
            }
            else
            {
                platformUI.ActivatePlatform();
            }
        }
        else
        {
            // 3. Esto nos dirá si falta la UI
            SpatialBridge.coreGUIService.DisplayToastMessage("ERROR: PlatformUI está vacío en el Trigger");
        }
    }
}
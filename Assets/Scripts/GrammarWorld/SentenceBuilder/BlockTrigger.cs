using UnityEngine;
using SpatialSys.UnitySDK; // Necesario para el Toast

public class BlockTrigger : MonoBehaviour
{
    public int blockIndex;

    public void Activar()
    {
        // 1. ESTE MENSAJE ES CLAVE: Te confirmará si el trigger físico funciona
        //SpatialBridge.coreGUIService.DisplayToastMessage("Pisaste el trigger del bloque: " + blockIndex);
        
        // 2. Ejecuta la lógica
        BridgeQuizManager.Instance.OnAvatarReachedEdge(blockIndex);
        QuizUIManager.Instance.MoverCanvasAPosicion(this.transform);
    }
}
using UnityEngine;

public class BlockTrigger : MonoBehaviour
{
    public int blockIndex;

    public void Activar()
    {
        // 1. Mueve la ventana del Canvas EXACTAMENTE a la posición de este trigger
        QuizUIManager.Instance.MoverCanvasAPosicion(this.transform);
        
        // 2. Le avisa al manager que muestre la pregunta
        BridgeQuizManager.Instance.OnAvatarReachedEdge(blockIndex);
    }
}
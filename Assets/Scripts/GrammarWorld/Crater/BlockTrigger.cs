using UnityEngine;

public class BlockTrigger : MonoBehaviour
{
    public int blockIndex;

    [Tooltip("Opcional. Si este trigger no está orientado hacia el jugador (p. ej. TriggerInicioPuente, que también dispara el tutorial), asigná acá otro transform del que tomar solo la rotación de la ventana de pregunta.")]
    public Transform canvasRotationOverride;

    public void Activar()
    {
        // 1. Mueve la ventana del Canvas a la posición de este trigger (y a la rotación de canvasRotationOverride si se asignó)
        QuizUIManager.Instance.MoverCanvasAPosicion(this.transform, canvasRotationOverride);

        // 2. Le avisa al manager que muestre la pregunta
        BridgeQuizManager.Instance.OnAvatarReachedEdge(blockIndex);
    }
}
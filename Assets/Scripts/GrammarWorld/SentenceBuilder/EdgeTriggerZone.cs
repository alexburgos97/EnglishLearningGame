using UnityEngine;

public class EdgeTriggerZone : MonoBehaviour
{
    public int blockIndex;

    void OnTriggerEnter(Collider other)
    {
        if (BridgeQuizManager.Instance == null) return;
        if (BridgeQuizManager.Instance.quizActive) return;

        // Activar sin verificar tag por ahora
        BridgeQuizManager.Instance.OnAvatarReachedEdge(blockIndex);
    }
}

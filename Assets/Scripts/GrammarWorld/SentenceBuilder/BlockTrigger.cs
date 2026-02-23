using UnityEngine;

public class BlockTrigger : MonoBehaviour
{
    public int blockIndex;

    public void Activar()
    {
        BridgeQuizManager.Instance.OnAvatarReachedEdge(blockIndex);
    }
}
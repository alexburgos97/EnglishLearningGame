using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Reemplazo directo del componente SpatialTriggerEvent del SDK de Spatial.
/// Se coloca sobre un Box Collider normal marcado como "Is Trigger" y dispara
/// UnityEvents al entrar/salir el jugador, igual que hacían los antiguos triggers de Spatial.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlayerTriggerZone : MonoBehaviour
{
    public string playerTag = "Player";

    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            onPlayerEnter.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            onPlayerExit.Invoke();
    }
}

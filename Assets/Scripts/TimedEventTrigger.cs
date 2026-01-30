using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TimedEventTrigger : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("El tiempo en segundos a esperar antes de ejecutar el evento.")]
    [SerializeField] private float delayTime = 2.0f;

    [Tooltip("El evento que se ejecutará al finalizar el tiempo.")]
    [SerializeField] private UnityEvent onTimeReached;

    /// <summary>
    /// Llama a esta función para iniciar el temporizador.
    /// Al finalizar el tiempo 'delayTime', se disparará el UnityEvent.
    /// </summary>
    public void ExecuteWithDelay()
    {
        // Iniciamos la corrutina para esperar el tiempo definido
        StartCoroutine(WaitAndInvoke());
    }

    private IEnumerator WaitAndInvoke()
    {
        // Espera la cantidad de segundos definida en el editor
        yield return new WaitForSeconds(delayTime);

        // Ejecuta el evento si no es nulo
        if (onTimeReached != null)
        {
            onTimeReached.Invoke();
        }
    }

    // Opcional: Si necesitas detener el contador antes de que termine
    public void CancelTimer()
    {
        StopAllCoroutines();
    }
}
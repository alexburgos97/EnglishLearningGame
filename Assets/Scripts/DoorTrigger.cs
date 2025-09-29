using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public string targetSceneName; // Aquí definiremos a qué "escena" local vamos

    private LocalSceneSwitcher sceneSwitcher; // Referencia al "cerebro"

    void Start()
    {
        // Buscamos el "cerebro" automáticamente al inicio
        sceneSwitcher = FindObjectOfType<LocalSceneSwitcher>();
        if (sceneSwitcher == null)
        {
            Debug.LogError("DoorTrigger: No se encontró un LocalSceneSwitcher en la escena. ¡Asegúrate de que el SceneSwitchManager existe y tiene el script!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto que entró en el trigger tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            if (sceneSwitcher != null)
            {
                sceneSwitcher.SwitchToScene(targetSceneName); // Le decimos al cerebro que cambie
            }
        }
    }
}
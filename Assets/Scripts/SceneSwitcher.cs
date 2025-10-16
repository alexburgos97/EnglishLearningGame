using UnityEngine;
using SpatialSys.UnitySDK;

public class SceneSwitcher : MonoBehaviour
{
    [Header("Contenedores de Contenido de Escena")]
    public GameObject lobbyContent;
    public GameObject grammarWorldContent; // Para tu _EspacioGrammarWorld

    [Header("Materiales de Skybox para Cada Escena")]
    public Material lobbySkybox;
    public Material grammarWorldSkybox; // Para el skybox de GrammarWorld

    [Header("Puntos de Entrada (Entrance Points)")]
    public Transform lobbyEntrancePoint;
    public Transform grammarWorldEntrancePoint; // Para tu GrammarWorldSpawnPoint

    void Start()
    {
        // Al iniciar el juego, siempre comenzamos en el lobby
        SwitchToScene("lobby");
    }

    public void SwitchToScene(string sceneName)
    {
        // 1. Desactivar todo el contenido primero
        lobbyContent.SetActive(false);
        grammarWorldContent.SetActive(false);

        Transform targetEntrance = null; // Para almacenar el punto de entrada de destino

        // 2. Activar el contenido y configurar el skybox según el nombre de la escena
        switch (sceneName.ToLower())
        {
            case "lobby":
                lobbyContent.SetActive(true);
                RenderSettings.skybox = lobbySkybox;
                targetEntrance = lobbyEntrancePoint;
                break;
            case "grammarworld": // ¡Este es el nombre clave para tu GrammarWorld!
                grammarWorldContent.SetActive(true);
                RenderSettings.skybox = grammarWorldSkybox;
                targetEntrance = grammarWorldEntrancePoint;
                break;
            default:
                Debug.LogWarning("SceneSwitcher: Nombre de escena no reconocido: " + sceneName);
                return;
        }

        // 3. Teletransportar al jugador al punto de entrada correcto
        if (targetEntrance != null)
        {
            TeleportPlayer(targetEntrance);
        }
    }

   private void TeleportPlayer(Transform targetTransform)
{
    // =======================================================================
    // PLAN B: Usar el método universal de Unity para encontrar al jugador.
    // Spatial siempre etiqueta al avatar del jugador local con el tag "Player".
    // Este método es el más robusto contra cambios en la API de Spatial.
    // =======================================================================
    GameObject localPlayerObject = GameObject.FindWithTag("Player");

    if (localPlayerObject != null)
    {
        // Una vez que tenemos el GameObject, podemos acceder a su transform y moverlo.
        localPlayerObject.transform.position = targetTransform.position;
        localPlayerObject.transform.rotation = targetTransform.rotation;
    }
    else
    {
        Debug.LogError("¡ERROR CRÍTICO! No se pudo encontrar ningún GameObject con el tag 'Player'. " +
                     "Asegúrate de que el Spatial Runtime está activo y el avatar ha sido instanciado.");
    }
}
}

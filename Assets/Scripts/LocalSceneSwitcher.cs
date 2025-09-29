using UnityEngine;
using SpatialSys.UnitySDK;
using SpatialSys.UnitySDK.Internal;

public class LocalSceneSwitcher : MonoBehaviour
{
    [Header("Contenido de las Escenas")]
    public GameObject _EspacioLobby;
    public GameObject _EspacioBiblioteca; 
    public GameObject _EspacioGrammarWorld;
    public GameObject _EspacioVocabWorld;

    [Header("Skyboxes para Cada Escena")]
    public Material lobbySkybox;
    public Material bibliotecaSkybox; 
    public Material grammarWorldSkybox;
    public Material vocabWorldSkybox;

    [Header("Entrance Points (Arrastra los GameObjects aquí)")]
    public Transform lobbyEntrancePoint;
    public Transform bibliotecaEntrancePoint;
    public Transform grammarWorldEntrancePoint;
    public Transform vocabWorldEntrancePoint;

    void Start()
    {
        SwitchToScene("lobby");
    }

    public void SwitchToScene(string sceneName)
    {
        // Desactivamos todo para empezar de cero
        _EspacioLobbyContent.SetActive(false);
        _EspacioBibliotecaContent.SetActive(false);
        _EspacioGrammarWorldContent.SetActive(false);
        _EspacioVocabWorldContent.SetActive(false);

        Transform targetEntrancePoint = null;

        switch (sceneName.ToLower())
        {
            case "lobby":
                lobbyContent.SetActive(true);
                RenderSettings.skybox = lobbySkybox;
                targetEntrancePoint = lobbyEntrancePoint;
                break;

            case "biblioteca": // Aquí el nombre interno que usaremos para activar
                bibliotecaContent.SetActive(true);
                RenderSettings.skybox = bibliotecaSkybox;
                targetEntrancePoint = bibliotecaEntrancePoint;
                break;

            case "grammarWorld":
                grammarWorldContent.SetActive(true);
                RenderSettings.skybox = grammarWorldSkybox;
                targetEntrancePoint = grammarWorldEntrancePoint;
                break;

            case "vocabWorld":
                vocabWorlddContent.SetActive(true);
                RenderSettings.skybox = vocabWorldSkybox;
                targetEntrancePoint = vocabWorldEntrancePoint;
                break;
        }

        if (targetEntrancePoint != null)
        {
            TeleportPlayerToTransform(targetEntrancePoint);
        }
    }

    void TeleportPlayerToTransform(Transform target)
{
    // Utiliza la forma recomendada para acceder al jugador local y teletransportarlo.
    // SpatialPlayer.Local es la forma más directa en las versiones actuales del toolkit.
    if (SpatialPlayer.Local != null) 
    {
        SpatialPlayer.Local.SetPositionAndRotation(target.position, target.rotation);
    }
    else
    {
        Debug.LogWarning("SpatialPlayer.Local no disponible. No se pudo teletransportar. ¿Estás en el modo de ejecución de Spatial?");
        // Opcional: Para pruebas en el editor sin Spatial Runtime, puedes agregar esto:
        // GameObject playerInEditor = GameObject.FindWithTag("Player"); 
        // if (playerInEditor != null)
        // {
        //     playerInEditor.transform.position = target.position;
        //     playerInEditor.transform.rotation = target.rotation;
        // }
    }
}
}

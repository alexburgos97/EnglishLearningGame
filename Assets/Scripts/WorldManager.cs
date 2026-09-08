using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private GameObject espacioLobby;
    [SerializeField] private GameObject espacioGrammarWorld;
    [SerializeField] private GameObject espacioVocabWorld;

    [Header("Puntos de aparición del jugador")]
    [Tooltip("El jugador vive fuera de los tres espacios (para no desactivarse junto con ellos), así que hay que teletransportarlo manualmente al cambiar de mundo.")]
    [SerializeField] private Transform spawnLobby;
    [SerializeField] private Transform spawnGrammarWorld;
    [SerializeField] private Transform spawnVocabWorld;

    private void Start()
    {
        // Al iniciar, solo el Lobby es visible
        espacioLobby.SetActive(true);
        espacioGrammarWorld.SetActive(false);
        espacioVocabWorld.SetActive(false);
    }

    public void EnterGrammarWorld()
    {
        espacioLobby.SetActive(false);
        espacioGrammarWorld.SetActive(true);
        espacioVocabWorld.SetActive(false);
        MovePlayerTo(spawnGrammarWorld);
    }

    public void EnterVocabWorld()
    {
        espacioLobby.SetActive(false);
        espacioGrammarWorld.SetActive(false);
        espacioVocabWorld.SetActive(true);
        MovePlayerTo(spawnVocabWorld);
    }

    public void ReturnToLobby()
    {
        espacioLobby.SetActive(true);
        espacioGrammarWorld.SetActive(false);
        espacioVocabWorld.SetActive(false);
        MovePlayerTo(spawnLobby);
    }

    private void MovePlayerTo(Transform spawnPoint)
    {
        if (spawnPoint == null || PlayerController.Instance == null) return;
        PlayerController.Instance.Teleport(spawnPoint.position, spawnPoint.rotation);
    }
}
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private GameObject espacioLobby;
    [SerializeField] private GameObject espacioGrammarWorld;
    [SerializeField] private GameObject espacioVocabWorld;

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
    }

    public void EnterVocabWorld()
    {
        espacioLobby.SetActive(false);
        espacioGrammarWorld.SetActive(false);
        espacioVocabWorld.SetActive(true);
    }

    public void ReturnToLobby()
    {
        espacioLobby.SetActive(true);
        espacioGrammarWorld.SetActive(false);
        espacioVocabWorld.SetActive(false);
    }
}
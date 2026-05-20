using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class AnimalObject : MonoBehaviour
{
    [Header("Configuración")]
    public string wordInEnglish;
    public AudioClip wordAudio;

    [Header("Animal oculto en escena")]
    public GameObject animalSprite;

    [Header("UI")]
    public TextMeshProUGUI animalNameText;
    public TextMeshProUGUI feedbackText;
    public Button closeButton;

    private AudioSource audioSource;
    private bool completed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        // Ocultar el animal al inicio
        if (animalSprite != null)
            animalSprite.SetActive(false);

        closeButton.onClick.AddListener(ClosePanel);
        Isla3Manager.Instance.RegisterAnimal();
    }

    public void OnPlayerInteract()
    {
        if (completed) return;

        // Mostrar el animal oculto
        if (animalSprite != null)
            animalSprite.SetActive(true);

        animalNameText.text = wordInEnglish.ToUpper();
        feedbackText.text = "You found: " + wordInEnglish.ToUpper() + "!";
        feedbackText.color = Color.green;

        if (wordAudio != null)
        {
            audioSource.clip = wordAudio;
            audioSource.Play();
        }

        completed = true;
        Isla3Manager.Instance.OnAnimalComplete();
        Isla3Manager.Instance.ShowAnimalPanel();
    }

    private void ClosePanel()
    {
        Isla3Manager.Instance.HideAllPanels();
    }
}

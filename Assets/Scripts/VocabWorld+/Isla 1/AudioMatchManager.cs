using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class AudioMatchManager : MonoBehaviour
{
    public static AudioMatchManager Instance { get; private set; }

    [Header("Referencias")]
    public AudioSource audioSource;

    [Header("Panel de preguntas")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public TextMeshProUGUI option3Text;
    public TextMeshProUGUI feedbackText;
    public Button replayButton;

    // Banco de preguntas Zona Ropa
    private string[] words = { "t-shirt", "shoes", "hat" };
    private AudioClip[] wordAudios;
    private string[,] options = new string[,]
    {
        { "t-shirt", "shoes", "hat" },
        { "shoes", "hat", "t-shirt" },
        { "hat", "t-shirt", "shoes" }
    };

    [Header("Audios de palabras")]
    public AudioClip audioTshirt;
    public AudioClip audioShoes;
    public AudioClip audioHat;

    private int currentIndex = 0;
    private bool questionActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        questionPanel.SetActive(false);
        feedbackText.text = "";

        wordAudios = new AudioClip[] { audioTshirt, audioShoes, audioHat };

        option1Button.onClick.AddListener(() => CheckAnswer(option1Text.text));
        option2Button.onClick.AddListener(() => CheckAnswer(option2Text.text));
        option3Button.onClick.AddListener(() => CheckAnswer(option3Text.text));
        replayButton.onClick.AddListener(ReplayAudio);
    }

    public void ShowQuestion(int index)
    {
        if (index >= words.Length) return;
        currentIndex = index;
        questionActive = true;
        feedbackText.text = "";

        // Reproducir audio
        audioSource.clip = wordAudios[index];
        audioSource.Play();

        // Mostrar opciones
        option1Text.text = options[index, 0];
        option2Text.text = options[index, 1];
        option3Text.text = options[index, 2];

        option1Button.interactable = true;
        option2Button.interactable = true;
        option3Button.interactable = true;

        questionText.text = "What did you hear?";
        questionPanel.SetActive(true);
    }

    private void CheckAnswer(string selected)
    {
        if (!questionActive) return;

        option1Button.interactable = false;
        option2Button.interactable = false;
        option3Button.interactable = false;

        if (selected == words[currentIndex])
        {
            feedbackText.text = "Correct! " + words[currentIndex].ToUpper();
            feedbackText.color = Color.green;
            questionActive = false;
            VocabCardManager.Instance.AddClothesWord(words[currentIndex]);
            Invoke(nameof(NextQuestion), 2f);
        }
        else
        {
            feedbackText.text = "Try again!";
            feedbackText.color = Color.red;
            Invoke(nameof(ReactivateButtons), 1.5f);
        }
    }

    private void ReactivateButtons()
    {
        option1Button.interactable = true;
        option2Button.interactable = true;
        option3Button.interactable = true;
        feedbackText.text = "";
        ReplayAudio();
    }

    private void NextQuestion()
    {
        questionPanel.SetActive(false);
        feedbackText.text = "";
        currentIndex++;

        if (currentIndex < words.Length)
            Isla1Manager.Instance.OnClothesWordComplete(currentIndex);
        else
            Isla1Manager.Instance.OnZonaRopaComplete();
    }

    private void ReplayAudio()
    {
        if (audioSource.clip != null)
            audioSource.Play();
    }
}
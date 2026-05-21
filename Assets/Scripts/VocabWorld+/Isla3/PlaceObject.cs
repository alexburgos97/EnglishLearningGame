using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class PlaceObject : MonoBehaviour
{
    [Header("Configuración")]
    public string correctAnswer;
    public AudioClip wordAudio;
    public string[] options = new string[3];

    [Header("UI")]
    public TextMeshProUGUI questionText;
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public TextMeshProUGUI option3Text;
    public TextMeshProUGUI feedbackText;

    private AudioSource audioSource;
    private bool completed = false;
    private bool answerInProgress = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        ShuffleOptions();

        option1Button.onClick.AddListener(() => CheckAnswer(option1Text.text));
        option2Button.onClick.AddListener(() => CheckAnswer(option2Text.text));
        option3Button.onClick.AddListener(() => CheckAnswer(option3Text.text));

        Isla3Manager.Instance.RegisterPlace();
    }

    private void ShuffleOptions()
    {
        for (int i = options.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = options[i];
            options[i] = options[j];
            options[j] = temp;
        }

        if (options.Length >= 3)
        {
            option1Text.text = options[0];
            option2Text.text = options[1];
            option3Text.text = options[2];
        }
    }

    public void OnPlayerInteract()
    {
    if (completed) return;

    if (wordAudio != null)
    {
        audioSource.clip = wordAudio;
        audioSource.Play();
    }

    questionText.text = "What is this place?";
    feedbackText.text = "";
    answerInProgress = false;

    // Reasignar opciones y listeners cada vez
    ShuffleOptions();

    option1Button.onClick.RemoveAllListeners();
    option2Button.onClick.RemoveAllListeners();
    option3Button.onClick.RemoveAllListeners();

    option1Button.onClick.AddListener(() => CheckAnswer(option1Text.text));
    option2Button.onClick.AddListener(() => CheckAnswer(option2Text.text));
    option3Button.onClick.AddListener(() => CheckAnswer(option3Text.text));

    option1Button.interactable = true;
    option2Button.interactable = true;
    option3Button.interactable = true;

    Isla3Manager.Instance.ShowPlacePanel();
    }

    private void CheckAnswer(string selected)
    {
        if (answerInProgress) return;
        answerInProgress = true;

        option1Button.interactable = false;
        option2Button.interactable = false;
        option3Button.interactable = false;

        if (selected.ToUpper() == correctAnswer.ToUpper())
        {
            feedbackText.text = "Correct! It's a " + correctAnswer.ToUpper() + "!";
            feedbackText.color = Color.green;
            completed = true;
            Isla3Manager.Instance.OnPlaceComplete();
            Invoke(nameof(ClosePanel), 2f);
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
        answerInProgress = false;
    }

    private void ClosePanel()
    {
        Isla3Manager.Instance.HideAllPanels();
    }
}

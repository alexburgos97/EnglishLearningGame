using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class FamilyObject : MonoBehaviour
{
    [Header("Configuración")]
    public string wordInEnglish;
    public AudioClip wordAudio;

    [Header("UI")]
    public TextMeshProUGUI feedbackText;
    public TMP_InputField answerInput;
    public Button submitButton;
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

        submitButton.onClick.AddListener(CheckAnswer);
        closeButton.onClick.AddListener(ClosePanel);

        Isla3Manager.Instance.RegisterFamily();
    }

    public void OnPlayerInteract()
    {
    if (completed) return;

    if (wordAudio != null)
        {
        audioSource.clip = wordAudio;
        audioSource.Play();
    }

    feedbackText.text = "";
    answerInput.text = "";

    // Reasignar el listener al objeto correcto
    submitButton.onClick.RemoveAllListeners();
    submitButton.onClick.AddListener(CheckAnswer);

    closeButton.onClick.RemoveAllListeners();
    closeButton.onClick.AddListener(ClosePanel);

    Isla3Manager.Instance.ShowFamilyPanel();
    }

    private void CheckAnswer()
    {
        string answer = answerInput.text.Trim().ToUpper();
        string correct = wordInEnglish.ToUpper();

        if (answer == correct)
        {
            feedbackText.text = "Well done! " + wordInEnglish.ToUpper();
            feedbackText.color = Color.green;
            completed = true;
            Isla3Manager.Instance.OnFamilyComplete();
            Invoke(nameof(ClosePanel), 2f);
        }
        else
        {
            feedbackText.text = "Try again!";
            feedbackText.color = Color.red;
            answerInput.text = "";
        }
    }

    private void ClosePanel()
    {
        Isla3Manager.Instance.HideAllPanels();
    }
}

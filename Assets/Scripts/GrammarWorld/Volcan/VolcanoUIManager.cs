using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class VolcanoUIManager : MonoBehaviour
{
    public static VolcanoUIManager Instance { get; private set; }

    [Header("Paneles de inicio")]
    public GameObject startPanelPS;
    public Button startButtonPS;
    public GameObject startPanelPA;
    public Button startButtonPA;

    [Header("Paneles de preguntas")]
    public GameObject questionPanelPS;
    public GameObject questionPanelPA;

    [Header("Presente Simple - Campos")]
    public TextMeshProUGUI sentenceTextPS;
    public TextMeshProUGUI feedbackTextPS;
    public Button buttonAPS;
    public Button buttonBPS;
    public TextMeshProUGUI buttonATextPS;
    public TextMeshProUGUI buttonBTextPS;

    [Header("Pasado Simple - Campos")]
    public TextMeshProUGUI sentenceTextPA;
    public TextMeshProUGUI feedbackTextPA;
    public Button buttonAPA;
    public Button buttonBPA;
    public TextMeshProUGUI buttonATextPA;
    public TextMeshProUGUI buttonBTextPA;

    [Header("Audios de feedback")]
    public AudioClip audioCorrect;
    public AudioClip audioWrong;
    private AudioSource audioSource;

    private int currentIndex = 0;
    private bool isPS = true;
    private string currentVerbA = "";
    private string currentVerbB = "";
    private bool answerInProgress = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (startPanelPS != null) startPanelPS.SetActive(false);
        if (startPanelPA != null) startPanelPA.SetActive(false);
        if (questionPanelPS != null) questionPanelPS.SetActive(false);
        if (questionPanelPA != null) questionPanelPA.SetActive(false);

        startButtonPS.onClick.AddListener(() => OnStartClicked(true));
        startButtonPA.onClick.AddListener(() => OnStartClicked(false));

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    public void ShowStartPanel(bool isPresentSimple)
    {
        isPS = isPresentSimple;
        if (isPresentSimple)
            startPanelPS.SetActive(true);
        else
            startPanelPA.SetActive(true);
    }

    private void OnStartClicked(bool isPresentSimple)
    {
        startPanelPS.SetActive(false);
        startPanelPA.SetActive(false);
        VolcanoQuizManager.Instance.StartGame(isPresentSimple);
    }

    public void ShowQuestion(int index)
    {
        currentIndex = index;
        answerInProgress = false;

        string verbA = VolcanoQuizManager.Instance.currentVerbsA[index];
        string verbB = VolcanoQuizManager.Instance.currentVerbsB[index];

        if (Random.value < 0.5f)
        {
            currentVerbA = verbA;
            currentVerbB = verbB;
        }
        else
        {
            currentVerbA = verbB;
            currentVerbB = verbA;
        }

        if (isPS)
        {
            feedbackTextPS.text = "";
            sentenceTextPS.text = VolcanoQuizManager.Instance.currentSentences[index];
            buttonATextPS.text  = currentVerbA;
            buttonBTextPS.text  = currentVerbB;

            buttonAPS.onClick.RemoveAllListeners();
            buttonBPS.onClick.RemoveAllListeners();
            buttonAPS.onClick.AddListener(() => CheckAnswer(currentVerbA));
            buttonBPS.onClick.AddListener(() => CheckAnswer(currentVerbB));

            buttonAPS.interactable = true;
            buttonBPS.interactable = true;
            questionPanelPS.SetActive(true);
        }
        else
        {
            feedbackTextPA.text = "";
            sentenceTextPA.text = VolcanoQuizManager.Instance.currentSentences[index];
            buttonATextPA.text  = currentVerbA;
            buttonBTextPA.text  = currentVerbB;

            buttonAPA.onClick.RemoveAllListeners();
            buttonBPA.onClick.RemoveAllListeners();
            buttonAPA.onClick.AddListener(() => CheckAnswer(currentVerbA));
            buttonBPA.onClick.AddListener(() => CheckAnswer(currentVerbB));

            buttonAPA.interactable = true;
            buttonBPA.interactable = true;
            questionPanelPA.SetActive(true);
        }
    }

    private void CheckAnswer(string selected)
    {
        if (answerInProgress) return;
        answerInProgress = true;

        string correct = VolcanoQuizManager.Instance.currentAnswers[currentIndex];

        if (isPS)
        {
            buttonAPS.interactable = false;
            buttonBPS.interactable = false;

            if (selected == correct)
            {
                feedbackTextPS.text = "CORRECT! WELL DONE!";
                feedbackTextPS.color = Color.green;
                PlayFeedbackAudio(true);
                Invoke(nameof(NextQuestion), 2f);
            }
            else
            {
                feedbackTextPS.text = "DON'T WORRY! TRY AGAIN!";
                feedbackTextPS.color = Color.red;
                PlayFeedbackAudio(false);
                Invoke(nameof(ShowTryAgain), 2f);
            }
        }
        else
        {
            buttonAPA.interactable = false;
            buttonBPA.interactable = false;

            if (selected == correct)
            {
                feedbackTextPA.text = "CORRECT! WELL DONE!";
                feedbackTextPA.color = Color.green;
                PlayFeedbackAudio(true);
                Invoke(nameof(NextQuestion), 2f);
            }
            else
            {
                feedbackTextPA.text = "DON'T WORRY! TRY AGAIN!";
                feedbackTextPA.color = Color.red;
                PlayFeedbackAudio(false);
                Invoke(nameof(ShowTryAgain), 2f);
            }
        }
    }

    public void PlayFeedbackAudio(bool isCorrect)
    {
        AudioClip clip = isCorrect ? audioCorrect : audioWrong;
        if (clip == null) return;
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void NextQuestion()
    {
        questionPanelPS.SetActive(false);
        questionPanelPA.SetActive(false);
        VolcanoQuizManager.Instance.OnAnswerCorrect();
    }

    private void ShowTryAgain()
    {
        if (isPS)
        {
            buttonAPS.interactable = true;
            buttonBPS.interactable = true;
            feedbackTextPS.text = "";
        }
        else
        {
            buttonAPA.interactable = true;
            buttonBPA.interactable = true;
            feedbackTextPA.text = "";
        }
        VolcanoQuizManager.Instance.OnAnswerWrong();
    }

    public void ClosePanel()
    {
        questionPanelPS.SetActive(false);
        questionPanelPA.SetActive(false);
    }

    public void HidePSPanels()
    {
        startPanelPS.SetActive(false);
        questionPanelPS.SetActive(false);
    }

    public void HidePAPanels()
    {
        startPanelPA.SetActive(false);
        questionPanelPA.SetActive(false);
    }
}
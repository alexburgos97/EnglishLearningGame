using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class VolcanoUIManager : MonoBehaviour
{
    public static VolcanoUIManager Instance { get; private set; }

    [Header("Panel principal")]
    public GameObject volcanoPanel;
    public TextMeshProUGUI sentenceText;
    public TextMeshProUGUI feedbackText;
    public Button buttonA;
    public Button buttonB;
    public TextMeshProUGUI buttonAText;
    public TextMeshProUGUI buttonBText;

    [Header("Panel de inicio")]
    public GameObject startPanel;
    public Button startButton;

    private int currentIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        volcanoPanel.SetActive(false);
        startPanel.SetActive(false);
        startButton.onClick.AddListener(OnStartClicked);
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
    }

    private void OnStartClicked()
    {
        startPanel.SetActive(false);
        VolcanoQuizManager.Instance.StartGame();
    }

    public void ShowQuestion(int index)
    {
        currentIndex = index;
        feedbackText.text = "";

        sentenceText.text = VolcanoQuizManager.Instance.currentSentences[index];
        buttonAText.text  = VolcanoQuizManager.Instance.currentVerbsA[index];
        buttonBText.text  = VolcanoQuizManager.Instance.currentVerbsB[index];

        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();
        buttonA.onClick.AddListener(() => CheckAnswer(
            VolcanoQuizManager.Instance.currentVerbsA[currentIndex]));
        buttonB.onClick.AddListener(() => CheckAnswer(
            VolcanoQuizManager.Instance.currentVerbsB[currentIndex]));

        buttonA.interactable = true;
        buttonB.interactable = true;
        volcanoPanel.SetActive(true);
    }

    private void CheckAnswer(string selected)
    {
        string correct = VolcanoQuizManager.Instance.currentAnswers[currentIndex];
        string fullSentence = VolcanoQuizManager.Instance.currentFullSentences[currentIndex];

        buttonA.interactable = false;
        buttonB.interactable = false;

        if (selected == correct)
        {
            feedbackText.text = "Correct! " + fullSentence;
            feedbackText.color = Color.green;
            Invoke(nameof(NextQuestion), 2f);
        }
        else
        {
            feedbackText.text = "Don't worry! The correct answer is: " + fullSentence;
            feedbackText.color = Color.red;
            Invoke(nameof(ShowTryAgain), 2f);
        }
    }

    private void NextQuestion()
    {
        volcanoPanel.SetActive(false);
        VolcanoQuizManager.Instance.OnAnswerCorrect();
    }

    private void ShowTryAgain()
    {
        buttonA.interactable = true;
        buttonB.interactable = true;
        feedbackText.text = "";
        VolcanoQuizManager.Instance.OnAnswerWrong();
    }

    public void ClosePanel()
    {
        volcanoPanel.SetActive(false);
    }
}
using UnityEngine;
using SpatialSys.UnitySDK;
using UnityEngine.UI;
using TMPro;

public class QuizUIManager : MonoBehaviour
{
    public static QuizUIManager Instance { get; private set; }

    public GameObject quizPanel;
    public TextMeshProUGUI sentenceText;
    public TextMeshProUGUI optionAText;
    public TextMeshProUGUI optionBText;
    public TextMeshProUGUI feedbackText;
    public Button buttonA;
    public Button buttonB;

    [Header("El Canvas completo para moverlo")]
    public Transform quizCanvas;

    private int currentIndex = 0;
    private bool answerInProgress = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void MoverCanvasAPosicion(Transform posicionTrigger)
    {
        if (quizCanvas != null)
        {
            quizCanvas.position = posicionTrigger.position + new Vector3(0, 1.5f, 0);
            quizCanvas.rotation = posicionTrigger.rotation;
        }
    }

    public void ShowQuestion(int index)
    {
        currentIndex = index;
        answerInProgress = false;

        sentenceText.text = BridgeQuizManager.Instance.currentSentences[index];
        optionAText.text  = BridgeQuizManager.Instance.currentOptionsA[index];
        optionBText.text  = BridgeQuizManager.Instance.currentOptionsB[index];
        feedbackText.text = "";

        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();
        buttonA.onClick.AddListener(() => CheckAnswer(BridgeQuizManager.Instance.currentOptionsA[currentIndex]));
        buttonB.onClick.AddListener(() => CheckAnswer(BridgeQuizManager.Instance.currentOptionsB[currentIndex]));

        buttonA.interactable = true;
        buttonB.interactable = true;
        quizPanel.SetActive(true);
    }

    private void CheckAnswer(string selected)
    {
        if (answerInProgress) return;
        answerInProgress = true;

        // Desactivar botones inmediatamente para evitar doble clic
        buttonA.interactable = false;
        buttonB.interactable = false;

        string correct = BridgeQuizManager.Instance.currentAnswers[currentIndex];

        if (selected == correct)
        {
        feedbackText.text = "Correct!";
        feedbackText.color = Color.blue;
        // Reproducir audio correcto
        CraterTutorialManager.Instance.PlayFeedbackAudio(true);
        BridgeQuizManager.Instance.OnAnswerCorrect();
        Invoke(nameof(ClosePanel), 1f);
        }
    else
    {
        feedbackText.text = "Be careful! Try again!";
        feedbackText.color = Color.red;
        // Reproducir audio incorrecto
        CraterTutorialManager.Instance.PlayFeedbackAudio(false);
        Invoke(nameof(ChangeQuestion), 1.5f);
    }
    }

    private void ChangeQuestion()
    {
        BridgeQuizManager.Instance.ChangeCurrentQuestion();
        ShowQuestion(currentIndex);
    }

    private void ClosePanel()
    {
        quizPanel.SetActive(false);
    }
}
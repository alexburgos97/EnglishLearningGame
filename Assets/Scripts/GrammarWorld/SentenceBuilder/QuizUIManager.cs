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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // El trigger llama a este método y le pasa su posición exacta
    public void MoverCanvasAPosicion(Transform posicionTrigger)
    {
        if (quizCanvas != null)
        {
            // 1. Se teletransporta exactamente al cubo invisible que pisaste y sube 1.5 metros (altura de los ojos)
            quizCanvas.position = posicionTrigger.position + new Vector3(0, 1.5f, 0);
            
            // 2. Copia la rotación exacta del cubo invisible (CERO matemáticas de Spatial)
            quizCanvas.rotation = posicionTrigger.rotation; 
        }
    }

    public void ShowQuestion(int index)
    {
        currentIndex = index;

        sentenceText.text = BridgeQuizManager.Instance.currentSentences[index];
        optionAText.text  = BridgeQuizManager.Instance.currentOptionsA[index];
        optionBText.text  = BridgeQuizManager.Instance.currentOptionsB[index];
        feedbackText.text = "";

        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();
        buttonA.onClick.AddListener(() => CheckAnswer(BridgeQuizManager.Instance.currentOptionsA[currentIndex]));
        buttonB.onClick.AddListener(() => CheckAnswer(BridgeQuizManager.Instance.currentOptionsB[currentIndex]));

        quizPanel.SetActive(true);
    }

    private void CheckAnswer(string selected)
    {
        string correct = BridgeQuizManager.Instance.currentAnswers[currentIndex];

        if (selected == correct)
        {
            feedbackText.text = "Correct!";
            feedbackText.color = Color.green;
            Invoke(nameof(ClosePanel), 1f);
            BridgeQuizManager.Instance.OnAnswerCorrect();
        }
        else
        {
            feedbackText.text = "Be careful! Try again!";
            feedbackText.color = Color.red;
            BridgeQuizManager.Instance.OnAnswerWrong();
        }
    }

    private void ClosePanel()
    {
        quizPanel.SetActive(false);
    }
}
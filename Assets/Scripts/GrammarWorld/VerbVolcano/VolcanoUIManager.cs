using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class VolcanoUIManager : MonoBehaviour
{
    [Header("Configuración")]
    public bool isPresentSimplePlatform;

    [Header("Elementos UI (De esta ventana)")]
    public GameObject startPanel;
    public GameObject quizPanel;
    public TextMeshProUGUI sentenceText;
    public TextMeshProUGUI buttonAText;
    public TextMeshProUGUI buttonBText;
    public TextMeshProUGUI feedbackText;
    public Button buttonA;
    public Button buttonB;

    [Header("Conexiones (De esta plataforma)")]
    public VolcanoQuizManager centralManager;
    public VolcanoRock rockScript; 
    public Transform rockTargetPoint; 

    private int currentIndex = 0;
    private int maxQuestions;
    private string[,] currentBank;

    // Bancos de preguntas integrados directamente en la UI correspondiente
    private string[,] presentSimple = new string[,] {
        {"SHE", "EATS", "EAT", "EATS", "She ___ two eggs for breakfast.", "She eats two eggs for breakfast."},
        {"HE", "DRINKS", "DRINK", "DRINKS", "He ___ coffee twice a day.", "He drinks coffee twice a day."},
        {"THEY","ARE", "IS", "ARE", "They ___ always at the cafeteria.", "They are always at the cafeteria."},
        {"I", "BUY", "BOUGHT", "BUY", "I ___ three bottles of water every morning.","I buy three bottles of water every morning."},
        {"THEY","ARE", "IS", "ARE", "They ___ usually at home in the evenings.", "They are usually at home in the evenings."}
    };

    private string[,] pastSimple = new string[,] {
        {"HE", "WAS", "ARE", "WAS", "He ___ a very fast runner when he was young.", "He was a very fast runner when he was young."},
        {"I", "WASHED", "WASHES", "WASHED", "I ___ my face and brushed my teeth this morning.", "I washed my face and brushed my teeth this morning."},
        {"THEY","WERE", "IS", "WERE", "They ___ very happy together.", "They were very happy together."},
        {"IT", "PLAYED", "PLAY", "PLAYED", "It ___ with its ball in the garden.", "It played with its ball in the garden."},
        {"SHE", "BOUGHT", "BUY", "BOUGHT", "She ___ a gift for her mother's birthday.", "She bought a gift for her mother's birthday."},
        {"WE", "WERE", "IS", "WERE", "We ___ late for school.", "We were late for school."},
        {"HE", "FIXED", "FIX", "FIXED", "He ___ his bicycle yesterday.", "He fixed his bicycle yesterday."},
        {"YOU", "CLEANED", "CLEAN", "CLEANED", "You ___ your bedroom on Saturday.", "You cleaned your bedroom on Saturday."},
        {"IT", "WAS", "IS", "WAS", "It ___ a very hot day.", "It was a very hot day."},
        {"WE", "STUDIED", "STUDY", "STUDIED", "We ___ hard for the test.", "We studied hard for the test."}
    };

    void Start()
    {
        startPanel.SetActive(false);
        quizPanel.SetActive(false);
        
        if (isPresentSimplePlatform)
        {
            currentBank = presentSimple;
            maxQuestions = 5;
        }
        else
        {
            currentBank = pastSimple;
            maxQuestions = 10;
        }
    }

    public void ShowVolcanoMessage(string msg)
    {
        SpatialBridge.coreGUIService.DisplayToastMessage(msg);
    }

    public void ActivatePlatform()
    {
        if (centralManager == null) return;
        
        // Bloqueo: Si ya se completó, no vuelve a salir el Start
        if (isPresentSimplePlatform && centralManager.presentSimpleCompleted) return;
        if (!isPresentSimplePlatform && centralManager.pastSimpleCompleted) return;

        startPanel.SetActive(true);
    }

    // Este método lo conectaremos al botón START
    public void StartQuiz()
    {
        startPanel.SetActive(false);
        currentIndex = 0;
        LoadQuestion();
    }

    private void LoadQuestion()
    {
        if (currentIndex >= maxQuestions)
        {
            quizPanel.SetActive(false);
            centralManager.PlatformCompleted(isPresentSimplePlatform);
            return;
        }

        string pronoun = currentBank[currentIndex, 0];
        string verbA = currentBank[currentIndex, 1];
        string verbB = currentBank[currentIndex, 2];
        string sentence = currentBank[currentIndex, 4];

        sentenceText.text = sentence;
        buttonAText.text = verbA;
        buttonBText.text = verbB;
        feedbackText.text = "";

        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();
        buttonA.onClick.AddListener(() => CheckAnswer(verbA));
        buttonB.onClick.AddListener(() => CheckAnswer(verbB));

        buttonA.interactable = true;
        buttonB.interactable = true;

        quizPanel.SetActive(true);

        if (rockScript != null && rockTargetPoint != null)
        {
            rockScript.Launch(pronoun, rockTargetPoint);
        }
    }

    private void CheckAnswer(string selected)
    {
        string correct = currentBank[currentIndex, 3];
        string fullSentence = currentBank[currentIndex, 5];

        buttonA.interactable = false;
        buttonB.interactable = false;

        if (selected == correct)
        {
            feedbackText.text = "Correct! " + fullSentence;
            feedbackText.color = Color.green;
            currentIndex++;
            Invoke(nameof(LoadQuestion), 2.5f);
        }
        else
        {
            feedbackText.text = "Don't worry! The correct answer is:\n" + fullSentence;
            feedbackText.color = Color.red;
            Invoke(nameof(ResetCurrentQuestion), 3.5f);
        }
    }

    private void ResetCurrentQuestion()
    {
        LoadQuestion();
    }
}
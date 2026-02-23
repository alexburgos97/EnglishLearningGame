using UnityEngine;
using SpatialSys.UnitySDK;

public class BridgeQuizManager : MonoBehaviour
{
    public static BridgeQuizManager Instance { get; private set; }

    [Header("Arrastra aquí tus cubos en orden")]
    public BridgeBlock[] bridgeBlocks;

    private string[,] allQuestions = new string[,]
    {
        {"I ___ hungry every morning.",                  "AM",    "IS",     "AM"},
        {"I ___ rice and chicken every day.",            "EAT",   "EATS",   "EAT"},
        {"She ___ two eggs for breakfast.",              "EAT",   "EATS",   "EATS"},
        {"She ___ a good cook.",                         "IS",    "AM",     "IS"},
        {"He ___ coffee twice a day.",                   "DRINK", "DRINKS", "DRINKS"},
        {"We ___ for four people on Sundays.",           "COOK",  "COOKS",  "COOK"},
        {"They ___ always at the cafeteria.",            "IS",    "ARE",    "ARE"},
        {"We usually ___ lunch at 1:00 p.m.",            "HAVE",  "HAS",    "HAVE"},
        {"My friends ___ one glass of juice every day.", "DRINK", "DRINKS", "DRINK"},
        {"My sandwich ___ very big.",                    "ARE",   "IS",     "IS"},
        {"She always ___ fruit for breakfast.",          "EAT",   "EATS",   "EATS"},
        {"I ___ three bottles of water every morning.",  "BUY",   "BUYS",   "BUY"},
        {"We ___ never late for breakfast.",             "IS",    "ARE",    "ARE"},
        {"They sometimes ___ pizza for dinner.",         "BUY",   "BUYS",   "BUY"},
        {"I ___ ten vegetables every week.",             "EAT",   "EATS",   "EAT"},
        {"I never ___ soda in the morning.",             "DRINK", "DRINKS", "DRINK"},
        {"My family ___ dinner three times a week.",     "COOK",  "COOKS",  "COOKS"},
        {"We ___ to the market every Saturday.",         "GO",    "GOES",   "GO"},
        {"He ___ apples but doesn't like bananas.",      "LIKE",  "LIKES",  "LIKES"},
        {"They ___ usually at home in the evenings.",    "IS",    "ARE",    "ARE"},
    };

    [HideInInspector] public string[] currentSentences;
    [HideInInspector] public string[] currentOptionsA;
    [HideInInspector] public string[] currentOptionsB;
    [HideInInspector] public string[] currentAnswers;

    private int currentIndex = 0;
    public bool quizActive = false;
    private int totalQuestions => allQuestions.GetLength(0);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        int blocksCount = bridgeBlocks.Length;
        if (blocksCount > totalQuestions)
        {
            Debug.LogError("Tienes más bloques que preguntas!");
            return;
        }

        currentSentences = new string[blocksCount];
        currentOptionsA  = new string[blocksCount];
        currentOptionsB  = new string[blocksCount];
        currentAnswers   = new string[blocksCount];

        SelectRandomQuestions(blocksCount);
    }

    private void SelectRandomQuestions(int cantidad)
    {
        int[] indices = new int[totalQuestions];
        for (int i = 0; i < totalQuestions; i++) indices[i] = i;

        for (int i = totalQuestions - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }

        for (int i = 0; i < cantidad; i++)
        {
            int q = indices[i];
            currentSentences[i] = allQuestions[q, 0];
            currentOptionsA[i]  = allQuestions[q, 1];
            currentOptionsB[i]  = allQuestions[q, 2];
            currentAnswers[i]   = allQuestions[q, 3];
        }
    }

    public void OnAvatarReachedEdge(int blockIndex)
    {
        if (quizActive) return;
        if (blockIndex != currentIndex) return;

        quizActive = true;
        QuizUIManager.Instance.ShowQuestion(currentIndex);
    }

    public void OnAnswerCorrect()
    {
        bridgeBlocks[currentIndex].MoveToPosition();
        currentIndex++;
        quizActive = false;

        if (currentIndex >= bridgeBlocks.Length)
            OnBridgeComplete();
    }

    public void OnAnswerWrong()
    {
        quizActive = false;
    }

    private void OnBridgeComplete()
    {
        SpatialBridge.coreGUIService.DisplayToastMessage("Perfect! The bridge is stable!");
    }
}
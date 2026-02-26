using UnityEngine;
using SpatialSys.UnitySDK;

public class VolcanoQuizManager : MonoBehaviour
{
    public static VolcanoQuizManager Instance { get; private set; }

    [Header("Referencias")]
    public VolcanoRock rock;
    public Transform puntoLanzamiento;
    public Transform puntoJugadorPS;
    public Transform puntoJugadorPA;

    // Banco Presente Simple
    private string[,] presentSimple = new string[,]
    {
        {"SHE", "EATS",    "EAT",    "EATS",    "She ___ two eggs for breakfast.",          "She eats two eggs for breakfast."},
        {"HE",  "DRINKS",  "DRINK",  "DRINKS",  "He ___ coffee twice a day.",               "He drinks coffee twice a day."},
        {"THEY","ARE",     "IS",     "ARE",     "They ___ always at the cafeteria.",         "They are always at the cafeteria."},
        {"I",   "BUY",     "BOUGHT", "BUY",     "I ___ three bottles of water every morning.","I buy three bottles of water every morning."},
        {"THEY","ARE",     "IS",     "ARE",     "They ___ usually at home in the evenings.", "They are usually at home in the evenings."},
    };

    // Banco Pasado Simple
    private string[,] pastSimple = new string[,]
    {
        {"HE",  "WAS",     "ARE",    "WAS",     "He ___ a very fast runner when he was young.",           "He was a very fast runner when he was young."},
        {"I",   "WASHED",  "WASHES", "WASHED",  "I ___ my face and brushed my teeth this morning.",       "I washed my face and brushed my teeth this morning."},
        {"THEY","WERE",    "IS",     "WERE",    "They ___ very happy together.",                           "They were very happy together."},
        {"IT",  "PLAYED",  "PLAY",   "PLAYED",  "It ___ with its ball in the garden.",                    "It played with its ball in the garden."},
        {"SHE", "BOUGHT",  "BUY",    "BOUGHT",  "She ___ a gift for her mother's birthday.",              "She bought a gift for her mother's birthday."},
        {"WE",  "WERE",    "IS",     "WERE",    "We ___ late for school.",                                "We were late for school."},
        {"HE",  "FIXED",   "FIX",    "FIXED",   "He ___ his car in the garage yesterday.",               "He fixed his car in the garage yesterday."},
        {"YOU", "CLEANED", "CLEANS", "CLEANED", "You ___ your room because you were very busy.",          "You cleaned your room because you were very busy."},
        {"IT",  "WAS",     "ARE",    "WAS",     "It ___ a beautiful sunny day yesterday.",               "It was a beautiful sunny day yesterday."},
        {"WE",  "STUDIED", "STUDIES","STUDIED", "We ___ for the exam because we wanted a good grade.",   "We studied for the exam because we wanted a good grade."},
    };

    [HideInInspector] public string[] currentPronouns;
    [HideInInspector] public string[] currentSentences;
    [HideInInspector] public string[] currentVerbsA;
    [HideInInspector] public string[] currentVerbsB;
    [HideInInspector] public string[] currentAnswers;
    [HideInInspector] public string[] currentFullSentences;

    private int currentIndex = 0;
    private bool gameActive = false;
    private bool isPresentSimple = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowStartPanel(bool presentSimple)
    {
        isPresentSimple = presentSimple;
        VolcanoUIManager.Instance.ShowStartPanel();
    }

    public void StartGame()
    {
        currentIndex = 0;
        gameActive = true;

        if (isPresentSimple)
            PrepareQuestions(presentSimple, 5);
        else
            PrepareQuestions(pastSimple, 10);

        LaunchNextRock();
    }

    private void PrepareQuestions(string[,] bank, int count)
    {
        currentPronouns      = new string[count];
        currentSentences     = new string[count];
        currentVerbsA        = new string[count];
        currentVerbsB        = new string[count];
        currentAnswers       = new string[count];
        currentFullSentences = new string[count];

        // Fisher-Yates shuffle
        int total = bank.GetLength(0);
        int[] indices = new int[total];
        for (int i = 0; i < total; i++) indices[i] = i;
        for (int i = total - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }

        for (int i = 0; i < count; i++)
        {
            int q = indices[i];
            currentPronouns[i]      = bank[q, 0];
            currentVerbsA[i]        = bank[q, 1];
            currentVerbsB[i]        = bank[q, 2];
            currentAnswers[i]       = bank[q, 3];
            currentSentences[i]     = bank[q, 4];
            currentFullSentences[i] = bank[q, 5];
        }
    }

    private void LaunchNextRock()
    {
        if (currentIndex >= currentPronouns.Length)
        {
            OnGameComplete();
            return;
        }

        Transform target = isPresentSimple ? puntoJugadorPS : puntoJugadorPA;
        rock.Launch(currentPronouns[currentIndex], target);
        VolcanoUIManager.Instance.ShowQuestion(currentIndex);
    }

    public void OnRockLanded()
    {
        if (!gameActive) return;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Too slow! Try again!");
        VolcanoUIManager.Instance.ClosePanel();
        Invoke(nameof(RetryQuestion), 1.5f);
    }

    private void RetryQuestion()
    {
        Transform target = isPresentSimple ? puntoJugadorPS : puntoJugadorPA;
        rock.Launch(currentPronouns[currentIndex], target);
        VolcanoUIManager.Instance.ShowQuestion(currentIndex);
    }

    public void OnAnswerCorrect()
    {
        currentIndex++;
        LaunchNextRock();
    }

    public void OnAnswerWrong()
    {
        Transform target = isPresentSimple ? puntoJugadorPS : puntoJugadorPA;
        rock.Launch(currentPronouns[currentIndex], target);
        VolcanoUIManager.Instance.ShowQuestion(currentIndex);
    }

    private void OnGameComplete()
    {
        gameActive = false;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Amazing! You completed the Volcano Challenge!");
        GameProgressManager.Instance.AwardVerbMasterMedal();
    }
}
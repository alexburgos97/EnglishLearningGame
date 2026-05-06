using UnityEngine;
using SpatialSys.UnitySDK;

public class VolcanoQuizManager : MonoBehaviour
{
    public static VolcanoQuizManager Instance { get; private set; }

    [Header("Referencias")]
    public VolcanoRock rock;
    public Transform puntoLanzamiento;
    public GameObject[] collidersLava;
    public Transform puntoReaparicion;

    [Header("Puntos de jugador")]
    public Transform posicionPS;
    public Transform posicionPA;

    // Control de completado
    public bool PresentSimpleCompleted => presentSimpleCompleted;
    private bool presentSimpleCompleted = false;
    private bool pastSimpleCompleted = false;
    private bool psStarted = false;
    private bool paStarted = false;

    // Banco Presente Simple
    private string[,] presentSimple = new string[,]
    {
        {"SHE", "EATS",    "EAT",    "EATS",    "___ ___ two eggs for breakfast.",            "She eats two eggs for breakfast."},
        {"HE",  "DRINKS",  "DRINK",  "DRINKS",  "___ ___ coffee twice a day.",                "He drinks coffee twice a day."},
        {"THEY","ARE",     "IS",     "ARE",     "___ ___ always at the cafeteria.",            "They are always at the cafeteria."},
        {"I",   "BUY",     "BOUGHT", "BUY",     "___ ___ three bottles of water every morning.","I buy three bottles of water every morning."},
        {"THEY","ARE",     "IS",     "ARE",     "___ ___ usually at home in the evenings.",    "They are usually at home in the evenings."},
    };

    // Banco Pasado Simple
    private string[,] pastSimple = new string[,]
    {
        {"HE",  "WAS",     "ARE",    "WAS",     "___ ___ a very fast runner when he was young.",          "He was a very fast runner when he was young."},
        {"I",   "WASHED",  "WASHES", "WASHED",  "___ ___ my face and brushed my teeth this morning.",     "I washed my face and brushed my teeth this morning."},
        {"THEY","WERE",    "IS",     "WERE",    "___ ___ very happy together.",                            "They were very happy together."},
        {"IT",  "PLAYED",  "PLAY",   "PLAYED",  "___ ___ with its ball in the garden.",                   "It played with its ball in the garden."},
        {"SHE", "BOUGHT",  "BUY",    "BOUGHT",  "___ ___ a gift for her mother's birthday.",              "She bought a gift for her mother's birthday."},
        {"WE",  "WERE",    "IS",     "WERE",    "___ ___ late for school.",                               "We were late for school."},
        {"HE",  "FIXED",   "FIX",    "FIXED",   "___ ___ his car in the garage yesterday.",              "He fixed his car in the garage yesterday."},
        {"YOU", "CLEANED", "CLEANS", "CLEANED", "___ ___ your room because you were very busy.",          "You cleaned your room because you were very busy."},
        {"IT",  "WAS",     "ARE",    "WAS",     "___ ___ a beautiful sunny day yesterday.",              "It was a beautiful sunny day yesterday."},
        {"WE",  "STUDIED", "STUDIES","STUDIED", "___ ___ for the exam because we wanted a good grade.",  "We studied for the exam because we wanted a good grade."},
        {"HE",  "PAINTED", "PAINT",  "PAINTED", "___ ___ a picture of himself in the mirror.",           "He painted a picture of himself in the mirror."},
        {"THEY","VISITED", "VISITS", "VISITED", "___ ___ their grandparents last weekend.",              "They visited their grandparents last weekend."},
        {"I",   "COOKED",  "COOKS",  "COOKED",  "___ ___ dinner for myself because I was alone.",        "I cooked dinner for myself because I was alone."},
        {"SHE", "WAS",     "ARE",    "WAS",     "___ ___ tired, so she went to bed early.",              "She was tired, so she went to bed early."},
        {"YOU", "WERE",    "IS",     "WERE",    "___ ___ my best friend when we were children.",         "You were my best friend when we were children."},
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

    public void TryStartPresentSimple()
    {
        if (presentSimpleCompleted) return;
        if (psStarted) return;
        VolcanoUIManager.Instance.ShowStartPanel(true);
    }

    public void TryStartPastSimple()
    {
        if (!presentSimpleCompleted)
        {
            SpatialBridge.coreGUIService.DisplayToastMessage(
                "Complete Present Simple first!");
            return;
        }
        if (pastSimpleCompleted) return;
        if (paStarted) return;
        VolcanoUIManager.Instance.ShowStartPanel(false);
    }

    public void StartGame(bool isPS)
    {
        isPresentSimple = isPS;
        currentIndex = 0;
        gameActive = true;

        if (isPresentSimple)
        {
            psStarted = true;
            PrepareQuestions(presentSimple, 5);
        }
        else
        {
            paStarted = true;
            PrepareQuestions(pastSimple, 15);
        }

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
            OnSectionComplete();
            return;
        }

        Transform target = isPresentSimple ? posicionPS : posicionPA;
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
        Transform target = isPresentSimple ? posicionPS : posicionPA;
        rock.Launch(currentPronouns[currentIndex], target);
        VolcanoUIManager.Instance.ShowQuestion(currentIndex);
    }

    public void OnRockLanded()
    {
        if (!gameActive) return;
        SpatialBridge.coreGUIService.DisplayToastMessage("Too slow! Try again!");
        VolcanoUIManager.Instance.ClosePanel();
        Invoke(nameof(RetryQuestion), 1.5f);
    }

    private void RetryQuestion()
    {
        Transform target = isPresentSimple ? posicionPS : posicionPA;
        rock.Launch(currentPronouns[currentIndex], target);
        VolcanoUIManager.Instance.ShowQuestion(currentIndex);
    }

    private void OnSectionComplete()
    {
        gameActive = false;

        if (isPresentSimple)
        {
            presentSimpleCompleted = true;
            SpatialBridge.coreGUIService.DisplayToastMessage(
                "Great! Now go to the right side for Past Simple!");
        }
        else
        {
            pastSimpleCompleted = true;
            CheckBothCompleted();
        }
    }

    private void CheckBothCompleted()
{
    if (presentSimpleCompleted && pastSimpleCompleted)
    {
        // Desactivar TODOS los colliders de lava
        foreach (GameObject collider in collidersLava)
        {
            if (collider != null)
                collider.SetActive(false);
        }

        // Enfriar efecto de lava
        LavaEffect lavaEffect = FindObjectOfType<LavaEffect>();
        if (lavaEffect != null)
            lavaEffect.CoolLava();

        // Apagar todos los LavaBlockers
        LavaBlocker[] blockers = FindObjectsOfType<LavaBlocker>();
        foreach (LavaBlocker blocker in blockers)
            blocker.CoolDown();

        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Amazing! Cross the volcano to claim your medal!");
    }
}
    public int GetPresentSimpleCount()
        {
        return presentSimple.GetLength(0);
    }

    public string GetPronoun(int index)
        {
        return presentSimple[index, 0];
    }

    public string GetVerbA(int index)
        {
        return presentSimple[index, 1];
    }

    public string GetVerbB(int index)
        {
        return presentSimple[index, 2];
    }

    public string GetAnswer(int index)
        {
        return presentSimple[index, 3];
    }

    public string GetSentence(int index)
        {
        return presentSimple[index, 4];
    }
}
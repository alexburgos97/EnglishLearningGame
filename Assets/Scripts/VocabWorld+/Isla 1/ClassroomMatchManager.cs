using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class ClassroomMatchManager : MonoBehaviour
{
    public static ClassroomMatchManager Instance { get; private set; }

    [Header("Panel principal")]
    public GameObject classroomPanel;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("Imagenes de objetos (3 slots)")]
    public Image[] objectImages;

    [Header("Palabras draggables (3 slots)")]
    public DraggableWord[] draggableWords;

    [Header("Sprites de los 9 objetos en orden")]
    public Sprite[] objectSprites;

    [Header("Audios")]
    public AudioClip dragAudio;
    public AudioClip correctAudio;
    public AudioClip incorrectAudio;

    private AudioSource audioSource;

    private string[] allWords = new string[]
    {
        "BACKPACK", "RULER", "BOOK",
        "GLUE", "SCISSORS", "PENCIL",
        "STAPLER", "MARKER", "SHARPENER"
    };

    private int currentRound = 0;
    private int consecutiveCorrect = 0;
    private bool[] completedInThisRound = new bool[3];
    private bool activityCompleted = false;

    private int[][] rounds = new int[][]
    {
        new int[] {0, 1, 2},
        new int[] {3, 4, 5},
        new int[] {6, 7, 8}
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        classroomPanel.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    public void ShowPanel()
    {
    if (activityCompleted) return;
    classroomPanel.SetActive(true);
    currentRound = 0;
    consecutiveCorrect = 0;
    Invoke(nameof(LoadFirstRound), 0.1f);
    }

private void LoadFirstRound()
{
    LoadRound(0);
}

    private void LoadRound(int roundIndex)
    {
        completedInThisRound = new bool[3] { false, false, false };
        consecutiveCorrect = 0;

        // Mezclar indices de imagenes
        int[] imageIndices = new int[]
        {
            rounds[roundIndex][0],
            rounds[roundIndex][1],
            rounds[roundIndex][2]
        };

        for (int i = imageIndices.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = imageIndices[i];
            imageIndices[i] = imageIndices[j];
            imageIndices[j] = temp;
        }

        // Asignar imagenes mezcladas con su palabra correcta
        for (int i = 0; i < 3; i++)
        {
            int wordIndex = imageIndices[i];

            if (objectImages[i] != null && objectSprites[wordIndex] != null)
                objectImages[i].sprite = objectSprites[wordIndex];

            DroppableObject droppable = objectImages[i].GetComponent<DroppableObject>();
            if (droppable != null)
            {
                droppable.SetCorrectWord(allWords[wordIndex]);
                droppable.ResetObject();
            }
        }

        // Mezclar palabras draggables independientemente
        string[] wordPool = new string[]
        {
            allWords[rounds[roundIndex][0]],
            allWords[rounds[roundIndex][1]],
            allWords[rounds[roundIndex][2]]
        };

        for (int i = wordPool.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = wordPool[i];
            wordPool[i] = wordPool[j];
            wordPool[j] = temp;
        }

        for (int i = 0; i < 3; i++)
        {
            if (draggableWords[i] != null)
            {
                draggableWords[i].wordInEnglish = wordPool[i];
                draggableWords[i].ResetWord();
            }
        }

        feedbackText.text = "";
    }

    public void OnWordMatched(int slotIndex)
    {
        if (activityCompleted) return;
        if (completedInThisRound[slotIndex]) return;

        completedInThisRound[slotIndex] = true;
        consecutiveCorrect++;

        ShowFeedback("Correct!", Color.green);

        if (consecutiveCorrect >= 3)
        {
            // Audio correcto solo cuando completa las 3
            if (correctAudio != null)
            {
                audioSource.clip = correctAudio;
                audioSource.Play();
            }
            Invoke(nameof(OnComplete), 1.5f);
        }
    }

    public void OnWordFailed()
    {
        if (activityCompleted) return;

        // Audio incorrecto
        if (incorrectAudio != null)
        {
            audioSource.clip = incorrectAudio;
            audioSource.Play();
        }

        ShowFeedback("TRY AGAIN!", Color.red);
        consecutiveCorrect = 0;
        currentRound = (currentRound + 1) % rounds.Length;
        Invoke(nameof(LoadNextRound), 2f);
    }

    private void LoadNextRound()
    {
        LoadRound(currentRound);
    }

    public void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
        }
    }

    private void OnComplete()
    {
        activityCompleted = true;
        ShowFeedback("Well done! All words matched!", Color.green);

        // Añadir las 3 palabras de la ronda correcta al VocabCard
        foreach (int index in rounds[currentRound])
            VocabCardManager.Instance.AddSchoolWord(allWords[index]);

        Invoke(nameof(CompleteZona), 1.5f);
    }

    private void CompleteZona()
    {
        classroomPanel.SetActive(false);
        Isla1Manager.Instance.OnZonaEscolarComplete();
    }
}
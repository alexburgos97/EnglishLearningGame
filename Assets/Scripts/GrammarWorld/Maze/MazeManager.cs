using UnityEngine;
using SpatialSys.UnitySDK;

public class MazeManager : MonoBehaviour
{
    public static MazeManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject syntaxSprite;
    public AudioSource audioSource;
    public GameObject triggerEntrada;
    public GameObject triggerSalida;

    [Header("Triggers del laberinto")]
    public GameObject[] triggers = new GameObject[10];

    [Header("Audios de instrucciones")]
    public AudioClip[] instrucciones = new AudioClip[10];

    [Header("Textos de instrucciones")]
    public string[] textos = new string[]
    {
        "Walk past the two blue flowers now, and stop in front of the tall rock.",
        "Tomorrow, you will find the coin under the bridge. Today, go to the tree.",
        "You turned right at the apple yesterday. Today, turn left at the rock.",
        "Jump on the green platform, then go next to the red house.",
        "You will stand behind the exit. First, walk past the three blue platforms.",
        "Last game, you looked between the two rocks. Now, look under the bridge.",
        "Find the key behind the house, and walk to the crystal arch.",
        "In the next level, you will go on the red platform. Now, go to the green platform.",
        "The player climbed on the bridge a minute ago. Now, walk under the bridge.",
        "Go to the big apple, turn right, and stop next to the small tree."
    };

    private int currentStep = 0;
    private bool gameStarted = false;
    private bool isTalking = false;
    private Vector3 originalScale;
    private float pulseTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        syntaxSprite.SetActive(false);
        triggerSalida.SetActive(false);

        // Solo Trigger_1 activo al inicio, los demás desactivados
        for (int i = 0; i < triggers.Length; i++)
            triggers[i].SetActive(false);

        originalScale = syntaxSprite.transform.localScale;
    }

    void Update()
    {
        if (!isTalking) return;

        // Efecto de pulso mientras habla
        pulseTimer += Time.deltaTime * 5f;
        float pulse = 1f + Mathf.Sin(pulseTimer) * 0.1f;
        syntaxSprite.transform.localScale = originalScale * pulse;

        // Detectar cuando termina el audio
        if (!audioSource.isPlaying)
        {
            isTalking = false;
            syntaxSprite.transform.localScale = originalScale;
        }
    }

    public void StartMaze()
    {
        gameStarted = true;
        currentStep = 0;
        triggerEntrada.SetActive(false);
        triggers[0].SetActive(true);
        ShowInstruction(0);
    }

    public void OnTriggerReached(int step)
    {
        if (step != currentStep) return;

        // Desactivar trigger actual
        triggers[currentStep].SetActive(false);

        // Mover SyntaxSprite al trigger actual
        syntaxSprite.SetActive(true);
        syntaxSprite.transform.position =
            triggers[currentStep].transform.position + Vector3.up * 1.5f;

        currentStep++;

        if (currentStep < triggers.Length)
        {
            // Activar siguiente trigger
            triggers[currentStep].SetActive(true);
            ShowInstruction(currentStep);
        }
        else
        {
            // Completó los 10 pasos
            ShowInstruction(9);
            triggerSalida.SetActive(true);
        }
    }

    private void ShowInstruction(int index)
    {
        MazeUIManager.Instance.ShowInstruction(textos[index]);

        if (instrucciones[index] != null)
        {
            audioSource.clip = instrucciones[index];
            audioSource.Play();
            isTalking = true;
            pulseTimer = 0f;
        }
    }

    public void OnMazeCompleted()
    {
        syntaxSprite.SetActive(false);
        MazeUIManager.Instance.HidePanel();
        GameProgressManager.Instance.AwardPathfinderMedal();
    }
}
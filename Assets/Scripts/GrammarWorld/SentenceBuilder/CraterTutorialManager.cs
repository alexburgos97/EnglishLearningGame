using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class CraterTutorialManager : MonoBehaviour
{
    public static CraterTutorialManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject syntaxSprite;
    public AudioSource audioSource;
    public BridgeBlock firstBlock;

    [Header("Audios de fases")]
    public AudioClip audioPhase1;
    public AudioClip audioPhase2;
    public AudioClip audioPhase3;
    public AudioClip audioPhase4;

    [Header("UI")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI instructionText;
    public Button nextButton;
    public GameObject demoQuestionPanel;
    public TextMeshProUGUI demoSentenceText;
    public Button demoOption1Button;
    public Button demoOption2Button;
    public TextMeshProUGUI demoOption1Text;
    public TextMeshProUGUI demoOption2Text;
    public TextMeshProUGUI demoFeedbackText;
    public GameObject botonesPanel;
    public Button repetirButton;
    public Button playButton;

    private string[] phaseTexts = new string[]
    {
        "Welcome to the Great Crater! Syntaxia is in a mess. Look at the bridge... it is broken! We need to fix it to cross the crater.",
        "Let's practice the Present Simple. Look at the sentence and the pronoun (I, You, He, She...). Then, choose the correct verb from the two options.",
        "Each correct answer moves one block of the bridge. Build the path to reach the other side! If you need help, click on the Study Resources.",
        "Are you ready? Complete the bridge to win the Builder's Medal for your passport. Let's start building!"
    };

    private string[,] demoQuestions = new string[,]
    {
        {"She ___ two eggs for breakfast.", "EAT", "EATS", "EATS"},
        {"He ___ coffee twice a day.",      "DRINK", "DRINKS", "DRINKS"},
        {"They ___ at home in the evenings.","IS", "ARE", "ARE"},
        {"I ___ rice every day.",           "EAT", "EATS", "EAT"}
    };

    private int currentPhase = 0;
    private int currentDemoQuestion = 0;
    private bool isTalking = false;
    private Vector3 originalScale;
    private float pulseTimer = 0f;
    private bool tutorialCompleted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        tutorialPanel.SetActive(false);
        demoQuestionPanel.SetActive(false);
        botonesPanel.SetActive(false);
        nextButton.gameObject.SetActive(false);

        originalScale = syntaxSprite.transform.localScale;

        audioSource = syntaxSprite.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = syntaxSprite.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        nextButton.onClick.AddListener(OnNextClicked);
        repetirButton.onClick.AddListener(OnRepetirClicked);
        playButton.onClick.AddListener(OnPlayClicked);

        demoOption1Button.onClick.AddListener(() => CheckDemoAnswer(demoOption1Text.text));
        demoOption2Button.onClick.AddListener(() => CheckDemoAnswer(demoOption2Text.text));
    }

    void Update()
    {
        if (!isTalking) return;
        pulseTimer += Time.deltaTime * 5f;
        float pulse = 1f + Mathf.Sin(pulseTimer) * 0.1f;
        syntaxSprite.transform.localScale = originalScale * pulse;
        if (!audioSource.isPlaying)
        {
            isTalking = false;
            syntaxSprite.transform.localScale = originalScale;
        }
    }

    public void StartTutorial()
    {
        if (tutorialCompleted) return;

        tutorialPanel.SetActive(true);
        currentPhase = 0;
        ShowPhase();
    }

    private void ShowPhase()
    {
        instructionText.text = phaseTexts[currentPhase];
        demoQuestionPanel.SetActive(false);
        botonesPanel.SetActive(false);
        nextButton.gameObject.SetActive(false);

        AudioClip clip = null;
        switch (currentPhase)
        {
            case 0: clip = audioPhase1; break;
            case 1: clip = audioPhase2; break;
            case 2: clip = audioPhase3; break;
            case 3: clip = audioPhase4; break;
        }
        PlayAudio(clip);

        if (currentPhase == 2)
        {
            // Fase 3 - mostrar demo
            ShowDemoQuestion();
        }
        else if (currentPhase == 3)
        {
            // Fase 4 - mostrar botones repetir y play
            botonesPanel.SetActive(true);
        }
        else
        {
            // Fases 1 y 2 - mostrar boton next
            nextButton.gameObject.SetActive(true);
        }
    }

    private void ShowDemoQuestion()
    {
        demoQuestionPanel.SetActive(true);
        demoFeedbackText.text = "";

        int q = currentDemoQuestion % demoQuestions.GetLength(0);
        demoSentenceText.text = demoQuestions[q, 0];
        demoOption1Text.text = demoQuestions[q, 1];
        demoOption2Text.text = demoQuestions[q, 2];

        demoOption1Button.interactable = true;
        demoOption2Button.interactable = true;
    }

    private void CheckDemoAnswer(string selected)
    {
        int q = currentDemoQuestion % demoQuestions.GetLength(0);
        string correct = demoQuestions[q, 3];

        demoOption1Button.interactable = false;
        demoOption2Button.interactable = false;

        if (selected == correct)
        {
            demoFeedbackText.text = "Well done!";
            demoFeedbackText.color = Color.green;

            // Mover el primer bloque como demostracion
            if (firstBlock != null)
                firstBlock.MoveToPosition();

            Invoke(nameof(GoToPhase4), 2f);
        }
        else
        {
            demoFeedbackText.text = "Try again!";
            demoFeedbackText.color = Color.red;
            currentDemoQuestion++;
            Invoke(nameof(ShowDemoQuestion), 1.5f);
        }
    }

    private void GoToPhase4()
    {
        currentPhase = 3;
        ShowPhase();
    }

    private void OnNextClicked()
    {
        currentPhase++;
        ShowPhase();
    }

    private void OnRepetirClicked()
    {
        currentPhase = 0;
        ShowPhase();
    }

    private void OnPlayClicked()
    {
        tutorialCompleted = true;
        tutorialPanel.SetActive(false);

        // Resetear el primer bloque a su posicion original
        if (firstBlock != null)
            firstBlock.ResetPosition();

        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Game started! Cross the bridge!");
    }

    private void PlayAudio(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.clip = clip;
        audioSource.Play();
        isTalking = true;
        pulseTimer = 0f;
    }
}
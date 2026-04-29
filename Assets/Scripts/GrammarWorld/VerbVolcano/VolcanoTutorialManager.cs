using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class VolcanoTutorialManager : MonoBehaviour
{
    public static VolcanoTutorialManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject syntaxSprite;
    public AudioSource audioSource;
    public VolcanoRock rock;
    public Transform puntoLanzamiento;
    public Transform puntoDemo;

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
        "Welcome to the Verb Volcano! Sparky here. The volcano is angry and will throw rocks with pronouns.",
        "You will start on the LEFT platform with Present Simple verbs. After completing it, move to the RIGHT platform for Past Simple verbs.",
        "Watch! A rock will appear with a pronoun. Read the sentence and choose the correct verb to match the pronoun. Try it!",
        "Are you ready to master the verbs? Go to the LEFT platform and start launching!"
    };

    private int currentPhase = 0;
    private int lastDemoIndex = -1;
    private string currentCorrectAnswer = "";
    private bool isTalking = false;
    private Vector3 originalScale;
    private float pulseTimer = 0f;
    private bool tutorialCompleted = false;
    private bool answerInProgress = false;

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
            ShowDemoQuestion();
        else if (currentPhase == 3)
            botonesPanel.SetActive(true);
        else
            nextButton.gameObject.SetActive(true);
    }

    private void ShowDemoQuestion()
    {
        demoQuestionPanel.SetActive(true);
        demoFeedbackText.text = "";
        answerInProgress = false;

        // Acceder al banco de Presente Simple del VolcanoQuizManager
        int count = VolcanoQuizManager.Instance.GetPresentSimpleCount();

        // Seleccionar pregunta aleatoria diferente a la anterior
        int q;
        do {
            q = Random.Range(0, count);
        } while (q == lastDemoIndex && count > 1);
        lastDemoIndex = q;

        string pronoun = VolcanoQuizManager.Instance.GetPronoun(q);
        string sentence = VolcanoQuizManager.Instance.GetSentence(q);
        string verbA = VolcanoQuizManager.Instance.GetVerbA(q);
        string verbB = VolcanoQuizManager.Instance.GetVerbB(q);
        currentCorrectAnswer = VolcanoQuizManager.Instance.GetAnswer(q);

        demoSentenceText.text = sentence;
        demoOption1Text.text = verbA;
        demoOption2Text.text = verbB;

        demoOption1Button.interactable = true;
        demoOption2Button.interactable = true;

        if (rock != null && puntoLanzamiento != null && puntoDemo != null)
            rock.Launch(pronoun, puntoDemo);
    }

    private void CheckDemoAnswer(string selected)
    {
        if (answerInProgress) return;
        answerInProgress = true;

        demoOption1Button.interactable = false;
        demoOption2Button.interactable = false;

        if (selected == currentCorrectAnswer)
        {
            demoFeedbackText.text = "Well done!";
            demoFeedbackText.color = Color.green;
            Invoke(nameof(GoToPhase4), 2f);
        }
        else
        {
            demoFeedbackText.text = "Try again!";
            demoFeedbackText.color = Color.red;
            Invoke(nameof(ShowDemoQuestion), 1.5f);
        }
    }

    private void GoToPhase4()
    {
        if (rock != null)
            rock.gameObject.SetActive(false);
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
        lastDemoIndex = -1;
        ShowPhase();
    }

    private void OnPlayClicked()
    {
        tutorialCompleted = true;
        tutorialPanel.SetActive(false);

        if (rock != null)
            rock.gameObject.SetActive(false);

        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Go to the LEFT platform and start!");
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
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PassportManager : MonoBehaviour
{
    // La escena no tiene ningún GameObject EventSystem (se perdió en la migración
    // fuera de Spatial), así que el GraphicRaycaster del Canvas nunca recibía clics
    // y ningún botón UI —incluido el ícono del pasaporte— respondía. Se crea uno
    // automáticamente antes de que cargue la escena si no existe.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;

        GameObject go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        DontDestroyOnLoad(go);
    }

    public static PassportManager Instance { get; private set; }

    [Header("Paneles")]
    public GameObject passportPanel;
    public Button passportIcon;

    [Header("Medallas GrammarWorld - Imágenes")]
    public Image medalBuilders;
    public Image medalVerbMaster;
    public Image medalPathfinder;
    public Image medalSentenceBuilder;

    [Header("Medallas VocabWorld+ - Imágenes")]
    public Image medalDailyLifeScout;
    public Image medalNumberCruncher;
    public Image medalGlobalCitizen;
    public Image medalLexiconLegend;

    [Header("GrammarWorld - Sprites en color")]
    public Sprite buildersColor;
    public Sprite verbMasterColor;
    public Sprite pathfinderColor;
    public Sprite sentenceBuilderColor;

    [Header("GrammarWorld - Sprites en gris")]
    public Sprite buildersGray;
    public Sprite verbMasterGray;
    public Sprite pathfinderGray;
    public Sprite sentenceBuilderGray;

    [Header("VocabWorld+ - Sprites en color")]
    public Sprite dailyLifeScoutColor;
    public Sprite numberCruncherColor;
    public Sprite globalCitizenColor;
    public Sprite lexiconLegendColor;

    [Header("VocabWorld+ - Sprites en gris")]
    public Sprite dailyLifeScoutGray;
    public Sprite numberCruncherGray;
    public Sprite globalCitizenGray;
    public Sprite lexiconLegendGray;

    private bool isOpen = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        passportPanel.SetActive(false);
        passportIcon.onClick.AddListener(TogglePassport);
    }

    public void TogglePassport()
    {
        isOpen = !isOpen;
        if (isOpen)
            OpenPassport();
        else
            passportPanel.SetActive(false);
    }

    private void OpenPassport()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning("PassportManager: GameProgressManager.Instance es null, no se puede abrir el pasaporte.");
            return;
        }

        // GrammarWorld
        medalBuilders.sprite = GameProgressManager.Instance.HasBuildersMedal() ?
            buildersColor : buildersGray;
        medalVerbMaster.sprite = GameProgressManager.Instance.HasVerbMaster() ?
            verbMasterColor : verbMasterGray;
        medalPathfinder.sprite = GameProgressManager.Instance.HasPathfinder() ?
            pathfinderColor : pathfinderGray;
        medalSentenceBuilder.sprite = GameProgressManager.Instance.HasSentenceBuilder() ?
            sentenceBuilderColor : sentenceBuilderGray;

        // VocabWorld+
        medalDailyLifeScout.sprite = GameProgressManager.Instance.HasDailyLifeScoutMedal() ?
            dailyLifeScoutColor : dailyLifeScoutGray;
        medalNumberCruncher.sprite = GameProgressManager.Instance.HasNumberCruncher() ?
            numberCruncherColor : numberCruncherGray;
        medalGlobalCitizen.sprite = GameProgressManager.Instance.HasGlobalCitizen() ?
            globalCitizenColor : globalCitizenGray;
        medalLexiconLegend.sprite = GameProgressManager.Instance.HasLexiconLegend() ?
            lexiconLegendColor : lexiconLegendGray;

        passportPanel.SetActive(true);
    }

    public void RefreshPassport()
    {
        if (isOpen) OpenPassport();
    }
}
using UnityEngine;
using UnityEngine.UI;
using SpatialSys.UnitySDK;

public class PassportManager : MonoBehaviour
{
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
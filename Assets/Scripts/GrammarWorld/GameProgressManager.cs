using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    [Header("GrammarWorld")]
    public GameObject bloqueadorInsignia;

    [Header("Medallas VocabWorld+")]
    public VocabMedallaTrigger medalla4;
    public VocabMedallaTrigger medalla5;
    public VocabMedallaTrigger medalla6;
    public LexiconLegendTrigger insigniaFinal2;
    public GameObject bloqueadorInsigniaVocab;

    private bool hasBuildersMedal        = false;
    private bool hasVerbMaster           = false;
    private bool hasPathfinder           = false;
    private bool hasSentenceBuilder      = false;
    private bool hasDailyLifeScoutMedal  = false;
    private bool hasNumberCruncher       = false;
    private bool hasGlobalCitizen        = false;
    private bool hasLexiconLegend        = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadProgress();
    }

    private void LoadProgress()
    {
        hasBuildersMedal       = PlayerPrefs.GetInt("hasBuildersMedal", 0) == 1;
        hasVerbMaster          = PlayerPrefs.GetInt("hasVerbMaster", 0) == 1;
        hasPathfinder          = PlayerPrefs.GetInt("hasPathfinder", 0) == 1;
        hasSentenceBuilder     = PlayerPrefs.GetInt("hasSentenceBuilder", 0) == 1;
        hasDailyLifeScoutMedal = PlayerPrefs.GetInt("hasDailyLifeScoutMedal", 0) == 1;
        hasNumberCruncher      = PlayerPrefs.GetInt("hasNumberCruncher", 0) == 1;
        hasGlobalCitizen       = PlayerPrefs.GetInt("hasGlobalCitizen", 0) == 1;
        hasLexiconLegend       = PlayerPrefs.GetInt("hasLexiconLegend", 0) == 1;
    }

    private void SaveProgress(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ==================
    // GRAMMARWORLD
    // ==================

    public void AwardBuildersMedal()
    {
        if (hasBuildersMedal) return;
        hasBuildersMedal = true;
        SaveProgress("hasBuildersMedal", true);
        CheckSentenceBuilderBadge();
    }

    public void AwardVerbMasterMedal()
    {
        if (hasVerbMaster) return;
        hasVerbMaster = true;
        SaveProgress("hasVerbMaster", true);
        CheckSentenceBuilderBadge();
    }

    public void AwardPathfinderMedal()
    {
        if (hasPathfinder) return;
        hasPathfinder = true;
        SaveProgress("hasPathfinder", true);
        CheckSentenceBuilderBadge();
    }

    private void CheckSentenceBuilderBadge()
    {
        if (hasBuildersMedal && hasVerbMaster && hasPathfinder && !hasSentenceBuilder)
        {
            if (bloqueadorInsignia != null)
                bloqueadorInsignia.SetActive(false);
            Debug.Log("The Sentence Builder Badge is waiting for you!");
        }
    }

    public void AwardSentenceBuilderBadge()
    {
        if (hasSentenceBuilder) return;
        hasSentenceBuilder = true;
        SaveProgress("hasSentenceBuilder", true);
        Debug.Log("You completed GrammarWorld! The Sentence Builder Badge is yours!");
    }

    // ==================
    // VOCABWORLD+
    // ==================

    public void AwardDailyLifeScoutMedal()
    {
        if (hasDailyLifeScoutMedal) return;
        hasDailyLifeScoutMedal = true;
        SaveProgress("hasDailyLifeScoutMedal", true);
        if (medalla4 != null) medalla4.MostrarMedalla();
    }

    public void AwardNumberCruncherMedal()
    {
        if (hasNumberCruncher) return;
        hasNumberCruncher = true;
        SaveProgress("hasNumberCruncher", true);
        if (medalla5 != null) medalla5.MostrarMedalla();
    }

    public void AwardGlobalCitizenMedal()
    {
        if (hasGlobalCitizen) return;
        hasGlobalCitizen = true;
        SaveProgress("hasGlobalCitizen", true);
        if (medalla6 != null) medalla6.MostrarMedalla();
    }

    public void OnMedalla4Collected()
    {
        Debug.Log("Daily Life Scout Medal earned!");
        CheckLexiconLegendBadge();
    }

    public void OnMedalla5Collected()
    {
        Debug.Log("Number Cruncher Medal earned!");
        CheckLexiconLegendBadge();
    }

    public void OnMedalla6Collected()
    {
        Debug.Log("Global Citizen Medal earned!");
        CheckLexiconLegendBadge();
    }

    public void AwardLexiconLegendBadge()
    {
        if (hasLexiconLegend) return;
        hasLexiconLegend = true;
        SaveProgress("hasLexiconLegend", true);
        Debug.Log("You completed VocabWorld+! The Lexicon Legend Badge is yours!");
    }

    private void CheckLexiconLegendBadge()
    {
        if (hasDailyLifeScoutMedal && hasNumberCruncher && hasGlobalCitizen && !hasLexiconLegend)
        {
            if (bloqueadorInsigniaVocab != null)
                bloqueadorInsigniaVocab.SetActive(false);
            if (insigniaFinal2 != null)
                insigniaFinal2.MostrarInsignia();
            Debug.Log("The Lexicon Legend Badge is waiting for you!");
        }
    }

    // ==================
    // GETTERS
    // ==================

    public bool HasBuildersMedal()         => hasBuildersMedal;
    public bool HasVerbMaster()            => hasVerbMaster;
    public bool HasPathfinder()            => hasPathfinder;
    public bool HasSentenceBuilder()       => hasSentenceBuilder;
    public bool HasDailyLifeScoutMedal()   => hasDailyLifeScoutMedal;
    public bool HasNumberCruncher()        => hasNumberCruncher;
    public bool HasGlobalCitizen()         => hasGlobalCitizen;
    public bool HasLexiconLegend()         => hasLexiconLegend;

    public void ResetAllProgress()
    {
        hasBuildersMedal       = false;
        hasVerbMaster          = false;
        hasPathfinder          = false;
        hasSentenceBuilder     = false;
        hasDailyLifeScoutMedal = false;
        hasNumberCruncher      = false;
        hasGlobalCitizen       = false;
        hasLexiconLegend       = false;

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All progress reset!");
    }
}
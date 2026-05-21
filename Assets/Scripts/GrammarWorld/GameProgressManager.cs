using UnityEngine;
using SpatialSys.UnitySDK;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    // GrammarWorld
    public GameObject bloqueadorInsignia;
    private bool hasBuildersMedal   = false;
    private bool hasVerbMaster      = false;
    private bool hasPathfinder      = false;
    private bool hasSentenceBuilder = false;

    // VocabWorld+
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
        SpatialBridge.userWorldDataStoreService.GetVariable("hasBuildersMedal", false)
            .SetCompletedEvent((response) => {
                hasBuildersMedal = (bool)response.value;
            });

        SpatialBridge.userWorldDataStoreService.GetVariable("hasVerbMaster", false)
            .SetCompletedEvent((response) => {
                hasVerbMaster = (bool)response.value;
            });

        SpatialBridge.userWorldDataStoreService.GetVariable("hasPathfinder", false)
            .SetCompletedEvent((response) => {
                hasPathfinder = (bool)response.value;
            });

        SpatialBridge.userWorldDataStoreService.GetVariable("hasSentenceBuilder", false)
            .SetCompletedEvent((response) => {
                hasSentenceBuilder = (bool)response.value;
            });

        SpatialBridge.userWorldDataStoreService.GetVariable("hasDailyLifeScoutMedal", false)
            .SetCompletedEvent((response) => {
                hasDailyLifeScoutMedal = (bool)response.value;
            });

        SpatialBridge.userWorldDataStoreService.GetVariable("hasNumberCruncher", false)
            .SetCompletedEvent((response) => {
                hasNumberCruncher = (bool)response.value;
            });

        SpatialBridge.userWorldDataStoreService.GetVariable("hasGlobalCitizen", false)
            .SetCompletedEvent((response) => {
                hasGlobalCitizen = (bool)response.value;
            });

        SpatialBridge.userWorldDataStoreService.GetVariable("hasLexiconLegend", false)
            .SetCompletedEvent((response) => {
                hasLexiconLegend = (bool)response.value;
            });
    }

    // ==================
    // GRAMMARWORLD
    // ==================

    public void AwardBuildersMedal()
    {
        if (hasBuildersMedal) return;
        hasBuildersMedal = true;
        SpatialBridge.userWorldDataStoreService.SetVariable("hasBuildersMedal", true);
        SpatialBridge.questService.quests[1].GetTaskByID(1).Complete();
        SpatialBridge.questService.quests[2].Start();
        CheckSentenceBuilderBadge();
    }

    public void AwardVerbMasterMedal()
    {
        if (hasVerbMaster) return;
        hasVerbMaster = true;
        SpatialBridge.userWorldDataStoreService.SetVariable("hasVerbMaster", true);
        SpatialBridge.questService.quests[2].GetTaskByID(1).Complete();
        SpatialBridge.questService.quests[3].Start();
        CheckSentenceBuilderBadge();
    }

    public void AwardPathfinderMedal()
    {
        if (hasPathfinder) return;
        hasPathfinder = true;
        SpatialBridge.userWorldDataStoreService.SetVariable("hasPathfinder", true);
        SpatialBridge.questService.quests[3].GetTaskByID(1).Complete();
        CheckSentenceBuilderBadge();
    }
    public void AwardLexiconLegendBadge()
    {
    CheckLexiconLegendBadge();
    }

    private void CheckSentenceBuilderBadge()
    {
    if (hasBuildersMedal && hasVerbMaster && hasPathfinder && !hasSentenceBuilder)
    {
        if (bloqueadorInsignia != null)
            bloqueadorInsignia.SetActive(false);
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "The Sentence Builder Badge is waiting for you!");
        SpatialBridge.questService.quests[4].Start();
    }
    }

    public void AwardSentenceBuilderBadge()
    {
    if (hasSentenceBuilder) return;
    hasSentenceBuilder = true;
    SpatialBridge.userWorldDataStoreService.SetVariable("hasSentenceBuilder", true);
    SpatialBridge.questService.quests[4].GetTaskByID(1).Complete();
    SpatialBridge.coreGUIService.DisplayToastMessage(
        "You completed GrammarWorld! The Sentence Builder Badge is yours!");
    }

    // ==================
    // VOCABWORLD+
    // ==================

    public void AwardDailyLifeScoutMedal()
    {
        if (hasDailyLifeScoutMedal) return;
        hasDailyLifeScoutMedal = true;
        SpatialBridge.userWorldDataStoreService.SetVariable("hasDailyLifeScoutMedal", true);
        SpatialBridge.questService.quests[5].GetTaskByID(1).Complete();
        CheckLexiconLegendBadge();
    }

    public void AwardNumberCruncherMedal()
    {
        if (hasNumberCruncher) return;
        hasNumberCruncher = true;
        SpatialBridge.userWorldDataStoreService.SetVariable("hasNumberCruncher", true);
        SpatialBridge.questService.quests[6].GetTaskByID(1).Complete();
        CheckLexiconLegendBadge();
    }

    public void AwardGlobalCitizenMedal()
    {
        if (hasGlobalCitizen) return;
        hasGlobalCitizen = true;
        SpatialBridge.userWorldDataStoreService.SetVariable("hasGlobalCitizen", true);
        SpatialBridge.questService.quests[7].GetTaskByID(1).Complete();
        CheckLexiconLegendBadge();
    }

    private void CheckLexiconLegendBadge()
    {
        if (hasDailyLifeScoutMedal && hasNumberCruncher && hasGlobalCitizen && !hasLexiconLegend)
        {
            hasLexiconLegend = true;
            SpatialBridge.userWorldDataStoreService.SetVariable("hasLexiconLegend", true);
            SpatialBridge.questService.quests[8].GetTaskByID(1).Complete();
            SpatialBridge.coreGUIService.DisplayToastMessage(
                "You completed VocabWorld+! The Lexicon Legend Badge is yours!");
        }
    }

    // ==================
    // GETTERS
    // ==================

    public bool HasBuildersMedal()          => hasBuildersMedal;
    public bool HasVerbMaster()             => hasVerbMaster;
    public bool HasPathfinder()             => hasPathfinder;
    public bool HasSentenceBuilder()        => hasSentenceBuilder;
    public bool HasDailyLifeScoutMedal()    => hasDailyLifeScoutMedal;
    public bool HasNumberCruncher()         => hasNumberCruncher;
    public bool HasGlobalCitizen()          => hasGlobalCitizen;
    public bool HasLexiconLegend()          => hasLexiconLegend;

    public void ResetAllProgress()
    {
        hasBuildersMedal        = false;
        hasVerbMaster           = false;
        hasPathfinder           = false;
        hasSentenceBuilder      = false;
        hasDailyLifeScoutMedal  = false;
        hasNumberCruncher       = false;
        hasGlobalCitizen        = false;
        hasLexiconLegend        = false;

        SpatialBridge.userWorldDataStoreService.SetVariable("hasBuildersMedal", false);
        SpatialBridge.userWorldDataStoreService.SetVariable("hasVerbMaster", false);
        SpatialBridge.userWorldDataStoreService.SetVariable("hasPathfinder", false);
        SpatialBridge.userWorldDataStoreService.SetVariable("hasSentenceBuilder", false);
        SpatialBridge.userWorldDataStoreService.SetVariable("hasDailyLifeScoutMedal", false);
        SpatialBridge.userWorldDataStoreService.SetVariable("hasNumberCruncher", false);
        SpatialBridge.userWorldDataStoreService.SetVariable("hasGlobalCitizen", false);
        SpatialBridge.userWorldDataStoreService.SetVariable("hasLexiconLegend", false);

        SpatialBridge.coreGUIService.DisplayToastMessage("All progress reset!");
    }
}
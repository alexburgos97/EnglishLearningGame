using UnityEngine;
using SpatialSys.UnitySDK;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    private bool hasBuildersMedal   = false;
    private bool hasVerbMaster      = false;
    private bool hasPathfinder      = false;
    private bool hasSentenceBuilder = false;

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
    }

    public void AwardBuildersMedal()
    {
    if (hasBuildersMedal) return;
    hasBuildersMedal = true;
    SpatialBridge.userWorldDataStoreService.SetVariable("hasBuildersMedal", true);
    SpatialBridge.questService.quests[1].GetTaskByID(1).Complete();
    
    // Activar Quest del volcán
    SpatialBridge.questService.quests[2].Start();
    
    CheckSentenceBuilderBadge();
    }

    public void AwardVerbMasterMedal()
    {
    if (hasVerbMaster) return;
    hasVerbMaster = true;
    SpatialBridge.userWorldDataStoreService.SetVariable("hasVerbMaster", true);
    SpatialBridge.questService.quests[2].GetTaskByID(1).Complete();
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

    private void CheckSentenceBuilderBadge()
    {
        if (hasBuildersMedal && hasVerbMaster && hasPathfinder && !hasSentenceBuilder)
        {
            hasSentenceBuilder = true;
            SpatialBridge.userWorldDataStoreService.SetVariable("hasSentenceBuilder", true);
            SpatialBridge.questService.quests[4].GetTaskByID(1).Complete();
            SpatialBridge.coreGUIService.DisplayToastMessage(
                "You completed GrammarWorld! The Sentence Builder Badge is yours!");
        }
    }

    public bool HasBuildersMedal()   => hasBuildersMedal;
    public bool HasVerbMaster()      => hasVerbMaster;
    public bool HasPathfinder()      => hasPathfinder;
    public bool HasSentenceBuilder() => hasSentenceBuilder;
}
using UnityEngine;
using UnityEngine.UI;
using SpatialSys.UnitySDK;

public class PassportManager : MonoBehaviour
{
    public static PassportManager Instance { get; private set; }

    [Header("Paneles")]
    public GameObject passportPanel;
    public Button passportIcon;

    [Header("Medallas - Imágenes en gris y color")]
    public Image medalBuilders;
    public Image medalVerbMaster;
    public Image medalPathfinder;
    public Image medalSentenceBuilder;

    [Header("Sprites en color")]
    public Sprite buildersColor;
    public Sprite verbMasterColor;
    public Sprite pathfinderColor;
    public Sprite sentenceBuilderColor;

    [Header("Sprites en gris")]
    public Sprite buildersGray;
    public Sprite verbMasterGray;
    public Sprite pathfinderGray;
    public Sprite sentenceBuilderGray;

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
        // Actualizar medallas según progreso
        medalBuilders.sprite = GameProgressManager.Instance.HasBuildersMedal() ?
            buildersColor : buildersGray;
        medalVerbMaster.sprite = GameProgressManager.Instance.HasVerbMaster() ?
            verbMasterColor : verbMasterGray;
        medalPathfinder.sprite = GameProgressManager.Instance.HasPathfinder() ?
            pathfinderColor : pathfinderGray;
        medalSentenceBuilder.sprite = GameProgressManager.Instance.HasSentenceBuilder() ?
            sentenceBuilderColor : sentenceBuilderGray;

        passportPanel.SetActive(true);
    }

    public void RefreshPassport()
    {
        if (isOpen) OpenPassport();
    }
}
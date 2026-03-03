using SpatialSys.UnitySDK;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MazeUIManager : MonoBehaviour
{
    public static MazeUIManager Instance { get; private set; }

    [Header("Panel de instrucciones")]
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;
    public Button replayButton;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        instructionPanel.SetActive(false);
        replayButton.onClick.AddListener(ReplayAudio);
    }

    public void ShowInstruction(string text)
    {
    instructionText.text = text;
    instructionPanel.SetActive(true);
    }   

    public void HidePanel()
    {
        instructionPanel.SetActive(false);
    }

    private void ReplayAudio()
    {
        MazeManager.Instance.ReplayCurrentInstruction();
    }
}
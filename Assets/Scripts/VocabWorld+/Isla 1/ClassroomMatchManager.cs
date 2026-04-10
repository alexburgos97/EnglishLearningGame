using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class ClassroomMatchManager : MonoBehaviour
{
    public static ClassroomMatchManager Instance { get; private set; }

    [Header("Panel principal")]
    public GameObject classroomPanel;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    private int matchedCount = 0;
    private int totalWords = 3;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        classroomPanel.SetActive(false);
    }

    public void ShowPanel()
    {
        classroomPanel.SetActive(true);
    }

    public void OnWordMatched()
    {
        matchedCount++;
        if (matchedCount >= totalWords)
            Invoke(nameof(OnComplete), 1.5f);
    }

    public void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            Invoke(nameof(ClearFeedback), 2f);
        }
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }

    private void OnComplete()
    {
        classroomPanel.SetActive(false);
        Isla1Manager.Instance.OnZonaEscolarComplete();
    }
}
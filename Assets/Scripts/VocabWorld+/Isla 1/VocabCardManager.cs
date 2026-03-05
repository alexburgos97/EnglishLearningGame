using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VocabCardManager : MonoBehaviour
{
    public static VocabCardManager Instance { get; private set; }

    [Header("Panel Vocab Card")]
    public GameObject vocabCardPanel;

    [Header("Slots Comida")]
    public TextMeshProUGUI foodSlot1;
    public TextMeshProUGUI foodSlot2;
    public TextMeshProUGUI foodSlot3;

    [Header("Slots Ropa")]
    public TextMeshProUGUI clothesSlot1;
    public TextMeshProUGUI clothesSlot2;
    public TextMeshProUGUI clothesSlot3;

    [Header("Slots Escolar")]
    public TextMeshProUGUI schoolSlot1;
    public TextMeshProUGUI schoolSlot2;
    public TextMeshProUGUI schoolSlot3;

    private int foodCount = 0;
    private int clothesCount = 0;
    private int schoolCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        vocabCardPanel.SetActive(false);
    }

    public void ShowVocabCard()
    {
        vocabCardPanel.SetActive(true);
    }

    public void HideVocabCard()
    {
        vocabCardPanel.SetActive(false);
    }

    public void AddFoodWord(string word)
    {
        if (foodCount == 0) foodSlot1.text = word;
        else if (foodCount == 1) foodSlot2.text = word;
        else if (foodCount == 2) foodSlot3.text = word;
        foodCount++;
        CheckCompletion();
    }

    public void AddClothesWord(string word)
    {
        if (clothesCount == 0) clothesSlot1.text = word;
        else if (clothesCount == 1) clothesSlot2.text = word;
        else if (clothesCount == 2) clothesSlot3.text = word;
        clothesCount++;
        CheckCompletion();
    }

    public void AddSchoolWord(string word)
    {
        if (schoolCount == 0) schoolSlot1.text = word;
        else if (schoolCount == 1) schoolSlot2.text = word;
        else if (schoolCount == 2) schoolSlot3.text = word;
        schoolCount++;
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (foodCount >= 3 && clothesCount >= 3 && schoolCount >= 3)
        {
            Isla1Manager.Instance.OnVocabCardComplete();
        }
    }
    public int GetFoodCount() => foodCount;
    public int GetClothesCount() => clothesCount;
    public int GetSchoolCount() => schoolCount;
}
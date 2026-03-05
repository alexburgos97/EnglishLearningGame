using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class ClassroomMatchManager : MonoBehaviour
{
    public static ClassroomMatchManager Instance { get; private set; }

    [Header("Referencias")]
    public AudioSource audioSource;

    [Header("Panel principal")]
    public GameObject classroomPanel;
    public TextMeshProUGUI feedbackText;

    [Header("Botones de palabras")]
    public Button wordButton1;
    public Button wordButton2;
    public Button wordButton3;
    public TextMeshProUGUI wordButtonText1;
    public TextMeshProUGUI wordButtonText2;
    public TextMeshProUGUI wordButtonText3;

    [Header("Slots de objetos")]
    public Button objectSlot1;
    public Button objectSlot2;
    public Button objectSlot3;
    public TextMeshProUGUI objectSlotText1;
    public TextMeshProUGUI objectSlotText2;
    public TextMeshProUGUI objectSlotText3;

    [Header("Audios")]
    public AudioClip audioBook;
    public AudioClip audioRuler;
    public AudioClip audioBackpack;

    private string[] words = { "book", "ruler", "backpack" };
    private AudioClip[] wordAudios;
    private string selectedWord = "";
    private int matchedCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        classroomPanel.SetActive(false);
        feedbackText.text = "";

        wordAudios = new AudioClip[] { audioBook, audioRuler, audioBackpack };

        // Configurar botones de palabras
        wordButton1.onClick.AddListener(() => SelectWord("book", audioBook));
        wordButton2.onClick.AddListener(() => SelectWord("ruler", audioRuler));
        wordButton3.onClick.AddListener(() => SelectWord("backpack", audioBackpack));

        // Configurar slots de objetos
        objectSlot1.onClick.AddListener(() => TryMatch("book"));
        objectSlot2.onClick.AddListener(() => TryMatch("ruler"));
        objectSlot3.onClick.AddListener(() => TryMatch("backpack"));

        objectSlotText1.text = "?";
        objectSlotText2.text = "?";
        objectSlotText3.text = "?";
    }

    public void ShowPanel()
    {
        wordButtonText1.text = "book";
        wordButtonText2.text = "ruler";
        wordButtonText3.text = "backpack";
        classroomPanel.SetActive(true);
    }

    private void SelectWord(string word, AudioClip audio)
    {
        selectedWord = word;
        feedbackText.text = "Now tap the correct object!";
        feedbackText.color = Color.white;

        if (audio != null)
        {
            audioSource.clip = audio;
            audioSource.Play();
        }
    }

    private void TryMatch(string objectName)
    {
        if (selectedWord == "") 
        {
            feedbackText.text = "Select a word first!";
            feedbackText.color = Color.yellow;
            return;
        }

        if (selectedWord == objectName)
        {
            feedbackText.text = "Correct! " + objectName.ToUpper();
            feedbackText.color = Color.green;

            // Desactivar botón de palabra usado
            DisableWordButton(objectName);

            // Mostrar palabra en slot
            ShowWordInSlot(objectName);

            VocabCardManager.Instance.AddSchoolWord(objectName);
            matchedCount++;
            selectedWord = "";

            if (matchedCount >= 3)
                Invoke(nameof(OnComplete), 1.5f);
        }
        else
        {
            feedbackText.text = "Try again!";
            feedbackText.color = Color.red;
            selectedWord = "";
        }
    }

    private void DisableWordButton(string word)
    {
        if (wordButtonText1.text == word) wordButton1.interactable = false;
        else if (wordButtonText2.text == word) wordButton2.interactable = false;
        else if (wordButtonText3.text == word) wordButton3.interactable = false;
    }

    private void ShowWordInSlot(string word)
    {
        if (word == "book") objectSlotText1.text = "book";
        else if (word == "ruler") objectSlotText2.text = "ruler";
        else if (word == "backpack") objectSlotText3.text = "backpack";
    }

    private void OnComplete()
    {
        classroomPanel.SetActive(false);
        Isla1Manager.Instance.OnZonaEscolarComplete();
    }
}
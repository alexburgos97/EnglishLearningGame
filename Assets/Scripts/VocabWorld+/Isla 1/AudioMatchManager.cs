using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;
using System.Collections.Generic;

public class AudioMatchManager : MonoBehaviour
{
    public static AudioMatchManager Instance { get; private set; }

    [Header("Referencias")]
    public AudioSource audioSource;

    [Header("Panel de preguntas")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;
    public TextMeshProUGUI option3Text;
    public TextMeshProUGUI feedbackText;
    public Button replayButton;

    private string currentWord = "";
    private AudioClip currentAudio;
    private List<string> allWords = new List<string>();
    private int clothesCompleted = 0;
    private int totalClothesWords = 3;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        questionPanel.SetActive(false);
        feedbackText.text = "";

        option1Button.onClick.AddListener(() => CheckAnswer(option1Text.text));
        option2Button.onClick.AddListener(() => CheckAnswer(option2Text.text));
        option3Button.onClick.AddListener(() => CheckAnswer(option3Text.text));
        replayButton.onClick.AddListener(ReplayAudio);
    }

    public void RegisterWord(string word)
    {
        if (!allWords.Contains(word))
            allWords.Add(word);
        totalClothesWords = allWords.Count;
    }

    public void ShowQuestionForWord(string word, AudioClip audio)
    {
        currentWord = word;
        currentAudio = audio;
        feedbackText.text = "";

        // Reproducir audio
        audioSource.clip = audio;
        audioSource.Play();

        // Generar opciones
        List<string> options = GenerateOptions(word);
        option1Text.text = options[0];
        option2Text.text = options[1];
        option3Text.text = options[2];

        option1Button.interactable = true;
        option2Button.interactable = true;
        option3Button.interactable = true;

        questionText.text = "What did you hear?";
        questionPanel.SetActive(true);
    }

    private List<string> GenerateOptions(string correctWord)
    {
        List<string> options = new List<string>();
        options.Add(correctWord);

        List<string> others = new List<string>(allWords);
        others.Remove(correctWord);

        // Mezclar y tomar 2 incorrectas
        for (int i = others.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = others[i];
            others[i] = others[j];
            others[j] = temp;
        }

        for (int i = 0; i < Mathf.Min(2, others.Count); i++)
            options.Add(others[i]);

        // Mezclar opciones finales
        for (int i = options.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = options[i];
            options[i] = options[j];
            options[j] = temp;
        }

        return options;
    }

    private void CheckAnswer(string selected)
    {
    option1Button.interactable = false;
    option2Button.interactable = false;
    option3Button.interactable = false;

    if (selected == currentWord)
        {
        feedbackText.text = "Correct! " + currentWord.ToUpper();
        feedbackText.color = Color.green;
        VocabCardManager.Instance.AddClothesWord(currentWord);
        clothesCompleted++;

        // Marcar el objeto como completado
        ClothesObject[] objects = FindObjectsOfType<ClothesObject>();
        foreach (ClothesObject obj in objects)
        {
            if (obj.wordInEnglish == currentWord)
                obj.MarkAsCompleted();
        }

        Invoke(nameof(NextQuestion), 2f);
        }
    else
        {
        feedbackText.text = "Try again!";
        feedbackText.color = Color.red;
        Invoke(nameof(ReactivateButtons), 1.5f);
        }
    }

    private void ReactivateButtons()
    {
        option1Button.interactable = true;
        option2Button.interactable = true;
        option3Button.interactable = true;
        feedbackText.text = "";
        ReplayAudio();
    }

    private void NextQuestion()
    {
        questionPanel.SetActive(false);
        feedbackText.text = "";

        if (clothesCompleted >= totalClothesWords)
            Isla1Manager.Instance.OnZonaRopaComplete();
    }

    private void ReplayAudio()
    {
        if (audioSource.clip != null)
            audioSource.Play();
    }
}
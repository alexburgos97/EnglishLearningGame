using UnityEngine;
using TMPro;
using SpatialSys.UnitySDK;

public class VocabObject : MonoBehaviour
{
    [Header("Configuración")]
    public string wordInEnglish;
    public AudioClip wordAudio;
    public string category; // "food", "clothes", "school"

    [Header("UI")]
    public GameObject wordLabel;
    public TextMeshProUGUI wordText;

    private AudioSource audioSource;
    private bool wordAdded = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        if (wordLabel != null)
            wordLabel.SetActive(false);
    }

    public void OnPlayerNear()
    {
        if (wordLabel != null)
        {
            wordLabel.SetActive(true);
            wordText.text = wordInEnglish;
        }

        if (wordAudio != null)
        {
            audioSource.clip = wordAudio;
            audioSource.Play();
        }
    }

    public void OnPlayerClick()
    {
        if (wordAdded) return;
        wordAdded = true;

        if (wordAudio != null)
        {
            audioSource.clip = wordAudio;
            audioSource.Play();
        }

        if (category == "food")
            VocabCardManager.Instance.AddFoodWord(wordInEnglish);
        else if (category == "clothes")
            VocabCardManager.Instance.AddClothesWord(wordInEnglish);
        else if (category == "school")
            VocabCardManager.Instance.AddSchoolWord(wordInEnglish);

        SpatialBridge.coreGUIService.DisplayToastMessage(
            wordInEnglish + " added to your Vocab Card!");

        Isla1Manager.Instance.OnFoodWordComplete();
    }

    public void OnPlayerLeave()
    {
        if (wordLabel != null)
            wordLabel.SetActive(false);
    }
}
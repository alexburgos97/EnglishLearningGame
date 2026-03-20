using UnityEngine;

public class ClothesObject : MonoBehaviour
{
    public string wordInEnglish;
    public AudioClip wordAudio;
    private bool wordAdded = false;

    void Start()
    {
        AudioMatchManager.Instance.RegisterWord(wordInEnglish);
    }

    public void OnPlayerNear()
    {
        if (wordAdded) return;
        AudioMatchManager.Instance.ShowQuestionForWord(wordInEnglish, wordAudio);
    }

    public void MarkAsCompleted()
    {
        wordAdded = true;
    }
}
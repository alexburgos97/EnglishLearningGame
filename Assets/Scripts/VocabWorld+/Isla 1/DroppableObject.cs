using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class DroppableObject : MonoBehaviour, IDropHandler
{
    public string correctWord;
    public AudioClip correctAudio;
    public int slotIndex;

    private bool isCompleted = false;
    private Image objectImage;

    void Start()
    {
        objectImage = GetComponent<Image>();
    }

    public void SetCorrectWord(string word)
    {
        correctWord = word;
    }

    public void ResetObject()
    {
        isCompleted = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
    if (isCompleted) return;

    DraggableWord draggable = eventData.pointerDrag.GetComponent<DraggableWord>();
    if (draggable == null) return;

    if (draggable.wordInEnglish == correctWord)
    {
        isCompleted = true;
        draggable.MarkAsCompleted();

        StartCoroutine(GlowEffect());

        if (correctAudio != null)
        {
            AudioSource audio = GetComponent<AudioSource>();
            if (audio == null) audio = gameObject.AddComponent<AudioSource>();
            audio.spatialBlend = 0f;
            audio.clip = correctAudio;
            audio.Play();
        }

        ClassroomMatchManager.Instance.ShowFeedback(
            correctWord + " correct!", Color.green);
        ClassroomMatchManager.Instance.OnWordMatched(slotIndex);
        }
        else
        {
        ClassroomMatchManager.Instance.OnWordFailed();
        }
    }

    private IEnumerator GlowEffect()
    {
        if (objectImage == null) yield break;
        Color originalColor = objectImage.color;
        objectImage.color = Color.yellow;
        yield return new WaitForSeconds(0.3f);
        objectImage.color = originalColor;
        yield return new WaitForSeconds(0.3f);
        objectImage.color = Color.yellow;
        yield return new WaitForSeconds(0.3f);
        objectImage.color = originalColor;
    }
}
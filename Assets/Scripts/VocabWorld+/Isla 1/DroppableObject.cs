using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class DroppableObject : MonoBehaviour, IDropHandler
{
    public string correctWord;
    public AudioClip correctAudio;

    private bool isCompleted = false;
    private Image objectImage;

    void Start()
    {
        objectImage = GetComponent<Image>();
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

            VocabCardManager.Instance.AddSchoolWord(correctWord);
            ClassroomMatchManager.Instance.OnWordMatched();
            ClassroomMatchManager.Instance.ShowFeedback(
                correctWord.ToUpper() + " correct!", Color.green);
        }
        else
        {
            ClassroomMatchManager.Instance.ShowFeedback(
                "Try again!", Color.red);
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
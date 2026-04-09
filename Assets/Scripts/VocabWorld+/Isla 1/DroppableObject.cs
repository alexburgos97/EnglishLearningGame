using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DroppableObject : MonoBehaviour, IDropHandler
{
    public string correctWord;
    public AudioClip correctAudio;

    private bool isCompleted = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

            // Efecto de brillo
            StartCoroutine(GlowEffect());

            // Reproducir audio correcto
            if (correctAudio != null)
            {
                AudioSource audio = GetComponent<AudioSource>();
                if (audio == null) audio = gameObject.AddComponent<AudioSource>();
                audio.spatialBlend = 0f;
                audio.clip = correctAudio;
                audio.Play();
            }

            VocabCardManager.Instance.AddSchoolWord(correctWord);
            //ClassroomMatchManager.Instance.OnWordMatched();

            SpatialSys.UnitySDK.SpatialBridge.coreGUIService.DisplayToastMessage(
                correctWord.ToUpper() + " correct!");
        }
        else
        {
            SpatialSys.UnitySDK.SpatialBridge.coreGUIService.DisplayToastMessage(
                "Try again!");
        }
    }

    private IEnumerator GlowEffect()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = originalColor;
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableWord : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public string wordInEnglish;
    public AudioClip wordAudio;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private AudioSource audioSource;
    private Canvas canvas;
    private RectTransform canvasRect;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayAudio();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // No cambiamos nada del canvas ni del parent
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);
        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ReturnToOriginalPosition();
    }

    public void ReturnToOriginalPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    public void MarkAsCompleted()
    {
        gameObject.SetActive(false);
    }

    private void PlayAudio()
    {
        if (wordAudio != null)
        {
            audioSource.clip = wordAudio;
            audioSource.Play();
        }
    }
}
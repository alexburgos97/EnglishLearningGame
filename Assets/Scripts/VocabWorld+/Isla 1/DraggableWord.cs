using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableWord : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public string wordInEnglish;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private AudioSource audioSource;
    private Canvas canvas;
    private RectTransform canvasRect;
    private bool isDragging = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // Forzar actualización del layout antes de guardar posición
        Canvas.ForceUpdateCanvases();
        originalPosition = rectTransform.anchoredPosition;
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void OnPointerClick(PointerEventData eventData) { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        PlayDragAudio();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Si se detuvo y volvio a mover reproducir audio nuevamente
        if (!audioSource.isPlaying && isDragging)
            PlayDragAudio();

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
        isDragging = false;
        audioSource.Stop();
        ReturnToOriginalPosition();
    }

    private void PlayDragAudio()
    {
        if (ClassroomMatchManager.Instance != null &&
            ClassroomMatchManager.Instance.dragAudio != null)
        {
            audioSource.clip = ClassroomMatchManager.Instance.dragAudio;
            audioSource.Play();
        }
    }

    public void ReturnToOriginalPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    public void MarkAsCompleted()
    {
        isDragging = false;
        audioSource.Stop();
        gameObject.SetActive(false);
    }

    public void ResetWord()
    {
    isDragging = false;
    audioSource.Stop();
    gameObject.SetActive(true);
    rectTransform.anchoredPosition = originalPosition;
    UpdateText();
    }

    public void UpdateText()
    {
        TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = wordInEnglish;
    }
}
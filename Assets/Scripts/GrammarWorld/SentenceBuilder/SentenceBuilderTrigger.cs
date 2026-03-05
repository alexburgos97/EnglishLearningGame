using UnityEngine;
using SpatialSys.UnitySDK;

public class SentenceBuilderTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject insigniaSprite;
    public AudioClip audioFelicitaciones;

    private AudioSource audioSource;
    private Vector3 originalScale;
    private float pulseTimer = 0f;
    private bool isPulsing = true;
    private bool awarded = false;

    void Start()
    {
        originalScale = insigniaSprite.transform.localScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!isPulsing) return;

        pulseTimer += Time.deltaTime * 2f;
        float pulse = 1f + Mathf.Sin(pulseTimer) * 0.15f;
        insigniaSprite.transform.localScale = originalScale * pulse;
    }

    public void OnPlayerEnter()
    {
        if (awarded) return;
        awarded = true;
        isPulsing = false;

        insigniaSprite.transform.localScale = originalScale;

        if (audioFelicitaciones != null)
        {
            audioSource.clip = audioFelicitaciones;
            audioSource.Play();
        }

        GameProgressManager.Instance.AwardSentenceBuilderBadge();
    }
}
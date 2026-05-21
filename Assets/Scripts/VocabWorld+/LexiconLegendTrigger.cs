using UnityEngine;
using SpatialSys.UnitySDK;

public class LexiconLegendTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject bloqueadorInsignia;
    public AudioClip audioCelebracion;
    public GameObject insigniaSprite;

    private AudioSource audioSource;
    private bool awarded = false;
    private bool isPulsing = true;
    private Vector3 originalScale;
    private float pulseTimer = 0f;

    void Start()
    {
        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        // El bloqueador empieza activo
        if (bloqueadorInsignia != null)
            bloqueadorInsignia.SetActive(true);
    }

    void Update()
    {
        if (!isPulsing) return;

        pulseTimer += Time.deltaTime * 2f;
        float pulse = 1f + Mathf.Sin(pulseTimer) * 0.15f;
        transform.localScale = originalScale * pulse;
    }

    void OnTriggerEnter(Collider other)
    {
        if (awarded) return;
        awarded = true;
        isPulsing = false;

        transform.localScale = originalScale;

        if (audioCelebracion != null)
        {
            audioSource.clip = audioCelebracion;
            audioSource.Play();
        }

        GameProgressManager.Instance.AwardLexiconLegendBadge();
    }
}
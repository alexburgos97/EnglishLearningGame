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

        if (bloqueadorInsignia != null)
            bloqueadorInsignia.SetActive(true);

        // La insignia empieza desactivada
        if (insigniaSprite != null)
            insigniaSprite.SetActive(false);
    }

    void Update()
    {
        if (!isPulsing) return;

        pulseTimer += Time.deltaTime * 2f;
        float pulse = 1f + Mathf.Sin(pulseTimer) * 0.15f;
        transform.localScale = originalScale * pulse;
    }

    public void MostrarInsignia()
    {
        if (insigniaSprite != null)
            insigniaSprite.SetActive(true);
        isPulsing = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (awarded) return;
        if (insigniaSprite == null || !insigniaSprite.activeSelf) return;

        awarded = true;
        isPulsing = false;
        transform.localScale = originalScale;

        if (audioCelebracion != null)
        {
            audioSource.clip = audioCelebracion;
            audioSource.Play();
        }

        GameProgressManager.Instance.AwardLexiconLegendBadge();
        Invoke(nameof(DesactivarInsignia), 2f);
    }

    private void DesactivarInsignia()
    {
        if (insigniaSprite != null)
            insigniaSprite.SetActive(false);
    }
}
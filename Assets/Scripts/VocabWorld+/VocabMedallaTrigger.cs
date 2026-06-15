using UnityEngine;

public class VocabMedallaTrigger : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject medallaSprite;
    public AudioClip audioCelebracion;

    private AudioSource audioSource;
    private Vector3 originalScale;
    private float pulseTimer = 0f;
    private bool isPulsing = false;
    private bool awarded = false;

    void Start()
    {
        if (medallaSprite != null)
            medallaSprite.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        originalScale = transform.localScale;
    }

    void Update()
    {
        if (!isPulsing) return;
        pulseTimer += Time.deltaTime * 2f;
        float pulse = 1f + Mathf.Sin(pulseTimer) * 0.15f;
        transform.localScale = originalScale * pulse;
    }

    public void MostrarMedalla()
    {
        if (medallaSprite != null)
            medallaSprite.SetActive(true);
        isPulsing = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (awarded) return;
        if (medallaSprite == null || !medallaSprite.activeSelf) return;

        awarded = true;
        isPulsing = false;
        transform.localScale = originalScale;

        if (audioCelebracion != null)
        {
            audioSource.clip = audioCelebracion;
            audioSource.Play();
        }

        Invoke(nameof(DesactivarMedalla), 2f);
    }

    private void DesactivarMedalla()
    {
        if (medallaSprite != null)
            medallaSprite.SetActive(false);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialSys.UnitySDK;

public class MathDialog : MonoBehaviour
{
    [Header("Personajes")]
    public Transform aventurero1;
    public Transform aventurera1;

    [Header("Audios - Orden: A1, B1, A2, B2...")]
    public AudioClip[] dialogAudios;

    [Header("UI")]
    public GameObject dialogPanel;
    public Button playButton;

    [Header("Configuracion")]
    public float spatialBlend = 1f;
    public float maxDistance = 15f;

    private AudioSource audioSourceA;
    private AudioSource audioSourceB;
    private int currentAudioIndex = 0;
    private bool isPlaying = false;
    private Vector3 originalScaleA;
    private Vector3 originalScaleB;
    private float pulseTimer = 0f;

    void Start()
    {
        // Crear AudioSource espacial en Aventurero1
        audioSourceA = aventurero1.GetComponent<AudioSource>();
        if (audioSourceA == null)
            audioSourceA = aventurero1.gameObject.AddComponent<AudioSource>();
        audioSourceA.spatialBlend = spatialBlend;
        audioSourceA.maxDistance = maxDistance;
        audioSourceA.rolloffMode = AudioRolloffMode.Linear;
        audioSourceA.playOnAwake = false;

        // Crear AudioSource espacial en Aventurera1
        audioSourceB = aventurera1.GetComponent<AudioSource>();
        if (audioSourceB == null)
            audioSourceB = aventurera1.gameObject.AddComponent<AudioSource>();
        audioSourceB.spatialBlend = spatialBlend;
        audioSourceB.maxDistance = maxDistance;
        audioSourceB.rolloffMode = AudioRolloffMode.Linear;
        audioSourceB.playOnAwake = false;

        originalScaleA = aventurero1.localScale;
        originalScaleB = aventurera1.localScale;

        dialogPanel.SetActive(true);
        playButton.onClick.AddListener(StartDialog);
    }

    void Update()
    {
        if (!isPlaying) return;

        // Determinar cual personaje habla
        bool isPersonajeA = currentAudioIndex % 2 == 0;
        AudioSource currentSource = isPersonajeA ? audioSourceA : audioSourceB;

        // Efecto de pulso en el personaje que habla
        pulseTimer += Time.deltaTime * 5f;
        float pulse = 1f + Mathf.Sin(pulseTimer) * 0.1f;

        if (isPersonajeA)
        {
            aventurero1.localScale = originalScaleA * pulse;
            aventurera1.localScale = originalScaleB;
        }
        else
        {
            aventurera1.localScale = originalScaleB * pulse;
            aventurero1.localScale = originalScaleA;
        }

        // Verificar si termino el audio actual
        if (!currentSource.isPlaying)
        {
            // Resetear escala
            aventurero1.localScale = originalScaleA;
            aventurera1.localScale = originalScaleB;

            currentAudioIndex++;

            if (currentAudioIndex >= dialogAudios.Length)
            {
                // Conversacion terminada
                OnDialogComplete();
            }
            else
            {
                // Reproducir siguiente audio
                PlayCurrentAudio();
            }
        }
    }

    public void StartDialog()
    {
        if (isPlaying) return;
        currentAudioIndex = 0;
        isPlaying = true;
        dialogPanel.SetActive(false);
        pulseTimer = 0f;
        PlayCurrentAudio();
    }

    private void PlayCurrentAudio()
    {
        if (currentAudioIndex >= dialogAudios.Length) return;
        if (dialogAudios[currentAudioIndex] == null)
        {
            currentAudioIndex++;
            if (currentAudioIndex < dialogAudios.Length)
                PlayCurrentAudio();
            return;
        }

        bool isPersonajeA = currentAudioIndex % 2 == 0;
        AudioSource currentSource = isPersonajeA ? audioSourceA : audioSourceB;

        currentSource.clip = dialogAudios[currentAudioIndex];
        currentSource.Play();
        pulseTimer = 0f;
    }

    private void OnDialogComplete()
    {
        isPlaying = false;
        aventurero1.localScale = originalScaleA;
        aventurera1.localScale = originalScaleB;
        dialogPanel.SetActive(true);

        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Now complete the equations!");
    }
}
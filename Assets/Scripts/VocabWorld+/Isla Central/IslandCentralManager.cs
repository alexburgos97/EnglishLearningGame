using UnityEngine;
using SpatialSys.UnitySDK;

public class IslandCentralManager : MonoBehaviour
{
    public static IslandCentralManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject syntaxSprite;
    public AudioSource audioSource;
    public GameObject puente_Isla1;

    [Header("Audios")]
    public AudioClip audioBienvenida;
    public AudioClip audioInstruccion;

    private bool isTalking = false;
    private Vector3 originalScale;
    private float pulseTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        puente_Isla1.SetActive(false);
        originalScale = syntaxSprite.transform.localScale;
        audioSource = syntaxSprite.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = syntaxSprite.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!isTalking) return;

        pulseTimer += Time.deltaTime * 5f;
        float pulse = 1f + Mathf.Sin(pulseTimer) * 0.1f;
        syntaxSprite.transform.localScale = originalScale * pulse;

        if (!audioSource.isPlaying)
        {
            isTalking = false;
            syntaxSprite.transform.localScale = originalScale;
        }
    }

    // Llamado por Trigger_Inicio_Isla_Central
    public void OnPlayerEnter()
    {
    SpatialBridge.questService.quests[5].Start();
    PlayAudio(audioBienvenida);
    }

    // Llamado por el jugador al tocar el mapa o un trigger
    public void ActivateIsland1()
    {
        PlayAudio(audioInstruccion);
        puente_Isla1.SetActive(true);
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Great! The bridge is open. Cross to explore Food Island!");
    }

    private void PlayAudio(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.clip = clip;
        audioSource.Play();
        isTalking = true;
        pulseTimer = 0f;
    }
}
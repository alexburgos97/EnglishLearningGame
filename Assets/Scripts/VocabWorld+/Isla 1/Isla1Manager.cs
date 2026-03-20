using UnityEngine;
using SpatialSys.UnitySDK;

public class Isla1Manager : MonoBehaviour
{
    public static Isla1Manager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject syntaxSprite;
    public AudioSource audioSource;
    public GameObject puente_Isla2;

    [Header("Audios Sparky")]
    public AudioClip audioInstruccionIsla1;

    [Header("Objetos Zona Comida")]
    public VocabObject watermelon;
    public VocabObject blueberry;
    public VocabObject cheese;

    [Header("Zona Ropa")]
    public GameObject zonaRopaActivator;

    [Header("Zona Escolar")]
    public GameObject zonaEscolarActivator;

    private bool zonaComidaComplete = false;
    private bool zonaRopaComplete = false;
    private bool zonaEscolarComplete = false;
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
        if (puente_Isla2 != null)
            puente_Isla2.SetActive(false);

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

    // Llamado por trigger de entrada a Isla 1
    public void OnPlayerEnterIsla1()
    {
        PlayAudio(audioInstruccionIsla1);
        VocabCardManager.Instance.ShowVocabCard();
    }

    // Zona Comida - cuando el jugador añade una palabra
    public void OnFoodWordComplete()
    {
        int foodCount = VocabCardManager.Instance.GetFoodCount();
        if (foodCount >= 3)
        {
            zonaComidaComplete = true;
            SpatialBridge.coreGUIService.DisplayToastMessage(
                "Great! Now explore the Clothes Zone!");
            CheckAllZonesComplete();
        }
    }

    // Zona Ropa - cuando completa una palabra
    public void OnClothesWordComplete(int index)
    {
        //AudioMatchManager.Instance.ShowQuestion(index);Ya no necesario, AudioMatchManager se activa por ClothesObject
    }

    // Zona Ropa completa
    public void OnZonaRopaComplete()
    {
        zonaRopaComplete = true;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Amazing! Now explore the School Zone!");
        CheckAllZonesComplete();
    }

    // Zona Escolar completa
    public void OnZonaEscolarComplete()
    {
        zonaEscolarComplete = true;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Excellent! Your Vocab Card is complete!");
        CheckAllZonesComplete();
    }

    private void CheckAllZonesComplete()
    {
        if (zonaComidaComplete && zonaRopaComplete && zonaEscolarComplete)
            OnIsla1Complete();
    }

    public void OnVocabCardComplete()
    {
        zonaComidaComplete = true;
        zonaRopaComplete = true;
        zonaEscolarComplete = true;
        OnIsla1Complete();
    }

    private void OnIsla1Complete()
    {
        VocabCardManager.Instance.HideVocabCard();

        if (puente_Isla2 != null)
            puente_Isla2.SetActive(true);

        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Congratulations! The bridge to Island 2 is now open!");

        GameProgressManager.Instance.AwardDailyLifeScoutMedal();
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
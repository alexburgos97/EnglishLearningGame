using UnityEngine;
using SpatialSys.UnitySDK;

public class Isla1Manager : MonoBehaviour
{
    public static Isla1Manager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject syntaxSprite;
    public AudioSource audioSource;
    public GameObject puente_Isla2;

    [Header("Panel de bienvenida")]
    public GameObject panelBienvenida;

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

    [Header("Flechas de dirección")]
    public GameObject[] flechasZonaComida;
    public GameObject[] flechasZonaRopa;
    public GameObject[] flechasZonaEscolar;
    public GameObject[] flechasIsla1Completa;

    [Header("Bloqueadores")]
    public GameObject[] bloqueadoresIsla1;

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

        // Desactivar panel al inicio
        if (panelBienvenida != null)
            panelBienvenida.SetActive(false);

        // Desactivar zonas al inicio
        if (zonaRopaActivator != null)
            zonaRopaActivator.SetActive(false);
        if (zonaEscolarActivator != null)
            zonaEscolarActivator.SetActive(false);

        // Desactivar todas las flechas al inicio
        DesactivarFlechas(flechasZonaComida);
        DesactivarFlechas(flechasZonaRopa);
        DesactivarFlechas(flechasZonaEscolar);
        DesactivarFlechas(flechasIsla1Completa);
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

            if (panelBienvenida != null)
                panelBienvenida.SetActive(false);
        }
    }

    private void ActivarFlechas(GameObject[] flechas)
    {
        if (flechas == null) return;
        foreach (GameObject flecha in flechas)
            if (flecha != null) flecha.SetActive(true);
    }

    private void DesactivarFlechas(GameObject[] flechas)
    {
        if (flechas == null) return;
        foreach (GameObject flecha in flechas)
            if (flecha != null) flecha.SetActive(false);
    }

    public void OnPlayerEnterIsla1()
    {
        PlayAudio(audioInstruccionIsla1);
        VocabCardManager.Instance.ShowVocabCard();

        if (panelBienvenida != null)
            panelBienvenida.SetActive(true);
    }

    public void OnPlayerExitIsla1()
    {
        VocabCardManager.Instance.HideVocabCard();
    }

    public void OnPlayerNearBridge()
    {
        if (!zonaComidaComplete || !zonaRopaComplete || !zonaEscolarComplete)
        {
            SpatialBridge.coreGUIService.DisplayToastMessage(
                "Complete all activities to unlock the bridge!");
        }
    }

    public void OnFoodWordComplete()
    {
        int foodCount = VocabCardManager.Instance.GetFoodCount();
        if (foodCount >= 3)
        {
            zonaComidaComplete = true;

            // Activar Zona Ropa
            if (zonaRopaActivator != null)
                zonaRopaActivator.SetActive(true);

            // Activar flechas hacia Zona Ropa
            ActivarFlechas(flechasZonaComida);

            SpatialBridge.coreGUIService.DisplayToastMessage(
                "Great! Now explore the Clothes Zone!");
            CheckAllZonesComplete();
        }
    }

    public void OnClothesWordComplete(int index) { }

    public void OnZonaRopaComplete()
    {
        zonaRopaComplete = true;

        // Activar Zona Escolar
        if (zonaEscolarActivator != null)
            zonaEscolarActivator.SetActive(true);

        // Cambiar flechas hacia Zona Escolar
        DesactivarFlechas(flechasZonaComida);
        ActivarFlechas(flechasZonaRopa);

        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Amazing! Now explore the School Zone!");
        CheckAllZonesComplete();
    }

    public void OnZonaEscolarComplete()
    {
        zonaEscolarComplete = true;

        // Cambiar flechas hacia salida
        DesactivarFlechas(flechasZonaRopa);
        ActivarFlechas(flechasZonaEscolar);

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
        DesactivarFlechas(flechasZonaEscolar);
        ActivarFlechas(flechasIsla1Completa);

        if (bloqueadoresIsla1 != null)
            foreach (GameObject bloqueador in bloqueadoresIsla1)
                if (bloqueador != null) bloqueador.SetActive(false);

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
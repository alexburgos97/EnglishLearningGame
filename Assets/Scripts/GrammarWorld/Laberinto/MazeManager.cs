using UnityEngine;
using SpatialSys.UnitySDK;

public class MazeManager : MonoBehaviour
{
    public static MazeManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject syntaxSprite;
    public AudioSource audioSource;
    public GameObject triggerEntrada;
    public GameObject triggerSalida;
    public Transform mazeCanvas;

    [Header("Medalla")]
    public GameObject medallaSprite;

    [Header("Triggers del laberinto")]
    public GameObject[] triggers;

    [Header("Audios de instrucciones")]
    public AudioClip[] instrucciones;

    [Header("Textos de instrucciones")]
    public string[] textos;

    private int currentStep = 0;
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
        syntaxSprite.SetActive(false);
        triggerSalida.SetActive(false);

        for (int i = 0; i < triggers.Length; i++)
            triggers[i].SetActive(false);

        originalScale = syntaxSprite.transform.localScale;
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

    public void StartMaze()
    {
        currentStep = 0;
        triggers[0].SetActive(true);

        MoveToPosition(triggerEntrada.transform);

        PlayInstruction(0);
        Invoke(nameof(DisableEntrance), 0.5f);
    }

    private void DisableEntrance()
    {
        triggerEntrada.SetActive(false);
    }

    public void OnTriggerReached(int step)
    {
        if (step != currentStep) return;

        triggers[currentStep].SetActive(false);

        MoveToPosition(triggers[currentStep].transform);

        int audioIndex = currentStep + 1;
        currentStep++;

        if (currentStep < triggers.Length)
        {
            triggers[currentStep].SetActive(true);
            PlayInstruction(audioIndex);
        }
        else
        {
            PlayInstruction(audioIndex);
            triggerSalida.SetActive(true);
        }
    }

    private void MoveToPosition(Transform target)
    {
        syntaxSprite.SetActive(true);
        syntaxSprite.transform.position = target.position + Vector3.up * 1.5f;
        syntaxSprite.transform.rotation = target.rotation;

        mazeCanvas.position = target.position + Vector3.up * 2f;
        mazeCanvas.rotation = target.rotation;
    }

    private void PlayInstruction(int index)
    {
        if (index < textos.Length)
            MazeUIManager.Instance.ShowInstruction(textos[index]);

        if (index < instrucciones.Length && instrucciones[index] != null)
        {
            audioSource.clip = instrucciones[index];
            audioSource.Play();
            isTalking = true;
            pulseTimer = 0f;
        }
    }

    public void ReplayCurrentInstruction()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
            isTalking = true;
            pulseTimer = 0f;
        }
    }

    public void OnMazeCompleted()
    {
        syntaxSprite.SetActive(false);
        MazeUIManager.Instance.HidePanel();
        GameProgressManager.Instance.AwardPathfinderMedal();

        // Desactivar la medalla de la escena
        Invoke(nameof(DesactivarMedalla), 2f);
    }

    private void DesactivarMedalla()
    {
        if (medallaSprite != null)
            medallaSprite.SetActive(false);
    }
}
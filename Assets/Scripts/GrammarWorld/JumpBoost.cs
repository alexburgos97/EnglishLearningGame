using UnityEngine;

/// <summary>
/// Power-up de salto para el cubo "PowerUpSalto" (antes escalable en Spatial.io).
/// Solo se vuelve visible/funcional cuando el jugador tiene las 3 medallas de GrammarWorld;
/// al atravesar el collider trigger le da un salto muy alto por tiempo limitado para poder
/// subirse al cubo sólido.
/// </summary>
public class JumpBoost : MonoBehaviour
{
    [Header("Requisito de medallas")]
    [Tooltip("Si está activo, el power-up solo se vuelve visible/funcional cuando el jugador tiene hasBuildersMedal, hasVerbMaster y hasPathfinder en GameProgressManager.")]
    public bool requireAllGrammarMedals = true;

    [Header("Referencias")]
    [Tooltip("Collider sólido original del cubo (para pararse/subir encima). Se desactiva junto con el visual mientras falten medallas.")]
    public Collider solidCollider;
    [Tooltip("Collider marcado como trigger que detecta al jugador atravesando el power-up y le da el salto. Puede ser un poco más grande que el sólido para que sea fácil de tocar.")]
    public Collider pickupTrigger;
    [Tooltip("Renderer del cubo. Si se deja vacío se busca automáticamente en este GameObject.")]
    public Renderer visualRenderer;

    [Header("Boost de salto")]
    public float boostedJumpHeight = 8f;
    public float boostDuration = 5f;

    [Header("Efecto visual pulsante (emisión del material, no afecta el tamaño del collider sólido)")]
    public float pulseSpeed = 2f;
    public Color pulseColor = Color.cyan;
    [Range(0f, 4f)] public float pulseIntensity = 2f;

    private bool isUnlocked = false;
    private Material materialInstance;
    private bool supportsEmission = false;
    private float pulseTimer = 0f;

    void Awake()
    {
        if (visualRenderer == null)
            visualRenderer = GetComponent<Renderer>();

        if (pickupTrigger != null)
            pickupTrigger.isTrigger = true;

        if (visualRenderer != null)
        {
            materialInstance = visualRenderer.material;
            supportsEmission = materialInstance.HasProperty("_EmissionColor");
            if (supportsEmission)
                materialInstance.EnableKeyword("_EMISSION");
        }

        SetUnlocked(false);
    }

    void Update()
    {
        bool shouldBeUnlocked = !requireAllGrammarMedals || HasAllGrammarMedals();
        if (shouldBeUnlocked != isUnlocked)
            SetUnlocked(shouldBeUnlocked);

        if (isUnlocked && supportsEmission)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulse = (Mathf.Sin(pulseTimer) + 1f) * 0.5f; // 0..1
            materialInstance.SetColor("_EmissionColor", pulseColor * Mathf.Lerp(0f, pulseIntensity, pulse));
        }
    }

    private bool HasAllGrammarMedals()
    {
        if (GameProgressManager.Instance == null) return false;
        return GameProgressManager.Instance.HasBuildersMedal()
            && GameProgressManager.Instance.HasVerbMaster()
            && GameProgressManager.Instance.HasPathfinder();
    }

    private void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;

        if (visualRenderer != null)
            visualRenderer.enabled = unlocked;
        if (solidCollider != null)
            solidCollider.enabled = unlocked;
        if (pickupTrigger != null)
            pickupTrigger.enabled = unlocked;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isUnlocked) return;
        if (!other.CompareTag("Player")) return;

        if (PlayerController.Instance != null)
            PlayerController.Instance.ApplyJumpBoost(boostedJumpHeight, boostDuration);
    }
}

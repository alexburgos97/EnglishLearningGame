using UnityEngine;

public class LavaEffect : MonoBehaviour
{
    [Header("Referencias")]
    public Renderer lavaRenderer;
    public ParticleSystem smokeParticles;

    [Header("Materiales")]
    public Material hotMaterial;
    public Material coldMaterial;

    [Header("Scroll de textura")]
    public float scrollSpeedX = 0.05f;
    public float scrollSpeedY = 0.03f;

    [Header("Enfriamiento")]
    public float coolingSpeed = 0.5f;

    private bool isCooling = false;
    private bool isCooled = false;
    private float coolingProgress = 0f;

    void Start()
    {
        if (lavaRenderer == null)
            lavaRenderer = GetComponent<Renderer>();

        if (hotMaterial != null)
            lavaRenderer.material = hotMaterial;
    }

    void Update()
    {
        if (lavaRenderer == null) return;

        // Scroll de textura solo cuando está caliente
        if (!isCooled && !isCooling)
        {
            float offsetX = Time.time * scrollSpeedX;
            float offsetY = Time.time * scrollSpeedY;
            lavaRenderer.material.SetTextureOffset(
                "_MainTex", new Vector2(offsetX, offsetY));
        }

        // Enfriamiento gradual
        if (isCooling)
        {
            coolingProgress += coolingSpeed * Time.deltaTime;
            coolingProgress = Mathf.Clamp01(coolingProgress);

            // Interpolar entre los dos materiales
            LerpMaterials(coolingProgress);

            if (coolingProgress >= 1f)
            {
                isCooling = false;
                isCooled = true;
                lavaRenderer.material = coldMaterial;
            }
        }
    }

    private void LerpMaterials(float t)
    {
        if (hotMaterial == null || coldMaterial == null) return;

        // Crear material temporal para la transicion
        Material tempMat = lavaRenderer.material;

        // Interpolar color principal
        if (hotMaterial.HasProperty("_Color") && coldMaterial.HasProperty("_Color"))
        {
            tempMat.SetColor("_Color", Color.Lerp(
                hotMaterial.GetColor("_Color"),
                coldMaterial.GetColor("_Color"), t));
        }

        // Interpolar emisión
        if (hotMaterial.HasProperty("_EmissionColor") && 
            coldMaterial.HasProperty("_EmissionColor"))
        {
            tempMat.SetColor("_EmissionColor", Color.Lerp(
                hotMaterial.GetColor("_EmissionColor"),
                coldMaterial.GetColor("_EmissionColor"), t));
        }

        // Reducir scroll gradualmente
        float currentScrollX = scrollSpeedX * (1f - t);
        float currentScrollY = scrollSpeedY * (1f - t);
        tempMat.SetTextureOffset("_MainTex", 
            new Vector2(Time.time * currentScrollX, 
                       Time.time * currentScrollY));
    }

    public void CoolLava()
    {
        isCooling = true;

        // Detener partículas gradualmente
        if (smokeParticles != null)
        {
            var emission = smokeParticles.emission;
            emission.rateOverTime = 0f;
            smokeParticles.Stop();
        }
    }
}
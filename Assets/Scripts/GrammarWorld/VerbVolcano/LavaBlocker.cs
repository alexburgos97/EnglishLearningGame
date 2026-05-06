using UnityEngine;
using SpatialSys.UnitySDK;

public class LavaBlocker : MonoBehaviour
{
    [Header("Bloqueador")]
    public Transform puntoReaparicion;

    [Header("Lava")]
    public Renderer lavaRenderer;
    public Color hotColor = new Color(1f, 0.3f, 0f);
    public Color coldColor = new Color(0.2f, 0.2f, 0.2f);
    public float scrollSpeedX = 0.05f;
    public float scrollSpeedY = 0.03f;

    [Header("Humo")]
    public GameObject smokeParticlesPrefab;
    private ParticleSystem smokeParticles;

    private float lastTeleportTime = 0f;
    private bool isCooled = false;
    private bool isCooling = false;
    private float coolingSpeed = 1f;
    private Material lavaMaterial;

    void Start()
    {
        if (lavaRenderer != null)
        {
            lavaMaterial = lavaRenderer.material;
            lavaMaterial.color = hotColor;
        }

        // Crear partículas de humo
        if (smokeParticlesPrefab != null)
        {
            GameObject smokeObj = Instantiate(
                smokeParticlesPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity);
            smokeObj.transform.SetParent(transform);
            smokeParticles = smokeObj.GetComponent<ParticleSystem>();
        }
        else
        {
            // Crear partículas simples sin prefab
            GameObject smokeObj = new GameObject("SmokeParticles");
            smokeObj.transform.SetParent(transform);
            smokeObj.transform.localPosition = Vector3.up * 0.5f;
            smokeParticles = smokeObj.AddComponent<ParticleSystem>();

            var main = smokeParticles.main;
            main.startColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            main.startSize = 2f;
            main.startLifetime = 3f;
            main.startSpeed = 1f;
            main.maxParticles = 50;

            var emission = smokeParticles.emission;
            emission.rateOverTime = 10f;

            var shape = smokeParticles.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(5f, 0.1f, 5f);
        }
    }

    void Update()
    {
        // Scroll de textura
        if (lavaMaterial != null && !isCooled)
        {
            float offsetX = Time.time * scrollSpeedX;
            float offsetY = Time.time * scrollSpeedY;
            lavaMaterial.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
        }

        // Enfriamiento gradual
        if (isCooling && lavaMaterial != null)
        {
            lavaMaterial.color = Color.Lerp(
                lavaMaterial.color,
                coldColor,
                coolingSpeed * Time.deltaTime);

            if (Vector4.Distance(lavaMaterial.color, coldColor) < 0.01f)
            {
                lavaMaterial.color = coldColor;
                isCooling = false;
                isCooled = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCooled) return;
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastTeleportTime < 2f) return;
        lastTeleportTime = Time.time;

        SpatialBridge.actorService.localActor.avatar.position =
            puntoReaparicion.position;
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Complete both challenges first!");
    }

    public void CoolLava()
    {
        isCooling = true;

        // Detener partículas de humo
        if (smokeParticles != null)
            smokeParticles.Stop();
    }
}
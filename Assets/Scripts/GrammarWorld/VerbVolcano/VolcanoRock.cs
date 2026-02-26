using UnityEngine;
using TMPro;

public class VolcanoRock : MonoBehaviour
{
    public TextMeshProUGUI pronounText;
    public Transform puntoLanzamiento;
    public Transform puntoJugador;

    private Rigidbody rb;
    private bool isLaunched = false;
    private bool hasLanded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(string pronoun, Transform targetPoint)
    {
        hasLanded = false;
        isLaunched = true;
        pronounText.text = pronoun;
        gameObject.SetActive(true);

        // Posicionar en el punto de lanzamiento
        transform.position = puntoLanzamiento.position;

        // Calcular velocidad para trayectoria parabólica
        Vector3 target = targetPoint.position;
        Vector3 origin = puntoLanzamiento.position;
        float height = 8f;
        float gravity = Mathf.Abs(Physics.gravity.y);

        float displacementY = target.y - origin.y;
        Vector3 displacementXZ = new Vector3(
            target.x - origin.x, 0, target.z - origin.z);

        float time = Mathf.Sqrt(-2 * height / -gravity) +
            Mathf.Sqrt(2 * (displacementY - height) / -gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * -gravity * height);
        Vector3 velocityXZ = displacementXZ / time;

        rb.velocity = velocityXZ + velocityY;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isLaunched) return;
        if (hasLanded) return;

        // Si toca el suelo o la lava
        if (collision.gameObject.CompareTag("Lava") ||
            collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            isLaunched = false;
            gameObject.SetActive(false);
            VolcanoQuizManager.Instance.OnRockLanded();
        }
    }
}
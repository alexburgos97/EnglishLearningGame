using UnityEngine;

public class Levitacion : MonoBehaviour
{
    [Header("Movimiento")]
    public float altura = 0.3f;
    public float velocidad = 1f;

    [Header("Rotacion")]
    public bool rotar = true;
    public float velocidadRotacion = 30f;

    [Header("Pulso")]
    public bool pulso = true;
    public float velocidadPulso = 2f;
    public float magnitudPulso = 0.1f;

    private Vector3 posicionInicial;
    private Vector3 escalaInicial;
    private float tiempoInicial;

    void Start()
    {
        posicionInicial = transform.position;
        escalaInicial = transform.localScale;
        tiempoInicial = Random.Range(0f, 2f);
    }

    void Update()
    {
        float t = Time.time + tiempoInicial;

        // Movimiento arriba y abajo
        float nuevaY = posicionInicial.y + Mathf.Sin(t * velocidad) * altura;
        transform.position = new Vector3(
            posicionInicial.x,
            nuevaY,
            posicionInicial.z);

        // Rotacion
        if (rotar)
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);

        // Pulso de escala
        if (pulso)
        {
            float pulse = 1f + Mathf.Sin(t * velocidadPulso) * magnitudPulso;
            transform.localScale = escalaInicial * pulse;
        }
    }
}
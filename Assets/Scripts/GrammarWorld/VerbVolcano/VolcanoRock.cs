using UnityEngine;
using TMPro; // Necesario para modificar el texto de la roca

public class VolcanoRock : MonoBehaviour
{
    [Header("Configuración Visual")]
    [Tooltip("El texto 3D o de UI de la roca donde aparecerá el pronombre")]
    public TMP_Text pronounText;
    public float speed = 5f;

    private Transform target;
    private bool isMoving = false;

    // El VolcanoUIManager llama a este método al cargar una pregunta nueva
    public void Launch(string textToDisplay, Transform targetPoint)
    {
        if (pronounText != null)
        {
            pronounText.text = textToDisplay;
        }

        target = targetPoint;
        
        // Reiniciamos el movimiento
        isMoving = true;
    }

    void Update()
    {
        // Movimiento simple hacia el objetivo (el jugador o el centro de la plataforma)
        if (isMoving && target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // Se detiene al llegar al destino
            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                isMoving = false;
            }
        }
    }
}
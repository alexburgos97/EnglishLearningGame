using UnityEngine;


public class RespawnTrigger : MonoBehaviour
{
    [Header("Arrastra aquí tu Punto de Reaparicion")]
    public Transform puntoInicio;

    public void Teletransportar()
    {
        if (puntoInicio != null)
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.Teleport(puntoInicio.position, puntoInicio.rotation);
            }

            Debug.Log("Watch your step! Let's try again.");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeAppear : MonoBehaviour
{
    [SerializeField] private GameObject[] bridgeSegments;

    // 1. Función para HACER APARECER el segmento basado en el índice
    public void ShowSegment(int index)
    {
        if (IsValidIndex(index))
        {
            bridgeSegments[index].SetActive(true);
        }
    }

    // 2. Función para HACER DESAPARECER el segmento basado en el índice
    public void HideSegment(int index)
    {
        if (IsValidIndex(index))
        {
            bridgeSegments[index].SetActive(false);
        }
    }

    // Opcional: Función para alternar (si está activo se apaga, y viceversa)
    public void ToggleSegment(int index)
    {
        if (IsValidIndex(index))
        {
            bool currentState = bridgeSegments[index].activeSelf;
            bridgeSegments[index].SetActive(!currentState);
        }
    }

    // Helper: Verifica que el array no sea nulo y que el índice esté dentro del rango
    private bool IsValidIndex(int index)
    {
        if (bridgeSegments == null || bridgeSegments.Length == 0)
        {
            Debug.LogWarning("El array bridgeSegments está vacío o es nulo.");
            return false;
        }

        if (index < 0 || index >= bridgeSegments.Length)
        {
            Debug.LogWarning($"El índice {index} está fuera de rango. Tienes {bridgeSegments.Length} segmentos.");
            return false;
        }

        return true;
    }
}

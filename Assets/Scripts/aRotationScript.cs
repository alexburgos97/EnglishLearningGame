using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aRotationScript : MonoBehaviour
{
    [SerializeField] private float velocidadRotacion = 50f;
    [SerializeField] private Vector3 ejeRotacion = Vector3.up; // Up = eje Y

    void Update()
    {
        transform.Rotate(ejeRotacion * velocidadRotacion * Time.deltaTime);
    }
}

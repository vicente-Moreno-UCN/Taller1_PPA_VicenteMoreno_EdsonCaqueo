using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Efectofondolejano : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camara;

    [Header("Configuración Parallax")]
    [Range(0f, 1f)]
    public float efectoParallaxX = 0.5f;

    private Vector3 ultimaPosicionCamara;

    private void Start()
    {
        if (camara == null)
        {
            camara = Camera.main.transform;
        }

        ultimaPosicionCamara = camara.position;
    }

    private void LateUpdate()
    {
        Vector3 desplazamientoCamara = camara.position - ultimaPosicionCamara;

        //se mueve el fondo solo una fracción del  movimiento de madeline
        transform.position += new Vector3(desplazamientoCamara.x * (1f - efectoParallaxX), desplazamientoCamara.y * (1f - efectoParallaxX), 0f);

        //se actualiza la posición guardada de la cámara
        ultimaPosicionCamara = camara.position;
    }
}

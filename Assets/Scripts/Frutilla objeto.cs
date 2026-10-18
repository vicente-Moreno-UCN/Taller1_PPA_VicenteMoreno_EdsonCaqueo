using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrutillaObjeto : MonoBehaviour
{
    [Header("Efectos de Recogida")]
    [SerializeField] private AudioClip sonidoRecogida;
    [SerializeField] private GameObject efectoParticulasPrefab; 

    private bool recolectada = false;

    //flotacion
    private Vector3 posicionInicial;
    private float frecuenciaFlotacion = 5f;
    private float amplitudFlotacion = 0.15f;

    private void Start()
    {
        posicionInicial = transform.position;
    }

    private void Update()
    {
        //floacion x2
        if (!recolectada)
        {
            float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * frecuenciaFlotacion) * amplitudFlotacion;
            transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !recolectada)
        {
            recolectada = true;
            RecogerFrutilla();
        }
    }

    private void RecogerFrutilla()
    {
        //sonido recogida
        if (sonidoRecogida != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);
        }

        //contador frutilla
        GestorFrutillas.Instance.RegistrarFrutillaRecogida();

        //sin colision
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        //desaparece la frutilla dando tiempo para el sonido
        Destroy(gameObject, 0.5f);
    }
}

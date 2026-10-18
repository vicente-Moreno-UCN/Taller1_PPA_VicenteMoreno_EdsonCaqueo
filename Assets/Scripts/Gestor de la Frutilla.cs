using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorFrutillas : MonoBehaviour
{
    public static GestorFrutillas Instance;

    [Header("Configuración de Frutillas")]
    private int frutillasRecogidas = 0;
    private int totalParaAbrir = 3;

    [Header("Suelo que se Mueve")]
    [SerializeField] private Transform sueloMovible; 
    [SerializeField] private Vector3 desplazamientoDestino; 
    [SerializeField] private float velocidadMovimiento = 2f;

    private Vector3 posicionInicialSuelo;
    private Vector3 posicionFinalSuelo;
    private bool activarMovimiento = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (sueloMovible != null)
        {
            posicionInicialSuelo = sueloMovible.position;
            posicionFinalSuelo = posicionInicialSuelo + desplazamientoDestino;
        }
    }

    private void Update()
    {
        if (activarMovimiento && sueloMovible != null)
        {
            //mueve el suelo para dejar terminar el final abierto y poder terminar el nivel
            sueloMovible.position = Vector3.MoveTowards(sueloMovible.position, posicionFinalSuelo, velocidadMovimiento * Time.deltaTime);
        }
    }

    public void RegistrarFrutillaRecogida()
    {
        frutillasRecogidas++;
        Debug.Log("Frutillas recogidas: " + frutillasRecogidas + "/" + totalParaAbrir);

        if (frutillasRecogidas >= totalParaAbrir)
        {
            AbrirCamino();
        }
    }

    private void AbrirCamino()
    {
        activarMovimiento = true;
        Debug.Log("¡Tercera frutilla recogida! Moviendo el suelo...");
    }
}
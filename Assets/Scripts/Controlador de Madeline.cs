using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMadeline : MonoBehaviour
{
    [Header("Parámetros de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private float fuerzaSalto = 10f;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform detectorSuelo;
    [SerializeField] private float radioDeteccion = 0.2f;
    [SerializeField] private LayerMask capaSuelo;

    [Header("Efectos de Audio")]
    [SerializeField] private AudioClip sonidoSalto;

    [Header("UI y Efectos de Muerte")]
    public GameObject panelGameOver;
    public AudioClip sonidoMuerte;

    private AudioSource audioSource;
    private Animator animator;
    private Rigidbody2D rb;

    private bool estaMuerto = false;
    private float movimientoHorizontal;
    private bool enSuelo;
    private bool mirandoDerecha = true;
    private bool estaAgachado = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //camara virtual
        CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            CinemachineConfiner2D confiner = vcam.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                confiner.InvalidateCache();
            }
        }
    }

    private void Update()
    {
        //reinicio por muerte
        if (estaMuerto)
        {
            if (panelGameOver != null && panelGameOver.activeSelf)
            {
                if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
                {
                    ReiniciarNivel();
                }
            }
            return; //no procesa movimiento si está muerta y solo deja reiniciar cuando el panel gma eover esta activo
        }

        //lectura de Entradas
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        //detección de teabag
        estaAgachado = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        //saltito
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && enSuelo)
        {
            Saltar();
        }

        //girar
        VoltearSprite();

        //animaciones
        ActualizarAnimaciones();
    }

    private void FixedUpdate()
    {
        //verificaion si toca el suelo
        enSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);

        //movimiento
        Mover();
    }

    private void Mover()
    {
        if (estaAgachado && enSuelo)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(movimientoHorizontal * velocidadMovimiento, rb.velocity.y);
    }

    private void Saltar()
    {
        rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);

        if (sonidoSalto != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoSalto);
        }
    }

    private void VoltearSprite()
    {
        if (movimientoHorizontal > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (movimientoHorizontal < 0 && mirandoDerecha)
        {
            Girar();
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void ActualizarAnimaciones()
    {
        if (animator == null) return;

        animator.SetFloat("Velocidad", Mathf.Abs(movimientoHorizontal));
        animator.SetBool("EnSuelo", enSuelo);
        animator.SetBool("EstaAgachado", estaAgachado);
    }

    private void OnDrawGizmosSelected()
    {
        if (detectorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detectado con: " + collision.gameObject.name + " (Tag: " + collision.tag + ")");//no funcionan los pinchos, esto es para probar
        if ((collision.CompareTag("Pinchos") || collision.CompareTag("Enemigo")) && !estaMuerto)
        {
            StartCoroutine(SecuenciaMuerte());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisión física detectada con: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");//no funcionan los pinchos, esto es para probar
        if ((collision.gameObject.CompareTag("Enemigo") || collision.gameObject.CompareTag("Pinchos")) && !estaMuerto)
        {
            StartCoroutine(SecuenciaMuerte());
        }
    }

    private IEnumerator SecuenciaMuerte()
    {
        estaMuerto = true;

        //detiene el movimiento
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        //sonido muerte
        if (audioSource != null && sonidoMuerte != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
        }

        //animacion muerte
        if (animator != null)
        {
            animator.SetBool("Morir", true);
        }

        //se pausa para la animacion y sonido
        yield return new WaitForSeconds(1f);

        //Game Over
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
    }

    private void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
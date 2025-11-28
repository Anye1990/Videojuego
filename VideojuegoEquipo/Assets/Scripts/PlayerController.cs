using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 8f;

    [Header("Salto")]
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip doubleJumpSound;
    public AudioClip hurtSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private Animator animator;
    public RuntimeAnimatorController[] characterAnimators;

    // Variables para guardar el input (teclas)
    private float moveInput;
    private bool jumpRequest;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        jumpsRemaining = maxJumps;

        // Cargar personaje seleccionado
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (characterAnimators.Length > 0 && selectedCharacterIndex < characterAnimators.Length)
        {
            animator.runtimeAnimatorController = characterAnimators[selectedCharacterIndex];
        }
    }

    // Update: Aquí SOLO leemos las teclas (Se ejecuta cada frame visual)
    private void Update()
    {
        // 1. Leer input horizontal
        moveInput = Input.GetAxis("Horizontal");

        // 2. Leer input de salto
        if (Input.GetButtonDown("Jump"))
        {
            jumpRequest = true; // Avisamos que queremos saltar
        }

        // 3. Animaciones y Giros (Son visuales, van en Update)
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    // FixedUpdate: Aquí SOLO movemos físicas (Se ejecuta 50 veces exactas por seg)
    private void FixedUpdate()
    {
        // 1. Detectar suelo
        if (groundCheck != null)
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            // Resetear saltos al tocar suelo
            if (isGrounded && !wasGrounded && rb.linearVelocity.y <= 0.1f)
            {
                jumpsRemaining = maxJumps;
                animator.SetBool("IsJumping", false);
            }
        }

        // 2. Mover personaje (Usamos el input guardado)
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // 3. Ejecutar salto si se pidió
        if (jumpRequest)
        {
            if (jumpsRemaining > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animator.SetBool("IsJumping", true);

                // Sonidos
                if (jumpsRemaining == maxJumps) PlaySound(jumpSound);
                else PlaySound(doubleJumpSound);

                jumpsRemaining--;
            }
            jumpRequest = false; // Ya saltamos, apagamos la solicitud
        }
    }
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void TakeDamage(int cantidad)
    {
        // Solo llamamos al GameManager, él se encarga de los corazones y vidas
        if (GameManager.instance != null)
        {
            GameManager.instance.TakeDamage(cantidad);
            PlaySound(hurtSound); // Si pusiste sonido
        }
    }

}
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

    [Header("Audio")] // NUEVO: Sección de Audio
    public AudioClip jumpSound;       // Sonido salto normal
    public AudioClip doubleJumpSound; // Sonido doble salto
    public AudioClip hurtSound;       // Sonido al recibir daño
    private AudioSource audioSource;  // El componente que emite el sonido

    private Rigidbody2D rb;
    private Animator animator;
    public RuntimeAnimatorController[] characterAnimators;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // NUEVO: Obtener el componente AudioSource
        audioSource = GetComponent<AudioSource>();

        jumpsRemaining = maxJumps;

        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (characterAnimators.Length > 0 && selectedCharacterIndex < characterAnimators.Length)
        {
            animator.runtimeAnimatorController = characterAnimators[selectedCharacterIndex];
        }
    }

    private void Update()
    {
        // ... (Tu lógica de GroundCheck sigue igual) ...
        if (groundCheck != null)
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (isGrounded && !wasGrounded && rb.linearVelocity.y <= 0.1f)
            {
                jumpsRemaining = maxJumps;
                animator.SetBool("IsJumping", false);
            }
        }
        // ... (Fallback de GroundCheck) ...

        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (move > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (move < 0) transform.localScale = new Vector3(-1, 1, 1);

        animator.SetFloat("Speed", Mathf.Abs(move));

        // LÓGICA DE SALTO CON SONIDO
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpsRemaining > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animator.SetBool("IsJumping", true);

                // NUEVO: Lógica para saber qué sonido tocar
                if (jumpsRemaining == maxJumps)
                {
                    // Es el primer salto
                    PlaySound(jumpSound);
                }
                else
                {
                    // Es el doble salto
                    PlaySound(doubleJumpSound);
                }

                jumpsRemaining--;
            }
        }
    }

    public void TakeDamage(int cantidad)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ChangeHealth(-cantidad);
            // NUEVO: Reproducir sonido de daño
            PlaySound(hurtSound);
        }
    }

    // NUEVO: Función auxiliar para reproducir sonidos sin interrumpir
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // ... (Gizmos sigue igual) ...
}
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 10f; // Ajustado un poco más alto para mejor feel

    [Header("Salto Avanzado (Game Feel)")]
    public int maxJumps = 2;
    private int jumpsRemaining;

    // Coyote Time: Permite saltar un poco después de caer
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    // Jump Buffer: Recuerda el salto si se presionó un poco antes de tocar suelo
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Feedback Visual")]
    public SpriteRenderer spriteRenderer;
    public Color hurtColor = Color.red;
    private Color originalColor;
    private bool isInvincible = false;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip doubleJumpSound;
    public AudioClip hurtSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private Animator animator;
    public RuntimeAnimatorController[] characterAnimators;

    // Input
    private float moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        jumpsRemaining = maxJumps;

        // Cargar personaje
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (characterAnimators.Length > 0 && selectedCharacterIndex < characterAnimators.Length)
        {
            animator.runtimeAnimatorController = characterAnimators[selectedCharacterIndex];
        }
    }

    private void Update()
    {
        // 1. Input y Timers
        moveInput = Input.GetAxis("Horizontal");

        // Gestión de Coyote Time
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Gestión de Jump Buffer
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // 2. Animaciones
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    private void FixedUpdate()
    {
        // 1. Detectar suelo
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Resetear saltos al tocar suelo
        if (isGrounded && !wasGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpsRemaining = maxJumps;
            animator.SetBool("IsJumping", false);
        }

        // 2. Mover personaje (si no estamos recibiendo knockback fuerte podríamos bloquear esto, pero lo dejamos fluido)
        if (!isInvincible) // Opcional: No mover si está herido
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // 3. Lógica de Salto Mejorada
        if (jumpBufferCounter > 0)
        {
            // Salto normal (usando Coyote Time) o Doble Salto
            if (coyoteTimeCounter > 0 || jumpsRemaining > 0)
            {
                PerformJump();

                // Si saltamos en el aire (doble salto), reducimos saltos
                if (coyoteTimeCounter <= 0)
                {
                    jumpsRemaining--;
                    PlaySound(doubleJumpSound);
                }
                else
                {
                    // Primer salto
                    jumpsRemaining--; // Consumimos uno
                    PlaySound(jumpSound);
                }

                jumpBufferCounter = 0; // Salto consumido
                coyoteTimeCounter = 0; // Evitar saltar infinitamente en el aire
            }
        }
    }

    void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetBool("IsJumping", true);
    }

    // Sobrecarga para aceptar la posición del enemigo (para knockback exacto)
    public void TakeDamage(int cantidad, Transform damagerPosition = null)
    {
        if (isInvincible) return;

        if (GameManager.instance != null)
        {
            GameManager.instance.TakeDamage(cantidad);
            PlaySound(hurtSound);
        }

        StartCoroutine(DamageFeedback(damagerPosition));
    }

    // Mantener compatibilidad con scripts antiguos que llaman solo con (int)
    public void TakeDamage(int cantidad)
    {
        TakeDamage(cantidad, null);
    }

    IEnumerator DamageFeedback(Transform damager)
    {
        isInvincible = true;

        // KNOCKBACK (Retroceso)
        float recoilDirection = transform.localScale.x * -1; // Por defecto: hacia atrás de donde mira
        if (damager != null)
        {
            // Calcular dirección opuesta al enemigo
            recoilDirection = (transform.position.x - damager.position.x) > 0 ? 1 : -1;
        }

        rb.linearVelocity = Vector2.zero; // Frenar en seco
        rb.AddForce(new Vector2(recoilDirection * 5f, 5f), ForceMode2D.Impulse); // Impulso diagonal

        // PARPADEO (Blink)
        for (int i = 0; i < 5; i++) // Parpadear 5 veces
        {
            spriteRenderer.color = hurtColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }

        isInvincible = false;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            // Variación de Pitch (Voz robótica vs natural)
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
            audioSource.pitch = 1f; // Resetear pitch
        }
    }
}
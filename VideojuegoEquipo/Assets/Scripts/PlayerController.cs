using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 10f;

    [Header("Salto Avanzado")]
    public int maxJumps = 2;
    private int jumpsRemaining;
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
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

    private bool isBouncing = false;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip doubleJumpSound;
    public AudioClip hurtSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private Animator animator;
    public RuntimeAnimatorController[] characterAnimators;
    private float moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        // --- FIX: Reseteo forzoso al iniciar ---
        isBouncing = false;
        isInvincible = false;
        moveInput = 0;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.WakeUp(); // Despierta al motor de físicas si estaba dormido
        }
        // ---------------------------------------

        jumpsRemaining = maxJumps;

        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (characterAnimators.Length > 0 && selectedCharacterIndex < characterAnimators.Length)
        {
            animator.runtimeAnimatorController = characterAnimators[selectedCharacterIndex];
        }
    }

    private void Update()
    {
        if (isBouncing) return;
        // Seguridad: Si el tiempo está detenido, no procesar input
        if (Time.timeScale == 0) return;

        moveInput = Input.GetAxis("Horizontal");

        if (isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump")) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpsRemaining = maxJumps;
            animator.SetBool("IsJumping", false);
            isBouncing = false;
        }

        if (isBouncing) return;

        if (!isInvincible)
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (jumpBufferCounter > 0)
        {
            if (coyoteTimeCounter > 0 || jumpsRemaining > 0)
            {
                PerformJump();
                if (coyoteTimeCounter <= 0)
                {
                    jumpsRemaining--;
                    PlaySound(doubleJumpSound);
                }
                else
                {
                    jumpsRemaining--;
                    PlaySound(jumpSound);
                }
                jumpBufferCounter = 0;
                coyoteTimeCounter = 0;
            }
        }
    }

    void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetBool("IsJumping", true);
    }

    public void Rebote(Vector2 fuerzaImpacto)
    {
        isBouncing = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(fuerzaImpacto, ForceMode2D.Impulse);
        animator.SetBool("IsJumping", true);

        StartCoroutine(RestaurarControl());
    }

    IEnumerator RestaurarControl()
    {
        yield return new WaitForSeconds(0.5f);
        isBouncing = false;
    }

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
    public void TakeDamage(int cantidad) { TakeDamage(cantidad, null); }

    IEnumerator DamageFeedback(Transform damager)
    {
        isInvincible = true;
        float recoilDirection = transform.localScale.x * -1;
        if (damager != null) recoilDirection = (transform.position.x - damager.position.x) > 0 ? 1 : -1;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(recoilDirection * 5f, 5f), ForceMode2D.Impulse);

        if (spriteRenderer != null)
        {
            for (int i = 0; i < 5; i++)
            {
                spriteRenderer.color = hurtColor;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        isInvincible = false;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
            audioSource.pitch = 1f;
        }
    }
}
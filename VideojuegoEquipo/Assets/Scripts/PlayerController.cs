using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 8f;

    [Header("Salto")]
    public int maxJumps = 2; // Cuántos saltos puede dar (2 = doble salto)
    private int jumpsRemaining; // Saltos que quedan disponibles en el aire

    [Header("Detección de Suelo")]
    public Transform groundCheck; // Asigna aquí el objeto vacío en los pies
    public float groundCheckRadius = 0.2f; // Radio de detección
    public LayerMask groundLayer; // Define qué es suelo
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator animator;
    public RuntimeAnimatorController[] characterAnimators;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Inicializa los saltos
        jumpsRemaining = maxJumps;

        // Lee qué personaje se eligió
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Asigna el controlador correcto
        if (characterAnimators.Length > 0 && selectedCharacterIndex < characterAnimators.Length)
        {
            animator.runtimeAnimatorController = characterAnimators[selectedCharacterIndex];
        }
    }

    private void Update()
    {
        // 1. Detectar si estamos en el suelo usando OverlapCircle
        if (groundCheck != null)
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            // Si acabamos de aterrizar, reiniciar los saltos y la animación
            if (isGrounded && !wasGrounded && rb.linearVelocity.y <= 0.1f)
            {
                jumpsRemaining = maxJumps;
                animator.SetBool("IsJumping", false);
            }
        }
        else
        {
            // Fallback por si olvidaste asignar el GroundCheck (Lógica antigua)
            if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            {
                isGrounded = true;
                jumpsRemaining = maxJumps;
                animator.SetBool("IsJumping", false);
            }
            else
            {
                isGrounded = false;
            }
        }

        // 2. Movimiento Horizontal
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // Voltear personaje
        if (move > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (move < 0) transform.localScale = new Vector3(-1, 1, 1);

        animator.SetFloat("Speed", Mathf.Abs(move));

        // 3. Lógica de Salto (Modificada para Doble Salto)
        if (Input.GetButtonDown("Jump"))
        {
            // Si nos quedan saltos disponibles...
            if (jumpsRemaining > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animator.SetBool("IsJumping", true);
                jumpsRemaining--; // Restar un salto
            }
        }
    }

    public void TakeDamage(int cantidad)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ChangeHealth(-cantidad);
        }
    }

    // Dibuja el círculo en el editor para ver el radio de detección
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
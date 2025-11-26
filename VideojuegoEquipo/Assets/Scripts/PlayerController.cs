using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f;
    private Rigidbody2D rb;
    private Animator animator;
    public RuntimeAnimatorController[] characterAnimators;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 1. Recuperar el personaje seleccionado
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // 2. Asignar el animador correcto si la lista está llena
        if (characterAnimators.Length > 0 && selectedCharacterIndex < characterAnimators.Length)
        {
            animator.runtimeAnimatorController = characterAnimators[selectedCharacterIndex];
        }
    }

    private void Update()
    {
        float move = Input.GetAxis("Horizontal");

        // MOVIMIENTO (Compatible con Unity 6 y anteriores)
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // GIRO DEL PERSONAJE (FLIP)
        if (move > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (move < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // ANIMACIÓN DE CORRER
        animator.SetFloat("Speed", Mathf.Abs(move));

        // SALTO
        if (Input.GetButtonDown("Jump"))
        {
            if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animator.SetBool("IsJumping", true);
            }
        }

        // CAÍDA / ATERRIZAJE
        // Si la velocidad vertical es casi 0, asumimos que ha tocado suelo
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            animator.SetBool("IsJumping", false);
        }
    }

    public void TakeDamage(int cantidad)
    {
        GameManager.instance.ChangeHealth(-cantidad);
    }
}
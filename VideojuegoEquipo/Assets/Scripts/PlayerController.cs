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

        // Lee qué personaje se eligió (0, 1 o 2)
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Asigna el controlador correcto
        if (characterAnimators.Length > 0 && selectedCharacterIndex < characterAnimators.Length)
        {
            animator.runtimeAnimatorController = characterAnimators[selectedCharacterIndex];
        }
    }

    private void Update()
    {
        float move = Input.GetAxis("Horizontal");

        // Recuerda usar rb.linearVelocity si estás en Unity 6, o rb.velocity en versiones anteriores
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (move > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (move < 0) transform.localScale = new Vector3(-1, 1, 1);

        animator.SetFloat("Speed", Mathf.Abs(move));

        if (Input.GetButtonDown("Jump"))
        {
            if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animator.SetBool("IsJumping", true);
            }
        }

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
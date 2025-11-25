using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Aquí puedes cargar el personaje seleccionado si tienes diferentes sprites
    }

    private void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            // Chequea si está en el piso antes de saltar
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void TakeDamage(int cantidad)
    {
        GameManager.instance.ChangeHealth(-cantidad);
    }
}
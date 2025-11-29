using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;

    private Vector2 targetDirection;

    void Start()
    {
        // Al nacer, calcula la dirección hacia el jugador una sola vez
        GameObject player = GameObject.FindGameObjectWithTag("Jugador");
        if (player != null)
        {
            targetDirection = (player.transform.position - transform.position).normalized;
        }
        else
        {
            targetDirection = Vector2.left; // Por defecto a la izquierda si no hay jugador
        }

        Destroy(gameObject, 4f); // Destruir a los 4 seg si no choca con nada
    }

    void Update()
    {
        // Moverse en línea recta
        transform.Translate(targetDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca con el jugador
        if (collision.CompareTag("Jugador"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // Si choca con el suelo o paredes (asegúrate que tengan el Layer "Ground" o "Piso")
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        Debug.Log("Choque con: " + collision.gameObject.name + " | Layer: " + LayerMask.LayerToName(collision.gameObject.layer));
    }
}
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float speed = 3.0f;
    public int damage = 1;
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    // MARCA ESTA CASILLA EN EL INSPECTOR SI TU DIBUJO ORIGINAL MIRA A LA IZQUIERDA
    public bool spriteMiraIzquierda = true;
    void Update()
    {
        // Esto bloquea a la abeja en el plano 2D pase lo que pase
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        Patrol();
    }
    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // Obtenemos la posición del destino actual
        Vector3 destino = patrolPoints[currentPointIndex].position;

        // --- CORRECCIÓN CLAVE ---
        // Forzamos que el destino tenga EXACTAMENTE la misma Z que la abeja.
        // Así evitamos que se quede atascada intentando ir al fondo de la pantalla.
        destino.z = transform.position.z;

        // 1. MOVERSE
        transform.position = Vector3.MoveTowards(transform.position, destino, speed * Time.deltaTime);

        // 2. GIRAR (FLIP)
        // Usamos 'destino' en lugar de 'targetPoint.position'
        Vector3 scale = transform.localScale;

        if (destino.x > transform.position.x)
        {
            // Derecha
            scale.x = spriteMiraIzquierda ? -1 : 1;
        }
        else if (destino.x < transform.position.x)
        {
            // Izquierda
            scale.x = spriteMiraIzquierda ? 1 : -1;
        }

        scale.y = 1;
        scale.z = 1;
        transform.localScale = scale;

        // 3. CAMBIAR OBJETIVO
        // Usamos Vector2.Distance para ignorar la Z por completo
        if (Vector2.Distance(transform.position, destino) < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
        {
            Collider2D enemyCollider = GetComponent<Collider2D>();
            Collider2D playerCollider = collision.collider;

            float enemyTop = enemyCollider.bounds.max.y;
            float playerBottom = playerCollider.bounds.min.y;

            if (playerBottom >= enemyTop - 0.2f)
            {
                Destroy(gameObject);
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 6f);
                }
            }
            else
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }
}
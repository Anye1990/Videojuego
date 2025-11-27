using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public float speed = 2.0f;
    public int damage = 1;
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Girar al enemigo
        if (targetPoint.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else if (targetPoint.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    // CAMBIO AQUÍ: Usamos OnCollision en lugar de OnTrigger para detectar la física del golpe
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
        {
            // Verificamos la dirección del golpe usando los "contactos" de la colisión.
            // Si la normal Y es positiva (> 0.5), significa que el golpe vino de ARRIBA hacia abajo.
            if (collision.contacts[0].normal.y < -0.5f)
            {

                // Lógica de Respaldo más sencilla: ¿Está el jugador sobre el enemigo?
                if (collision.transform.position.y > transform.position.y + 0.5f)
                {
                    // 1. Eliminar al enemigo
                    Destroy(gameObject);

                    // 2. Hacer que el jugador rebote (Efecto Mario)
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        // Le damos un impulso hacia arriba
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 6f);
                    }
                }
                else
                {
                    HacerDano(collision.gameObject);
                }
            }
            else
            {
                HacerDano(collision.gameObject);
            }
        }
    }

    void HacerDano(GameObject playerObj)
    {
        PlayerController player = playerObj.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }
}
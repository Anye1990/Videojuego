using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float speed = 3.0f;
    public int damage = 1;
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;
    public bool spriteMiraIzquierda = true;

    void Update()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Patrol();
    }

    // "LateUpdate" se ejecuta al final de todo, ideal para corregir posiciones
    void LateUpdate()
    {
        // 1. FORZAR SIEMPRE LA POSICIÓN Z A 0 (Evita que desaparezca al fondo)
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        // 2. FORZAR SIEMPRE ROTACIÓN RECTA (Evita que gire rara)
        transform.rotation = Quaternion.identity;
    }

    void Patrol()
    {
        Transform targetPoint = patrolPoints[currentPointIndex];

        // Usamos Vector2 para ignorar problemas de altura/profundidad
        Vector2 miPos = transform.position;
        Vector2 destino = targetPoint.position;

        // Moverse
        transform.position = Vector2.MoveTowards(miPos, destino, speed * Time.deltaTime);

        // Girar (Flip)
        Vector3 scale = transform.localScale;
        // Nos aseguramos que el scale sea positivo en Y y Z siempre
        scale.y = Mathf.Abs(scale.y);
        scale.z = Mathf.Abs(scale.z);

        if (Vector2.Distance(miPos, destino) > 0.1f) // Solo girar si nos movemos
        {
            if (destino.x > miPos.x) // Va a la derecha
                scale.x = spriteMiraIzquierda ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            else // Va a la izquierda
                scale.x = spriteMiraIzquierda ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);

            transform.localScale = scale;
        }

        // Cambiar de punto si está cerca
        if (Vector2.Distance(miPos, destino) < 0.2f)
        {
            currentPointIndex++;
            if (currentPointIndex >= patrolPoints.Length) currentPointIndex = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. ¿Es el jugador?
        if (collision.gameObject.CompareTag("Jugador"))
        {

            // Usamos -0.5f para ser generosos (permite golpear un poco en diagonal)
            if (collision.contacts[0].normal.y < -0.5f)
            {
                Debug.Log("¡Abeja aplastada! (Detectado por normal)");

                // Hacemos rebotar al jugador
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 8f); // Salto un poco más alto
                }

                // Destruimos la abeja
                Destroy(gameObject);
            }
            else
            {

                // Si no fue desde arriba, dañamos al jugador
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(1, transform);
                }
            }
        }
    }
}

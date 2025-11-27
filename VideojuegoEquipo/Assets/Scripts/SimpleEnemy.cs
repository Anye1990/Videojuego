using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip deathSound; // Asigna 'sfx_bump' o 'sfx_disappear'

    // Esta función la llamarías cuando el jugador le salta encima o le dispara
    public void Die()
    {
        // 1. Reproducir sonido en el lugar donde murió el enemigo
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // 2. Efecto de partículas (Opcional, si tienes)
        // Instantiate(deathEffect, transform.position, Quaternion.identity);

        // 3. Destruir al enemigo
        Destroy(gameObject);
    }

    // Ejemplo de colisión (Si el jugador salta sobre él)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
        {
            // Detectar si el jugador cae desde arriba (Stomp)
            // Nota: Esto es una lógica básica, puede requerir ajustes según tu física
            if (collision.transform.position.y > transform.position.y + 0.5f)
            {
                Die(); // Matar enemigo

                // Impulsar al jugador un poco hacia arriba (rebote)
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null) playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 5f);
            }
            else
            {
                // Si choca de lado, dañar al jugador
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null) player.TakeDamage(1);
            }
        }
    }
}
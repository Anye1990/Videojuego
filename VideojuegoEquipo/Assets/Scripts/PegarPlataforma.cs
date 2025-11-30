using UnityEngine;

public class PegarPlataforma : MonoBehaviour
{
    // Cuando el personaje toca la plataforma
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos si es el Jugador
        if (collision.gameObject.CompareTag("Jugador"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    // Cuando el personaje salta o se baja de la plataforma
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!gameObject.activeInHierarchy) return;

        if (collision.gameObject.CompareTag("Jugador"))
        {
            // Solo quitamos el padre si el jugador sigue teniendo a ESTA plataforma como padre
            // (Evita bugs si el jugador saltó muy rápido a otra plataforma)
            if (collision.transform.parent == this.transform)
            {
                collision.transform.SetParent(null);
            }
        }
    }
}
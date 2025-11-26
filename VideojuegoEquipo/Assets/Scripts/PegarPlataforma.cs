using UnityEngine;

public class PegarPlataforma : MonoBehaviour
{
    // Cuando el personaje toca la plataforma
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos si es el Jugador por su etiqueta
        if (collision.gameObject.CompareTag("Jugador"))
        {
            // Hacemos que el padre del jugador sea esta plataforma
            collision.transform.SetParent(this.transform);
        }
    }

    // Cuando el personaje salta o se baja de la plataforma
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
        {
            // Le quitamos el padre para que vuelva a ser libre
            collision.transform.SetParent(null);
        }
    }
}

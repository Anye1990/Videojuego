using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public enum ItemType { Moneda, Gema, Estrella } // Tipos para organizarnos
    public ItemType tipoDeObjeto;

    [Tooltip("Cuántos puntos da este objeto")]
    public int valorEnPuntos = 10;

    [Tooltip("Sonido al recoger (Opcional)")]
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador"))
        {
            // 1. Sumar puntos y contar el objeto en el GameManager
            GameManager.instance.CollectObject(valorEnPuntos);

            // 2. Reproducir sonido (si tienes un AudioSource en el objeto o usas PlayClipAtPoint)
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // 3. Destruir el objeto
            Destroy(gameObject);
        }
    }
}
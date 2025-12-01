using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public enum ItemType { Moneda, Gema, Estrella }
    public ItemType tipoDeObjeto;
    public int valorEnPuntos = 10;
    public AudioClip collectSound;

    [Header("Visual")]
    public GameObject floatingTextPrefab; // Asigna aquí tu Prefab de texto

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador"))
        {
            GameManager.instance.CollectObject(valorEnPuntos);

            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // TEXTO FLOTANTE
            if (floatingTextPrefab != null)
            {
                Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
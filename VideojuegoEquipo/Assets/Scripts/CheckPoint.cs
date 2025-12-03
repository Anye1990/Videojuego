using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Configuración")]
    public Sprite activeSprite; // Sprite cuando se activa (opcional)
    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador") && !isActivated)
        {
            isActivated = true;

            // Guardamos la posición del checkpoint en el GameManager
            GameManager.instance.ActivateCheckpoint(transform.position);

            // Cambio visual (opcional)
            if (activeSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = activeSprite;
            }

            Debug.Log("¡Checkpoint alcanzado!");
        }
    }
}
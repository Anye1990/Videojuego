using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Imágenes")]
    public Sprite flagOpen;   // Arrastra aquí la imagen de la bandera VERDE/ABIARTA
    public Sprite flagClosed; // Arrastra aquí la imagen de la bandera ROJA/CERRADA (Opcional)

    private SpriteRenderer rend; // Lo hacemos privado para que se configure solo
    private bool isActivated = false;

    private void Start()
    {
        // 1. Buscar automáticamente el componente que dibuja el sprite
        rend = GetComponent<SpriteRenderer>();

        // 2. Si no tienes imagen asignada, intenta poner la cerrada por defecto
        if (rend != null && flagClosed != null)
        {
            rend.sprite = flagClosed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Jugador") && !isActivated)
        {
            isActivated = true;

            // Guardar posición en GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdateCheckpoint(transform.position);
            }

            // Cambiar imagen a bandera abierta
            if (rend != null && flagOpen != null)
            {
                rend.sprite = flagOpen;
            }

            Debug.Log("¡Checkpoint Activado!");
        }
    }
}
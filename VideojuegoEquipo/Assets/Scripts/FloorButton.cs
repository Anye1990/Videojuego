using UnityEngine;

public class FloorButton : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject objectToReveal; // Arrastra aquí la Llave (que está desactivada)
    public Sprite pressedSprite;      // Opcional: Imagen del botón aplastado

    [Header("Configuración")]
    public bool isOneTimeUse = true;  // ¿Se puede usar solo una vez?

    private bool isPressed = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si es el Jugador y si el botón no ha sido presionado ya
        if (other.CompareTag("Jugador") && !isPressed)
        {
            PressButton();
        }
    }

    void PressButton()
    {
        isPressed = true;
        Debug.Log("¡Botón activado! Aparece la llave.");

        // 1. Activamos la llave (se vuelve visible y recolectable)
        if (objectToReveal != null)
        {
            objectToReveal.SetActive(true); //
        }

        // 2. Cambio visual (si tienes el sprite de botón bajado)
        if (pressedSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = pressedSprite;
        }

        // 3. (Opcional) Aquí podrías reproducir un sonido:
        // GetComponent<AudioSource>().Play();
    }

    // Opcional: Si quieres que el botón suba al salir (si no es de un solo uso)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isOneTimeUse && other.CompareTag("Jugador"))
        {
            // Aquí pondrías la lógica para desactivarlo si quisieras
        }
    }
}
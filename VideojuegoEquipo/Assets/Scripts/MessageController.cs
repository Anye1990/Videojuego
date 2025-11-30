using UnityEngine;
using TMPro; // Necesario para el texto
using UnityEngine.SceneManagement; // Necesario para cambiar escenas
using System; // Necesario para las Acciones

public class MessageController : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject messagePanel; // Arrastra aquí tu Panel completo
    public TextMeshProUGUI messageText; // Arrastra aquí el texto del mensaje

    // Guardamos una acción para saber qué hacer cuando se pulse el botón
    private Action onCloseAction;

    void Start()
    {
        // Nos aseguramos de que esté cerrado al iniciar por si acaso
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    // Función para llamar desde otros scripts
    public void ShowMessage(string text, Action onClosing = null)
    {
        if (messagePanel == null) return;

        // 1. Poner el texto
        messageText.text = text;

        // 2. Guardar qué hacer al cerrar (si es null, solo cerrará)
        onCloseAction = onClosing;

        // 3. Mostrar panel y PAUSAR el juego
        messagePanel.SetActive(true);
        Time.timeScale = 0f; // El tiempo se detiene (nadie se mueve)
    }

    // Esta función se conecta al botón "OK" del UI
    public void CloseMessage()
    {
        // 1. Reanudar el tiempo
        Time.timeScale = 1f;

        // 2. Ocultar panel
        messagePanel.SetActive(false);

        // 3. Ejecutar la acción pendiente (ej. ir al menú) si existe
        if (onCloseAction != null)
        {
            onCloseAction.Invoke();
            onCloseAction = null; // Limpiar
        }
    }
}

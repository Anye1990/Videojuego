using UnityEngine;

public class LevelStartInfo : MonoBehaviour
{
    [TextArea]
    public string mensajeDelNivel;

    void Start()
    {
        // Buscamos el controlador y le mandamos el mensaje
        MessageController msg = Object.FindAnyObjectByType<MessageController>();
        if (msg != null)
        {
            // Solo mostramos el mensaje, sin acción especial al cerrar
            msg.ShowMessage(mensajeDelNivel);
        }
    }
}
using UnityEngine;

public class LevelStartInfo : MonoBehaviour
{
    [TextArea]
    public string mensajeDelNivel;

    // Arrastra aquí el objeto "Controlador_Inicio" en el Inspector
    public MessageController controladorInicio;

    void Start()
    {
        if (controladorInicio != null)
        {
            controladorInicio.ShowMessage(mensajeDelNivel);
        }
    }
}
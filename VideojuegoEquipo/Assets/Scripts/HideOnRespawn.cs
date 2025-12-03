using UnityEngine;

public class HideOnRespawn : MonoBehaviour
{
    void Start()
    {
        // Preguntamos al GameManager: "¿Vengo de un Checkpoint?"
        if (GameManager.instance != null && GameManager.instance.checkpointActive)
        {
            // Si es verdad, me desactivo inmediatamente
            gameObject.SetActive(false);
            Debug.Log("Panel ocultado por Respawn");
        }
    }
}
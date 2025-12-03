using UnityEngine;

public class HideOnRespawn : MonoBehaviour
{
    void Start()
    {
        // Preguntamos al GameManager: "¿Vengo de un Checkpoint?"
        if (GameManager.instance != null && GameManager.instance.checkpointActive)
        {
            // --- SOLUCIÓN AL CONGELAMIENTO ---
            // Forzamos que el tiempo corra, por si el panel lo había pausado al nacer
            Time.timeScale = 1f;

            // Nos desactivamos
            gameObject.SetActive(false);
            Debug.Log("Panel ocultado por Respawn y tiempo reanudado.");
        }
    }
}
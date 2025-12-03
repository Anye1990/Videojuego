using UnityEngine;
// (No necesitas using SceneManager aquí si usas el LevelTransition)

public class LevelExit : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador"))
        {
            if (GameManager.instance.CanPassLevel())
            {
                Debug.Log("¡Nivel Completado!");

                // --- IMPORTANTE ---
                // Reseteamos el checkpoint para que en el siguiente nivel empecemos en el inicio
                GameManager.instance.ResetCheckpointData();

                LevelTransition.instance.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("¡No puedes pasar! Te faltan objetos o puntos.");
            }
        }
    }
}
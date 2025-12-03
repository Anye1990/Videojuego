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

                // --- CAMBIO AQUÍ ---
                // En lugar de SceneManager.LoadScene, usamos:
                LevelTransition.instance.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("¡No puedes pasar! Te faltan objetos o puntos.");
            }
        }
    }
}
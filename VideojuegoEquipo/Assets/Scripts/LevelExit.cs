using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextSceneName; // Nombre de la escena del siguiente nivel

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador"))
        {
            // Preguntamos al GameManager si cumplimos los requisitos
            if (GameManager.instance.CanPassLevel())
            {
                Debug.Log("¡Nivel Completado!");
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("¡No puedes pasar! Te faltan objetos o puntos.");
                // Aquí podrías mostrar un mensaje en pantalla avisando al jugador
            }
        }
    }
}
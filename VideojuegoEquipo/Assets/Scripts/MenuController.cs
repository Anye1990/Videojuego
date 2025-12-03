using UnityEngine;
using UnityEngine.SceneManagement; // Necesario por si falla la transición

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // Verificamos si el sistema de transición existe antes de usarlo
        if (LevelTransition.instance != null)
        {
            LevelTransition.instance.LoadScene("CharacterSelect");
        }
        else
        {
            Debug.LogWarning("No se encontró 'LevelTransition'. Cargando escena de golpe.");
            // Carga de seguridad por si olvidaste poner el objeto en la escena
            SceneManager.LoadScene("CharacterSelect");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
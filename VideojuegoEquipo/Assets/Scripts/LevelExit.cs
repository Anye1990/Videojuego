using UnityEngine;

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
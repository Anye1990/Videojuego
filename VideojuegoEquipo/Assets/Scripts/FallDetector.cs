using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena

public class FallDetector : MonoBehaviour
{
    // Opcional: Sonido al caer
    public AudioClip fallSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si lo que entra en la zona es el Jugador
        if (other.CompareTag("Jugador"))
        {
            Debug.Log("¡El jugador cayó al vacío!");

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1); // Quita 1 vida
                // Aquí deberías mover al jugador a un "Respawn Point" (CheckPoint)
                other.transform.position = new Vector3(0, 0, 0);
            }

        }
        // Si caen enemigos u objetos, los destruimos para limpiar memoria
        else if (other.gameObject.layer != LayerMask.NameToLayer("Ground"))
        {
            Destroy(other.gameObject);
        }
    }

    void RestartLevel()
    {
        // Obtiene el nombre de la escena actual y la vuelve a cargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
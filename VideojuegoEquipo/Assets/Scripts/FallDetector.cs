using UnityEngine;

public class FallDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador"))
        {
            // Llamamos al GameManager para que reste una vida
            GameManager.instance.LoseLife();
        }
        else if (other.gameObject.layer != LayerMask.NameToLayer("Ground"))
        {
            Destroy(other.gameObject);
        }
    }
}
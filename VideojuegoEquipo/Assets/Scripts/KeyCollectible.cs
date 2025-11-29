using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador"))
        {
            // Avisar al GameManager
            GameManager.instance.CollectKey();


            Destroy(gameObject);
        }
    }
}
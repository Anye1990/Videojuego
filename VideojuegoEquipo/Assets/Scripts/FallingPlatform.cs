using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.5f;
    private Rigidbody2D rb;
    private bool fallTriggered = false;

    void Start() { rb = GetComponent<Rigidbody2D>(); }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!fallTriggered && collision.gameObject.CompareTag("Jugador"))
        {
            fallTriggered = true;
            Invoke("Fall", fallDelay);
        }
    }

    void Fall() { rb.bodyType = RigidbodyType2D.Dynamic; }
}
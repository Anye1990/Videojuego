using UnityEngine;

public class AutoScroll : MonoBehaviour
{
    public float speed = 0.5f;
    public float resetPosition = 20f; // Ajusta esto según el ancho de tu imagen
    public float startPosition = -20f;

    void Update()
    {
        // Mueve el objeto a la izquierda
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Si se sale de la pantalla, vuelve al inicio (bucle)
        if (transform.position.x < startPosition)
        {
            transform.position = new Vector3(resetPosition, transform.position.y, transform.position.z);
        }
    }
}
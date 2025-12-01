using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1f; // Bajamos la velocidad para que se lea mejor
    public float destroyTime = 1f;

    // CAMBIO AQUÍ: Ponemos '0' en la Y para que nazca justo en el centro de la moneda, no arriba.
    public Vector3 offset = new Vector3(0, 0, 0);

    void Start()
    {
        // Aplicar el desplazamiento inicial (ahora es 0, así que sale desde la moneda)
        transform.localPosition += offset;

        // Destruir después de un tiempo
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // Moverse suavemente hacia arriba
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }
}
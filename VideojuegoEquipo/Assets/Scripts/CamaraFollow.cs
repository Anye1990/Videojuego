using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    public Transform target; // Arrastra aquí a tu Personaje
    public Vector3 offset = new Vector3(0, 2, -10); // Ajuste de posición (X, Y, Z)
    public float smoothSpeed = 0.125f; // Qué tan suave sigue la cámara (0 a 1)

    // Límites opcionales para que la cámara no se salga del nivel
    public bool enableLimits;
    public Vector2 minLimits;
    public Vector2 maxLimits;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculamos la posición deseada
        Vector3 desiredPosition = target.position + offset;

        // Suavizamos el movimiento usando Lerp (Interpolación Lineal)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplicamos la posición
        transform.position = smoothedPosition;

        // (Opcional) Si activas los límites, la cámara no pasará de estas coordenadas
        if (enableLimits)
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minLimits.x, maxLimits.x),
                Mathf.Clamp(transform.position.y, minLimits.y, maxLimits.y),
                transform.position.z
            );
        }
    }
}
using UnityEngine;

public class ParallaxEfect : MonoBehaviour
{
    public Camera cam;
    public float parallaxFactor; // 0 = se mueve igual que la cámara (estático), 1 = no se mueve nada

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        lastCameraPosition = cam.transform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cam.transform.position - lastCameraPosition;

        // Movemos el fondo basado en cuánto se movió la cámara multiplicado por el factor
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * parallaxFactor, 0);

        lastCameraPosition = cam.transform.position;
    }
}
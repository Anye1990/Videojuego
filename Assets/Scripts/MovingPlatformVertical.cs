using UnityEngine;

public class MovingPlatformVertical : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 3f;
    private Vector3 startPos;

    void Start() { startPos = transform.position; }

    void Update()
    {
        transform.position = startPos +
            Vector3.up * Mathf.Sin(Time.time * speed) * distance;
    }
}
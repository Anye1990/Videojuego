using UnityEngine;

public class MovingPlatformHorizontal : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 3f;
    private Vector3 startPos;

    void Start() { startPos = transform.position; }

    void Update()
    {
        transform.position = startPos +
            Vector3.right * Mathf.Sin(Time.time * speed) * distance;
    }
}
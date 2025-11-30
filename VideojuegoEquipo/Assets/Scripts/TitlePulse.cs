using UnityEngine;
using TMPro;

public class TitlePulse : MonoBehaviour
{
    public float speed = 2f;
    public float amount = 0.1f;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Hace que el texto crezca y se encoja suavemente (efecto latido)
        float scale = 1 + Mathf.Sin(Time.time * speed) * amount;
        transform.localScale = originalScale * scale;
    }
}
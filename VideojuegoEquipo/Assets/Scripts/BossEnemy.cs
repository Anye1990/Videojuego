using UnityEngine;
using System.Collections;

[DisallowMultipleComponent] 
public class BossEnemy : MonoBehaviour
{
    [Header("Configuracion de Vida")]
    public int maxHealth = 5;
    [SerializeField] private int currentHealth; // Visible en inspector para pruebas

    [Header("Invulnerabilidad")]
    public float invulnerabilityTime = 2.0f;
    private float lastHitTime; // Control exacto de tiempo

    [Header("Fisicas de Rebote")]
    public float bounceUpForce = 12f;
    public float bounceBackForce = 8f; // Aumentado para alejarte mas

    [Header("Feedback Visual")]
    public Color damageColor = Color.red;

    private SpriteRenderer[] allRenderers;
    private Color[] originalColors;

    void Start()
    {
        // IMPORTANTE: Forzar la vida al maximo al iniciar
        currentHealth = maxHealth;

        allRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            originalColors[i] = allRenderers[i].color;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jugador"))
        {
            ContactPoint2D contacto = collision.GetContact(0);

            // Golpe desde arriba
            if (contacto.normal.y < -0.5f)
            {
                // 1. EMPUJAR AL JUGADOR (Usando la nueva funcion Rebote)
                EmpujarJugador(collision.gameObject);

                // 2. CHEQUEO DE SEGURIDAD POR TIEMPO
                // Si ha pasado poco tiempo desde el ultimo golpe, ignoramos este choque
                if (Time.time < lastHitTime + invulnerabilityTime) return;

                // Si pasa el chequeo, aplicamos daño
                TakeDamage();
            }
            else
            {
                // Golpe lateral: Daño al jugador
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(1, transform);
                }
            }
        }
    }

    void EmpujarJugador(GameObject playerObj)
    {
        PlayerController player = playerObj.GetComponent<PlayerController>();
        if (player != null)
        {
            // Calcular direccion opuesta al boss
            float directionX = (playerObj.transform.position.x < transform.position.x) ? -1 : 1;

            // Vector de fuerza final
            Vector2 fuerza = new Vector2(directionX * bounceBackForce, bounceUpForce);

            // Llamamos a la nueva funcion del jugador que bloquea el input
            player.Rebote(fuerza);
        }
    }

    public void TakeDamage()
    {
        // Actualizamos el tiempo del golpe
        lastHitTime = Time.time;

        currentHealth--;
        Debug.Log("Boss Herido! Vida: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(VisualFeedbackRoutine());
        }
    }

    IEnumerator VisualFeedbackRoutine()
    {
        // CAMBIAR TODOS A ROJO
        foreach (SpriteRenderer sr in allRenderers)
        {
            sr.color = damageColor;
        }

        yield return new WaitForSeconds(invulnerabilityTime);

        // RESTAURAR COLORES ORIGINALES
        for (int i = 0; i < allRenderers.Length; i++)
        {
            allRenderers[i].color = originalColors[i];
        }
    }

    void Die()
    {
        Debug.Log("¡Boss Derrotado!");
        Destroy(gameObject);
    }
}
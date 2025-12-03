using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class BossController : MonoBehaviour
{
    [Header("Configuración Boss")]
    public Transform player;
    public float speed = 2.0f;
    public float attackRange = 5.0f;
    public float stopDistance = 3.0f;
    public int maxHealth = 5;
    [SerializeField] private int currentHealth;

    [Header("Combate (Disparos)")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackCooldown = 2.0f;
    private float lastAttackTime;

    [Header("Defensa (Rebote e Invulnerabilidad)")]
    public float bounceUpForce = 12f;
    public float bounceBackForce = 8f;
    public float invulnerabilityTime = 2.0f;
    private bool isInvulnerable = false;

    [Header("Visual")]
    public Animator animator;
    public Color damageColor = Color.red;
    private SpriteRenderer[] allRenderers;
    private Color[] originalColors;

    private bool isDead = false;
    private bool isFacingRight = true;
    private HUDController hud;
    public float waitAfterDeath = 3.0f;

    [Header("Límites de Vuelo")]
    public float alturaMaxima = 2.0f;
    public bool usarLimiteAltura = true;

    void Start()
    {
        currentHealth = maxHealth;

        // 1. Buscar HUD y Jugador
        hud = Object.FindAnyObjectByType<HUDController>();
        if (hud != null) hud.UpdateBossHealth(currentHealth, maxHealth);

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Jugador");
            if (p != null) player = p.transform;
        }

        // 2. Configurar Colores
        allRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            originalColors[i] = allRenderers[i].color;
        }
    }

    void Update()
    {
        if (player == null || isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 1. Mirar al jugador
        LookAtPlayer();

        // 2. Movimiento
        if (distance < attackRange)
        {
            if (distance > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }

            // 3. Disparar
            if (Time.time > lastAttackTime + attackCooldown)
            {
                Shoot();
            }
        }

        if (usarLimiteAltura)
        {
            if (transform.position.y > alturaMaxima)
            {
                transform.position = new Vector3(transform.position.x, alturaMaxima, transform.position.z);
            }
        }
    }

    // --- LÓGICA DE COLISIÓN Y REBOTE ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Jugador"))
        {
            // Detectar golpe desde ARRIBA
            if (collision.contacts[0].normal.y < -0.5f)
            {
                // 1. SIEMPRE EMPUJAR AL JUGADOR
                EmpujarJugador(collision.gameObject);

                // 2. SOLO DAÑAR SI NO ES INVULNERABLE
                if (!isInvulnerable)
                {
                    TakeDamage(1);
                }
            }
            else
            {
                // Golpe lateral: Dañar al jugador
                PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(1); // Simplificado, asumiendo que el GameManager maneja las vidas
                }
            }
        }
    }

    void EmpujarJugador(GameObject playerObj)
    {
        PlayerController pc = playerObj.GetComponent<PlayerController>();
        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float directionX = (playerObj.transform.position.x < transform.position.x) ? -1 : 1;
            Vector2 fuerza = new Vector2(directionX * bounceBackForce, bounceUpForce);

            if (pc != null)
            {
                // Usamos la función de rebote del jugador si existe (opcional)
                // pc.Rebote(fuerza); 
                // Si no tienes pc.Rebote, usa la física directa:
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(fuerza, ForceMode2D.Impulse);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(fuerza, ForceMode2D.Impulse);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damage;

        if (hud != null) hud.UpdateBossHealth(currentHealth, maxHealth);
        if (animator) animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        foreach (var sr in allRenderers) sr.color = damageColor;
        yield return new WaitForSeconds(invulnerabilityTime);
        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (allRenderers[i] != null) allRenderers[i].color = originalColors[i];
        }
        isInvulnerable = false;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("¡Jefe derrotado!");

        if (animator) animator.SetTrigger("Dying");
        if (hud != null) hud.HideBossUI();

        // Desactivar físicas para que no estorbe
        GetComponent<Collider2D>().enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero; // Detenerlo en seco

        // --- CAMBIO PRINCIPAL: ---
        // Ya no llamamos al MessageController.
        // Vamos directamente a la secuencia de salida.
        StartCoroutine(GoToMenuSequence());
    }

    IEnumerator GoToMenuSequence()
    {
        // 1. Espera dramática (explosiones, sonido final, etc.)
        yield return new WaitForSeconds(waitAfterDeath);

        // 2. Resetear Checkpoints (IMPORTANTE para la próxima partida)
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetCheckpointData();
        }

        // 3. Cargar Menú con transición
        if (LevelTransition.instance != null)
        {
            LevelTransition.instance.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("No se encontró LevelTransition, cargando normal.");
            SceneManager.LoadScene("MainMenu");
        }

        // (Opcional) Destruir el boss por limpieza, aunque al cambiar de escena se destruye solo.
        Destroy(gameObject, 0.5f);
    }

    void LookAtPlayer()
    {
        if (player.position.x > transform.position.x && !isFacingRight) Flip();
        else if (player.position.x < transform.position.x && isFacingRight) Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    void Shoot()
    {
        // Aviso para saber si el código intenta disparar
        Debug.Log("¡Intento de disparo en: " + Time.time + "!");

        lastAttackTime = Time.time;
        if (animator) animator.SetTrigger("Attack");

        if (projectilePrefab != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogError("¡Falta asignar el Projectile Prefab en el Inspector!");
        }
    }
}
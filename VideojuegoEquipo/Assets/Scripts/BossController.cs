using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent] // Seguridad para no ponerlo 2 veces
public class BossController : MonoBehaviour
{
    [Header("Configuración Boss")]
    public Transform player;
    public float speed = 2.0f;
    public float attackRange = 5.0f;
    public float stopDistance = 3.0f;
    public int maxHealth = 5;
    [SerializeField] private int currentHealth; // Visible para debug

    [Header("Combate (Disparos)")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackCooldown = 2.0f;
    private float lastAttackTime;

    [Header("Defensa (Rebote e Invulnerabilidad)")]
    public float bounceUpForce = 12f;    // Cuánto salta el jugador al golpearle
    public float bounceBackForce = 8f;   // Cuánto se aleja el jugador
    public float invulnerabilityTime = 2.0f; // Tiempo que se queda rojo
    private bool isInvulnerable = false;

    [Header("Visual")]
    public Animator animator;
    public Color damageColor = Color.red; // Color al recibir daño
    private SpriteRenderer[] allRenderers;
    private Color[] originalColors;

    private bool isDead = false;
    private bool isFacingRight = true;
    private HUDController hud;

    [Header("Límites de Vuelo")]
    public float alturaMaxima = 2.0f; // Ajusta este valor en el Inspector
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

        // 2. Configurar Colores para todas las partes del cuerpo
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
            // Si el enemigo sube más de la altura permitida, lo forzamos a bajar
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
                // 1. SIEMPRE EMPUJAR AL JUGADOR (Para que no se quede pegado)
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
                    playerScript.TakeDamage(1, transform); // Pasamos transform para el knockback
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
            // Calcular dirección para alejarlo (izquierda o derecha)
            float directionX = (playerObj.transform.position.x < transform.position.x) ? -1 : 1;
            Vector2 fuerza = new Vector2(directionX * bounceBackForce, bounceUpForce);

            // Si el jugador tiene el script nuevo con la función Rebote(), la usamos
            if (pc != null)
            {
                // INTENTA USAR LA FUNCION NUEVA QUE TE DI ANTES
                // Si te da error aquí es porque no actualizaste PlayerController.cs
                // Si no la tienes, descomenta la linea de abajo y borra la de pc.Rebote

                pc.Rebote(fuerza);
                // rb.linearVelocity = Vector2.zero; rb.AddForce(fuerza, ForceMode2D.Impulse);
            }
            else
            {
                // Fallback por si acaso
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(fuerza, ForceMode2D.Impulse);
            }
        }
    }
    // -----------------------------------

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damage;

        // Actualizar HUD
        if (hud != null) hud.UpdateBossHealth(currentHealth, maxHealth);

        // Animación
        if (animator) animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Activar Invulnerabilidad
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        // Poner todo rojo
        foreach (var sr in allRenderers) sr.color = damageColor;

        yield return new WaitForSeconds(invulnerabilityTime);

        // Volver a color normal
        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (allRenderers[i] != null) allRenderers[i].color = originalColors[i];
        }

        isInvulnerable = false;
    }

    void Die()
    {
        isDead = true;
        if (animator) animator.SetTrigger("Dying");
        if (hud != null) hud.HideBossUI();

        // Desactivar físicas del boss para que caiga o no moleste
        GetComponent<Collider2D>().enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.gravityScale = 1; // Cae al vacío si quieres

        MessageController msg = Object.FindAnyObjectByType<MessageController>();
        if (msg != null) StartCoroutine(ShowWinMessageRoutine(msg));
        else Destroy(gameObject, 3f);
    }

    IEnumerator ShowWinMessageRoutine(MessageController msg)
    {
        yield return new WaitForSeconds(2.0f);
        msg.ShowMessage("¡HAS GANADO!\nDerrotaste al Boss final.", () =>
        {
            SceneManager.LoadScene("MainMenu");
        });
        Destroy(gameObject);
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
        lastAttackTime = Time.time;
        if (animator) animator.SetTrigger("Attack");

        if (projectilePrefab != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        }
    }
}

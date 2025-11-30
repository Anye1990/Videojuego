using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [Header("Configuración Boss")]
    public Transform player;           // Arrastra al Jugador aquí
    public float speed = 2.0f;         // Velocidad de movimiento
    public float attackRange = 5.0f;   // Distancia para empezar a disparar
    public float stopDistance = 3.0f;  // Distancia para detenerse y disparar
    public int maxHealth = 5;

    [Header("Combate")]
    public GameObject projectilePrefab; // Arrastra aquí el Prefab de tu proyectil
    public Transform firePoint;         // Punto desde donde sale el disparo (opcional, usa transform si no tienes)
    public float attackCooldown = 2.0f; // Tiempo entre disparos

    [Header("Animaciones")]
    public Animator animator;

    private int currentHealth;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isFacingRight = true;

    [Header("Configuración Boss")]


    // Referencia al HUD
    private HUDController hud;

    void Start()
    {
        currentHealth = maxHealth;

        // Buscar el HUD en la escena
        hud = Object.FindAnyObjectByType<HUDController>();

        // Actualizar la UI al iniciar para que se vean los 5 corazones llenos
        if (hud != null)
        {
            hud.UpdateBossHealth(currentHealth, maxHealth);
        }


        // Buscar al jugador si no se asignó
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Jugador");
            if (p != null) player = p.transform;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Animación de daño
        if (animator) animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }

        currentHealth -= damage;

        // ACTUALIZAR HUD AQUÍ
        if (hud != null)
        {
            hud.UpdateBossHealth(currentHealth, maxHealth);
        }

        if (animator) animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        isDead = true;
        if (animator) animator.SetTrigger("Dying");

        if (hud != null) hud.HideBossUI();

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 1;

        MessageController msg = Object.FindAnyObjectByType<MessageController>();

        if (msg != null)
        {
            StartCoroutine(ShowWinMessageRoutine(msg));
        }
        else
        {
            // Si no hay sistema de mensajes, destruimos normal (seguridad)
            Destroy(gameObject, 3f);
        }
    }

    // Añade esta función al final de tu script BossController
    System.Collections.IEnumerator ShowWinMessageRoutine(MessageController msg)
    {
        yield return new WaitForSeconds(2.0f); // Espera 2 seg para ver al boss caer

        // Muestra el mensaje y define qué pasa al darle OK (Ir al MainMenu)
        msg.ShowMessage("¡HAS GANADO!\nDerrotaste al Boss final.", () =>
        {
            // Esto se ejecuta al pulsar el botón OK
            SceneManager.LoadScene("MainMenu");
        });

        Destroy(gameObject); // Ya podemos borrar al boss
    }

    void Update()
    {
        if (player == null || isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 1. Mirar siempre al jugador
        LookAtPlayer();

        // 2. Movimiento y Ataque
        if (distance < attackRange)
        {
            if (distance > stopDistance)
            {
                // Moverse hacia el jugador si está lejos
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }

            // Disparar si pasó el tiempo
            if (Time.time > lastAttackTime + attackCooldown)
            {
                Shoot();
            }
        }
    }

    void LookAtPlayer()
    {
        // Girar el sprite dependiendo de dónde esté el jugador
        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
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

        // Usamos "Attack" si tienes esa animación, si no, el Boss dispara igual
        if (animator) animator.SetTrigger("Attack");

        // Crear el proyectil
        if (projectilePrefab != null)
        {
            // Si tienes un objeto 'firePoint' úsalo, si no, usa la posición del boss
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        }
    }

    // Lógica de colisión (Saltar sobre él)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Jugador"))
        {
            // Detectar si el golpe viene de ARRIBA (Stomp)
            if (collision.contacts[0].normal.y < -0.5f)
            {
                TakeDamage(1); // Dañar al Boss

                // Impulsar al jugador hacia arriba
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null) playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 8f);
            }
            else
            {
                // Si chocan de lado, el jugador recibe daño
                PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(1);
                }
            }
        }
    }
}
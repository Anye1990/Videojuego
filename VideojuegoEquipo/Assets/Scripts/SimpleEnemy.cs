using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public float speed = 2.0f;
    public int damage = 1; // Bajamos el daño a 1 para probar (o usa 10 si prefieres)
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        // Si no hay puntos asignados, no hacemos nada para evitar errores
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];

        // Moverse hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // GIRAR AL ENEMIGO (FLIP)
        // Si el destino está a la derecha, miramos a la derecha (escala 1)
        if (targetPoint.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        // Si el destino está a la izquierda, miramos a la izquierda (escala -1)
        else if (targetPoint.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);

        // Si llegamos al punto (distancia muy corta)
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            // Pasamos al siguiente punto de la lista (0 -> 1 -> 0 ...)
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // Usamos OnTriggerEnter2D para juegos 2D
    {
        // IMPORTANTE: Verifica que tu personaje tenga la etiqueta "Player"
        if (other.CompareTag("Player"))
        {
            // Busca el script del jugador para hacerle daño
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
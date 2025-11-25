using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public float speed = 2.0f;
    public int damage = 10;
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }
}
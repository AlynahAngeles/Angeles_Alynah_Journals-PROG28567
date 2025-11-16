using UnityEngine;

public class ClosestPoint : MonoBehaviour
{
public Rigidbody2D enemy;  

private Rigidbody2D rb;

void Start()
{
    rb = GetComponent<Rigidbody2D>();
}

    void Update()
    { 

        Vector2 playerPoint = rb.ClosestPoint(enemy.position);
        Vector2 enemyPoint = enemy.ClosestPoint(rb.position);

        Debug.DrawLine(playerPoint, enemyPoint, Color.red);

        float distance = Vector2.Distance(playerPoint, enemyPoint);
    }
}
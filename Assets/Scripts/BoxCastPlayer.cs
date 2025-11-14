using UnityEngine;

public class BoxCastPlayer : MonoBehaviour
{
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;   

    private Rigidbody2D rb;
    private BoxCollider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            col.bounds.center,      
            col.bounds.size,        
            0f,                     
            Vector2.down,          
            groundCheckDistance,    
            groundLayer            
        );

        return hit.collider != null;
    }
}
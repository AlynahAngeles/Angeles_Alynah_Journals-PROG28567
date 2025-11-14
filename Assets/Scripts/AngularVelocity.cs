using UnityEngine;

public class AngularVelocity : MonoBehaviour
{
    public float spinSpeed = 180f;
    private Rigidbody2D rb;  

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  
    }

    void Update()
    {
        rb.angularVelocity = spinSpeed;  
    }
}
using UnityEngine;

public class AngularVelocity : MonoBehaviour
{
    public float spinSpeed = 180f;
    private Rigidbody2D rb;
    private bool spin = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }
         
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            rb.angularVelocity = spinSpeed;
            rb.angularDamping = 0f;
            spin = true;
            Debug.Log("Angular Damping is off");
        }

        if(Input.GetKeyDown(KeyCode.B) && spin)
        {
            rb.angularDamping = 10f;
            Debug.Log("Angular Damping is on");
        }
    }
}
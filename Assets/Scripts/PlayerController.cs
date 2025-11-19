using UnityEngine;
using static PlayerController;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D player;
    public float playerSpeed = 3f;
    public float maxSpeed = 6f;

    public FacingDirection facingDirection;

    public float groundCheckDistance = 0.74f;
    public LayerMask groundLayer;

    Vector2 playerInput = new Vector2();

    public enum FacingDirection
    {
        left, right, idle
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        playerInput.x = 0;

        if (Input.GetKey(KeyCode.A))
        {
            playerInput.x = -1;
        }

        else if(Input.GetKey(KeyCode.D))
        {
            playerInput.x = 1;
        }

        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {

        player.AddForce(new Vector2(playerInput.x * playerSpeed, 0f), ForceMode2D.Force);

        if(Mathf.Abs(player.linearVelocity.x) > maxSpeed)
        {
            player.linearVelocity = new Vector2(Mathf.Clamp(player.linearVelocity.x, -maxSpeed, maxSpeed), player.linearVelocity.y);
        }

        player.linearDamping = 7f;
    }

    public bool IsWalking()
    {

        bool walking = Mathf.Abs(player.linearVelocity.x) > 1f;

        if (walking)
        {
            GetFacingDirection();
            return true;
        }

        else
        {
            walking = false;
            return false;
        }
    }

    public bool IsGrounded()
    {

        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(player.position, Vector2.down * groundCheckDistance, Color.red);


        if (hit.collider != null)
        {
            Debug.Log("Player is grounded");
            return true;
        }
        else
        {
            Debug.Log("Player is not grounded");
            return false;
        }
    }

    public FacingDirection GetFacingDirection()
    {
        if (playerInput.x < -0.1)
        {
            facingDirection = FacingDirection.left;

        }

        else if (playerInput.x > 0.1)
        {
            facingDirection = FacingDirection.right;
        }

        return facingDirection;
    }
}

using UnityEngine;
using static PlayerController;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D player;
    public float playerSpeed = 3f;
    public float maxSpeed = 6f;

    public FacingDirection facingDirection;

    public float groundCheckDistance = 0.67f;
    public LayerMask groundLayer;

    public bool jump = false;
    public float terminalSpeed = -100f; //fastest speed of player when falling

    Vector2 playerInput = new Vector2();

    public float jumpHeight = 10f; //apex of jump height
    public float jumpTime = 0.5f; //apex of jump time
    public float jumpForce;
    public float gravity;

    public float fallMultiplier = 1.25f;

    public float coyoteTime = 0.5f;
    public float coyoteCounter;

    public bool isGrounded = true;

    public enum FacingDirection
    {
        left, right, idle
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();

        gravity = (-2 * jumpHeight) / (jumpTime * jumpTime);
        jumpForce = (2 * jumpHeight) / jumpTime;

        player.gravityScale = gravity / Physics2D.gravity.y;

    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded();

        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        playerInput.x = 0;
        playerInput.y = 0;

        if (Input.GetKey(KeyCode.A))
        {
            playerInput.x = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            playerInput.x = 1;
        }

        if (Input.GetKeyDown(KeyCode.W) && coyoteTime > 0f)
        {
            playerInput.y = 1;
        }

        MovementUpdate(playerInput);

        if (jump == true && player.linearVelocity.y < 0)
        {
            AirTime();
        }
    }

    private void MovementUpdate(Vector2 playerInput)
    {

        player.AddForce(new Vector2(playerInput.x * playerSpeed, 0f), ForceMode2D.Force);

        if(Mathf.Abs(player.linearVelocity.x) > maxSpeed)
        {
            player.linearVelocity = new Vector2(Mathf.Clamp(player.linearVelocity.x, -maxSpeed, maxSpeed), player.linearVelocity.y);
            player.linearDamping = 7f;
        }

        if (playerInput.y > 0f && coyoteCounter > 0f)
        {
            Debug.Log("Player jumped!");
            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpForce);
            coyoteCounter = 0f;
               
        }
    }

    public void AirTime()
    {
        Debug.Log("Player is falling");

        if (player.linearVelocity.y < 0)
        {
            player.linearVelocity += Vector2.down * (Mathf.Abs(Physics2D.gravity.y) * fallMultiplier * player.gravityScale * Time.deltaTime);
        }

        if (player.linearVelocity.y < terminalSpeed)
            player.linearVelocity = new Vector2(player.linearVelocity.x, terminalSpeed);

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
            isGrounded = true;
            jump = false;
            return true;
        }
        else
        {
            jump = true;
            isGrounded = false;
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

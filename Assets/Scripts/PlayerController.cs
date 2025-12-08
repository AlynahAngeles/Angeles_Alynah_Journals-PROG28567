using UnityEngine;
//using static PlayerController;
using System.Collections; //added so that I can add IEnumerator for the Coroutine

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D player;
    public float playerSpeed = 3.5f;
    public float maxSpeed = 6f;

    public FacingDirection facingDirection;

    public float groundCheckDistance = 0.67f;
    public LayerMask groundLayer;

    public bool jump = false;
    public float terminalSpeed; //fastest speed of player when falling

    Vector2 playerInput = new Vector2();

    public float jumpHeight = 20f; //apex of jump height
    public float jumpTime = 1.5f; //apex of jump time
    public float jumpForce = 20;
    public float gravity;

    public float fallMultiplier = 2.5f;

    public float coyoteTime = 0.5f;
    public float coyoteCounter;

    public bool isGrounded = true;
    private bool isDash = false; //boolean that changes based on whether the player is dashing or not to avoid spamming dash and properly start coroutine
    public float dashCoolDown = 1.5f;
    public float dashDuration = 0.5f;
    public float dashForce = 30f;
    public bool canDash = true;
    public float dashDirection;

    public float damage;
    public float lunge = 0;
    public bool canAttack = true;
    public float resetTime = 0.2f;
    public bool isLunging = false;
    public PlatformerEnemy enemy;

    public enum FacingDirection
    {
        left, right, idle
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        enemy = FindObjectOfType<PlatformerEnemy>();

        gravity = (-2 * jumpHeight) / (jumpTime * jumpTime);
        jumpForce = (2 * jumpHeight) / jumpTime;

        player.gravityScale = gravity / Physics2D.gravity.y;

    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded();

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

        if (!isDash && !isLunging)
        {

            if (Input.GetKey(KeyCode.A))
            {
                playerInput.x = -3f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                playerInput.x = 3f;
            }

            if (Input.GetKeyDown(KeyCode.W) && coyoteCounter > 0f)
            {
                playerInput.y = 1;
            }

            MovementUpdate(playerInput);
        }

        if (jump == true && player.linearVelocity.y < 0)
        {
            AirTime();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) //if statement for when the player presses left shift and is NOT dashing, then start the coroutine to dash again
        {
            StartCoroutine(Dash());
        }

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            if (isGrounded)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        Slash();
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }

    IEnumerator Dash() //coroutine for dash movement
    {
        Debug.Log("Player dashed.");
        canDash = false;
        isDash = true;

        if (facingDirection == FacingDirection.left && isDash == true)
        {
            dashDirection = -3f;
        }

        else if (facingDirection == FacingDirection.right && isDash == true)
        {
            dashDirection = 3f;
        }

        player.AddForce(new Vector2(dashDirection * dashForce, 0f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashDuration);

        isDash = false;

        yield return new WaitForSeconds(dashCoolDown);

        canDash = true;

    }

    private void MovementUpdate(Vector2 playerInput)
    {

        player.AddForce(new Vector2(playerInput.x * playerSpeed, 0f), ForceMode2D.Force);

        if (Mathf.Abs(player.linearVelocity.x) > maxSpeed)
        {
            player.linearVelocity = new Vector2(Mathf.Clamp(player.linearVelocity.x, -maxSpeed, maxSpeed), player.linearVelocity.y);
            player.linearDamping = 5f;
        }

        if (playerInput.y > 0f && coyoteCounter > 0f)
        {
            Debug.Log("Player jumped!");
            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpForce);
            coyoteCounter = 0f;

        }
    }

    public void Slash()
    {
        int damage = Random.Range(4, 8);
        Debug.Log("Player hit for " + damage + " damage!");

        if (enemy != null && enemy.HPVisible())
        {
            enemy.Damaged(damage);
        }

        if (facingDirection == FacingDirection.left)
        {
            lunge = -2f;
        }

        else if (facingDirection == FacingDirection.right)
        {
            lunge = 2f;
        }

        Vector2 playerVelocity = player.linearVelocity;
        player.linearVelocity = new Vector2(playerVelocity.x + lunge * 20f, playerVelocity.y);

        StartCoroutine(ResetVelocity(0.2f));
    }

    IEnumerator ResetVelocity(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);

        Debug.Log("Player reseting velocity");
        Vector2 resetVel = player.linearVelocity;
        resetVel.x = Mathf.Clamp(resetVel.x, -maxSpeed, maxSpeed);
        player.linearVelocity = new Vector2(resetVel.x, resetVel.y);

        isLunging = false;
    }

    public void AirTime()
    {

        if (player.linearVelocity.y < 0)
        {
            player.linearVelocity += Vector2.down * (Mathf.Abs(Physics2D.gravity.y) * (fallMultiplier) * player.gravityScale * Time.deltaTime);
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

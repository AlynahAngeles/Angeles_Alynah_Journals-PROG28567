using UnityEngine;
//using static PlayerController;
using System.Collections; //added so that I can add IEnumerator for the Coroutine

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D player;
    public float playerSpeed = 3.5f; //speed of player when walking
    public float maxSpeed = 6f; //max speed of player

    public FacingDirection facingDirection;

    public float groundCheckDistance = 0.67f; //this is the value that helps with detecting that the player is on the floor, it is a red line that touches the ground and detects if the player is grounded.
    public LayerMask groundLayer; //Layer mask for the ground layer for collision detection

    public bool jump = false; //boolean for jump so that the jumps can max out at 2 (double jump)
    public float terminalSpeed; //fastest speed of player when falling

    Vector2 playerInput = new Vector2(); //to detect the player's inputs and appropriately update the shovel knight's movement

    public float jumpHeight = 20f; //apex of jump height
    public float jumpTime = 1.5f; //apex of jump time
    public float jumpForce = 20; //The perfect jump force for the right amount of push upward
    public float gravity; //variable for gravity

    public float fallMultiplier = 2.5f; //the acceleration of the player when descending

    public float coyoteTime = 0.5f; //the amount of time in which the player can jump after leaving the ground
    public float coyoteCounter; //counting the time before just sending the player down (can no longer accept the jump)

    public bool isGrounded = true;
    private bool isDash = false; //boolean that changes based on whether the player is dashing or not to avoid spamming dash and properly start coroutine
    public float dashCoolDown = 1.5f; //cooldown time for dashing so player can't spam
    public float dashDuration = 0.5f; //duration of the dash to give it a snappy feel
    public float dashForce = 30f; //the force of the dash to add the appropriate amount of push when dashing
    public bool canDash = true; //a boolean that detected when a player can dash or not
    public float dashDirection; //a variable so that the dash goes in the direction that the player is facing

    public float damage; //variable for the amount of damage the player deals to the enemy
    public float lunge; //the variable for the lunge speed
    public float lungeForce = 60f; //variable for the amount of power behind the lunge
    public bool canAttack = true; //a boolean to detect when a player can attack or not, prevents input overload
    public float resetTime = 0.2f; //time for resetting the cooldown of the lunge
    public bool isLunging = false; //a boolean to detect when the player is lunging
    public float lungeDuration = 0.5f; //the duration of the lunge animation 
    public PlatformerEnemy enemy; //a call out so that the code can find the enemy sprite

    public enum FacingDirection
    {
        left, right, idle
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>(); //equating player to the rigidbody (attaching it to the shovel knight)
        enemy = FindObjectOfType<PlatformerEnemy>(); //finding the platformer enemy type within the scene

        gravity = (-2 * jumpHeight) / (jumpTime * jumpTime); //formula for gravity, sets the speed of how fast the player falls by multiplying the jump height to -2, hence the higher the jump = the faster the fall. 
        jumpForce = (2 * jumpHeight) / jumpTime; //initial speed going up, adjusts the power of the jump based on the wanted jump height and creates more power depending on how fast the jump is

        player.gravityScale = gravity / Physics2D.gravity.y; //adjusts the game's gravity from the default Unity settings to the values in the script

    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded(); //at the start, the player will always be grounded

        if (isGrounded) //if the player is on the ground, then...
        {
            coyoteCounter = coyoteTime; //coyote time is not starting and remains default
        }
        else
        {
            coyoteCounter -= Time.deltaTime; //or else, the coyote counter goes down based on Time.deltaTime (game's run-time/real time)
        }

        playerInput.x = 0; //reseting the x inputs to 0
        playerInput.y = 0; //reseting the y inputs to 0

        if (!isDash && !isLunging) //if the player is NOT dashing NOR lunging, then...
        {

            if (Input.GetKey(KeyCode.A)) //if the input is A
            {
                playerInput.x = -3f; //then the player should move down the x-axis by -3 (move to the left)
            }

            if (Input.GetKey(KeyCode.D)) //if the input is D
            {
                playerInput.x = 3f; //then the player should move down the x-axis by 3 (move to the right)
            }

            if (Input.GetKeyDown(KeyCode.W) && coyoteCounter > 0f) //if the input is W AND the coyote counter has not reached 0
            {
                playerInput.y = 1; //then let the player's y increase by 1 (let the player jump)
            }

            MovementUpdate(playerInput); //connects the if statements to the method MovementUpdate 
        }

        if (jump == true && player.linearVelocity.y < 0) //if jump is true AND the player is still falling (y velocity =/= 0)
        {
            AirTime(); //trigger the method AirTime()
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) //if statement for when the player presses left shift and CAN dash, then start the coroutine to dash again
        {
            StartCoroutine(Dash()); //call the coroutine to start
        }

        if (Input.GetMouseButtonDown(0) && canAttack) //if the player clicks the left mouse button AND they are enabled to attack...
        {
            if (isGrounded) //IF the player is grounded (this is to limit the movement in the air because it was messing with the player's Y velocity previously)
            {
                StartCoroutine(Attack()); //start the Attack() coroutine
            }
        }
    }

    IEnumerator Dash() //coroutine for dash movement
    {
        canDash = false; //once this coroutine is triggered, then the player CANNOT dash again
        isDash = true; //once the coroutine is triggered, then the player is dashing

        if (facingDirection == FacingDirection.left && isDash == true) //if the player is facing left and isDash is true (boolean is to prevent this increased velocity to maintain after the dash)
        {
            dashDirection = -3f; //then the player shall dash to the left by 3units per frame (in the direction of the negative x-axis)
        }

        else if (facingDirection == FacingDirection.right && isDash == true) //if the player is facing right and isDash is true (boolean is to prevent this increased velocity to maintain after the dash)
        {
            dashDirection = 3f; //then the player shall dash to the right by 3units per frame (in the direction of the positive x-axis)
        }

        player.AddForce(new Vector2(dashDirection * dashForce, 0f), ForceMode2D.Impulse); //the actual logic behind the physics of the dash
        //Used add force for a dynamic push when dashing. The player's position is then updated using Vector2, the x position is translated according to the dash direction (direction player facing) and the force (power of dash). 
        //I used ForceMode2D.Impulse so that the speed is one burst of energy rather than a gradual increase in speed--makes the movement snappy

        yield return new WaitForSeconds(dashDuration); //wait until the dash is done

        isDash = false; //to detect that the player is no lpnger dashing

        yield return new WaitForSeconds(dashCoolDown); //wait a couple of seconds to avoid input overload

        canDash = true; //which then lets the player dash once again

    }

    private void MovementUpdate(Vector2 playerInput) //method to update the player's movements
    {

        player.AddForce(new Vector2(playerInput.x * playerSpeed, 0f), ForceMode2D.Force); //moves the player according to the input (either go left or right) and go by the wanted speed. Used AddForce so that the movement stops gradually and looks smoother.

        if (Mathf.Abs(player.linearVelocity.x) > maxSpeed) //if the player's speed is going over the max speed, then...
        {
            player.linearVelocity = new Vector2(Mathf.Clamp(player.linearVelocity.x, -maxSpeed, maxSpeed), player.linearVelocity.y); //cap the speed of the player according to the max speed, this stops the player character from endlessly accelerating in speed when walking.
            player.linearDamping = 5f; //add damping to gradually stop the player from moving
        }

        if (playerInput.y > 0f && coyoteCounter > 0f) //if the y input is greater than 0 (if the player is jumping) and the coyote counter is still going, then...
        {
            Debug.Log("Player jumped!"); //debug line
            player.linearVelocity = new Vector2(player.linearVelocity.x, jumpForce); //lets the player jump! this translates the player's y position by the jump force
            coyoteCounter = 0f; //reset the coyote counter so that it stops the player from jumping forever

        }
    }

    IEnumerator Attack()
    {
        int damage = Random.Range(4, 8); //random range that the player's attack can be
        Debug.Log("Player hit for " + damage + " damage!"); //debug log to check if the randomized range actually works
        canAttack = false; //when the player attacks, they cannot attack again to prevent input overload
        isLunging = true; //the player is lunging which stops the player from being able to lung again

        if (enemy != null && enemy.HPVisible()) //if the enemy's HP bar is visible and the enemy is not dead..
        {
            enemy.Damaged(damage); //input the player's hit and subtract it from the enemy's HP in the Platformer Enemy script
        }

        if (facingDirection == FacingDirection.left) //if the player is facing left, then...
        {
            lunge = -5f; //the player lunges to the left by 5units per frame (fast and powerful lunge)
        }

        else if (facingDirection == FacingDirection.right) //if the player is facing right, then...
        {
            lunge = 5f; //the player lunges to the right by 5units per frame (fast and powerful lunge)
        }

        player.AddForce(new Vector2(lunge * lungeForce, 0f), ForceMode2D.Impulse); //the formula for the actual lunge
        //I was stuck on this for a long while and just decided to mirror the dash since it worked so well.
        //this line adds a burst of acceleration in the direction of the lunge to emulate the player lunging to lance the enemy

        yield return new WaitForSeconds(lungeDuration); //wait the duration of the lunge...

        isLunging = false; //to detect that the player is no longer lunging

        yield return new WaitForSeconds(resetTime); //wait for the reset time (cooldown)

        canAttack = true; //for the player to be able to attack again
    }

    public void AirTime() //method for the player in the air
    {

        if (player.linearVelocity.y < 0) //when the player's y velocity is less than 0 (or the player is falling/in the air)
        {
            player.linearVelocity += Vector2.down * (Mathf.Abs(Physics2D.gravity.y) * (fallMultiplier) * player.gravityScale * Time.deltaTime);
            //this is the physics of the player falling
            //this puts the player's y velocity to go down and multiplies the gravity by the fall multiplier, which speeds up the player's gravity going down and accelerates the player's down direction to fall faster than Unity's default.
        }

        if (player.linearVelocity.y < terminalSpeed) //if the player is falling fasted than the top speed desired
            player.linearVelocity = new Vector2(player.linearVelocity.x, terminalSpeed); //then the player's speed is capped so that the acceleration does not go too fast

    }
    
    public bool IsWalking() //method for if the player is walking or not
    {

        bool walking = Mathf.Abs(player.linearVelocity.x) > 1f; //boolean for if the player is walking. 

        if (walking) //if the player's velocity is greater than one (no matter the direction0
        {
            GetFacingDirection(); //get the facing direction and turn the player's direction/sprite according to this return value
            return true; //the player is walking = true. Turns the state to the walking sprite animation
        }

        else //if not walking
        {
            walking = false; //then the pkayer is not walking
            return false; //the player is NOT walking = false. Turns the player's state back to idle
        }
    }

    public bool IsGrounded() //boolean for if the player is grounded
    {

        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.down, groundCheckDistance, groundLayer); //this is the raycast that checks the distance from the middle of the player sprite until it touches the floor--detecting the ground layer
        Debug.DrawRay(player.position, Vector2.down * groundCheckDistance, Color.red); //this physically draws the red line down to see if the edge of the red line appropriately touches the ground layer, so that the sprite perfectly aligns with the ground (not floating or going through)

        if (hit.collider != null) //if the collider is NOT null or exists, then...
        {
            isGrounded = true; //the player is grounded!
            jump = false; //the player is NOT jumping
            return true; //return isGrounded as true (the player is grounded
        }
        else //if not grounded
        {
            jump = true; //then the player is jumping
            isGrounded = false; //the player is NOT grounded
            return false; //return isGrounded as true (the player is NOT grounded
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

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Our players rigidbody
    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRigidBody;
    // Our animator to control running/jumping etc.
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private SpriteRenderer spriteRenderer;
    // Where to go on death
    [SerializeField] Transform spawnPoint;
    [Header("Movement")]
    // The speed of movement
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpPower = 12f;
    [SerializeField] private int maxJumps = 2;
    private int currentJumps = 0;
    [SerializeField] private float wallSlideSpeed = 0.5f;
    // For holding our horizontal movement speed
    private float horizontalMovement;
    // For stopping movement when wall jumping
    private int wallJumps;
    // Time taken for a wall jump
    [SerializeField] private float wallJumpTime = 1f;
    [SerializeField] private float wallJumpPower = 18f;
    [SerializeField] private ParticleSystem slideParticles;
    [Header("Checks")]
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 groundCheckArea = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask groundCheckLayer;
    [SerializeField] private Transform wallCheckPosition;
    [SerializeField] private Vector2 wallCheckArea = new Vector2(0.05f, 1.0f);
    [SerializeField] private LayerMask wallCheckLayer;
    [Header("Fades")]
    [SerializeField] private GameObject fadeIn;
    [SerializeField] private GameObject fadeOut;
    private bool isDead = false;

    // Subscribe to see whether we should respawn the player
    private void Awake()
    {
        EventManager.OnRespawnTriggered += OnTriggerRespawn;
    }
    private void OnDestroy()
    {
        EventManager.OnRespawnTriggered -= OnTriggerRespawn;
    }
    public void Update()
    {
        // Don't do anything if we are dead
        if(isDead) return;
        // Detect if we have fallen off the earth
        if (transform.position.y < -10)
        {
            Kill();
        }
        // If not wall jumping
        if (wallJumps == 0)
        {
            // Move in the x for any movement and wall jumping
            playerRigidBody.velocity = new Vector2(horizontalMovement * moveSpeed, playerRigidBody.velocity.y);
            // Adjust for wallsliding
            if (isOnWall() && !isOnFloor() && horizontalMovement != 0)
            {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, playerRigidBody.velocity.y * wallSlideSpeed);
                if (!slideParticles.isPlaying)
                {
                    slideParticles.Play();
                }
            }
            else
            {
                slideParticles.Stop();
            }
        }
        // Determine whether we need to flip our sprite
        flip();
        // Control our animations
        animate();
    }

    private void flip()
    {
        // Check which way we are moving and flip our transforms x accordingly, leaving y and z alone
        if (playerRigidBody.velocity.x < -0.5)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if (playerRigidBody.velocity.x > 0.5)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    private void animate()
    {
        // Defaults
        animator.SetBool("isRunning", false);
        animator.SetBool("isJumping", false);
        // If we are not on the floor then we are jumping
        if (!isOnFloor())
        {
            animator.SetBool("isJumping", true);
        }
        // If we are moving then set our animation to running
        else if (horizontalMovement != 0)
        {
            animator.SetBool("isRunning", true);
        }
    }

    // This is called by the move event from our input system
    public void Move(InputAction.CallbackContext context)
    {
        // Get the value of the x movement from our context
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    // This is called from the jump event from our input system
    public void Jump(InputAction.CallbackContext context)
    {
        // Reset our jumps if we are on the floor
        if (isOnFloor())
        {
            currentJumps = 0;
        }
        // Deal with wall jumping
        if (context.performed && isOnWall())
        {
            // Get the direction of x from the localScale and fire in that direction - y as per normal jump
            playerRigidBody.velocity = new Vector2(wallJumpPower * -transform.localScale.x, wallJumpPower * 2);
            wallJumps++;
            // Play the jump sound
            SoundManager.Play("Jump");
            // Start a coroutine to reset the effect of a walljump after 0.5 seconds
            StartCoroutine(endWallJump(wallJumpTime));
        }
        // Now only proceed if we are less than our max jumps
        else if (context.performed && currentJumps < maxJumps)
        {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpPower);
            currentJumps++;
            // Play the jump sound
            SoundManager.Play("Jump");
        }
    }

    private IEnumerator endWallJump(float time)
    {
        // Wait for the specified seconds
        yield return new WaitForSeconds(time);
        wallJumps--;
    }

    // Determine whether the player is on the floor
    private bool isOnFloor()
    {
        return Physics2D.OverlapBox(groundCheckPosition.position, groundCheckArea, 0, groundCheckLayer);
    }
    // Determine whether the player is touching a wall
    private bool isOnWall()
    {
        return Physics2D.OverlapBox(wallCheckPosition.position, wallCheckArea, 0, wallCheckLayer);
    }

    // Draw our gizmos for checking ground/walls etc.
    private void OnDrawGizmos()
    {
        // Ground check in blue
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckArea);
        // Wall check in green
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(wallCheckPosition.position, wallCheckArea);
    }

    public void Kill()
    {
        if (!isDead)
        {
            StartCoroutine(DeathAnimation());
        }
    }

    private IEnumerator DeathAnimation()
    {
        // Play the hurt sound 
        SoundManager.Play("Hurt");
        // Hide our sprite
        spriteRenderer.enabled = false;
        // Play our death effect
        deathEffect.Play();
        // Set our state
        isDead = true;
        // Stop any movement
        playerRigidBody.simulated = false;
        yield return new WaitForSeconds(3f);
        // and trigger an event
        EventManager.OnPlayerDeath?.Invoke();
    }

    private void OnTriggerRespawn()
    {
        StartCoroutine(ReSpawn());
    }

    private IEnumerator ReSpawn()
    {
        // Start the fade out animation and wait one second
        GameObject fadeOutInstance = Instantiate(fadeOut);
        yield return new WaitForSeconds(1f);
        // Reset the player position
        transform.position = spawnPoint.position;
        playerRigidBody.velocity = Vector2.zero;
        playerRigidBody.simulated = true;
        // Set state back to alive
        isDead = false;
        // Show our sprite again
        spriteRenderer.enabled = true;
        yield return new WaitForSeconds(0.75f);
        // After the scene is loaded remove the fade out and start the fade in
        Destroy(fadeOutInstance);
        GameObject fadeInInstance = Instantiate(fadeIn);
        yield return new WaitForSeconds(1f);
        // Clean up
        Destroy(fadeInInstance);
    }
}

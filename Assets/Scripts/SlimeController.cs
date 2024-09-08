using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D slimeRigidBody;
    [SerializeField] private float slimeSpeed = 15f;
    [Header("Checks")]
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 groundCheckArea = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask groundCheckLayer;
    [SerializeField] private Transform wallCheckPosition;
    [SerializeField] private Vector2 wallCheckArea = new Vector2(0.05f, 1.0f);
    [SerializeField] private LayerMask wallCheckLayer;
    private int direction = 1;
    // Start is called before the first frame update
    void FixedUpdate()
    {
        // See if we are facing the right way
        CheckDirection();
        // Move the slime in the direction and speed specified
        slimeRigidBody.velocity = new Vector2(direction * slimeSpeed, slimeRigidBody.velocity.y);
        // Flip the slime to face the right way
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
    }

    private void CheckDirection()
    {
        // If the slime has hit a wall or is about to fall off a cliff then change direction
        bool onFloor = Physics2D.OverlapBox(groundCheckPosition.position, groundCheckArea, 0, groundCheckLayer);
        bool atWall = Physics2D.OverlapBox(wallCheckPosition.position, wallCheckArea, 0, wallCheckLayer);
        // If we are at a wall, or about to fall, then change direction
        if(atWall || !onFloor)
        {
            direction *= -1;
        }
    }

    private void OnDrawGizmos()
    {
        // Ground check in blue
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckArea);
        // Wall check in green
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(wallCheckPosition.position, wallCheckArea);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.Kill();
        }
    }
}

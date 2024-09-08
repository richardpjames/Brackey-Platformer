using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the collision was with a player 
        PlayerMovement player = collider.gameObject.GetComponent<PlayerMovement>();
        if (player != null)
        {
            // Play a sound effect
            SoundManager.Play("Coin");
            // Trigger event and destroy the coin
            EventManager.OnCoinCollected?.Invoke();
            Destroy(gameObject);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void StartGame()
    {
        SoundManager.Play("Coin");
        GameManager.ResetScore();
        GameManager.ResetLives();
        GameManager.LoadLevel("Level_1");
    }
}

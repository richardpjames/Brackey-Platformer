using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameComplete : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "You beat the game with " + GameManager.GetScore() + " points!";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "Score: " + GameManager.GetScore();
        EventManager.OnScoreUpdated += UpdateScore;
    }

    private void UpdateScore(int newScore)
    {
        text.text = "Score: " + newScore;
    }

    // Unsubscribe to events when destroyed
    private void OnDestroy()
    {
        EventManager.OnScoreUpdated -= UpdateScore;
    }
}

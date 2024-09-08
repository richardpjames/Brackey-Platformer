using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesCounter : MonoBehaviour
{
    [SerializeField] private Text text;
    // Start is called before the first frame update
    void Start()
    {
        UpdateLives();
        EventManager.OnRespawnTriggered += UpdateLives;
    }

    private void OnDestroy()
    {
        EventManager.OnRespawnTriggered -= UpdateLives;
    }

    private void UpdateLives()
    {
        if (GameManager.GetLives() > 1)
        {
            text.text = GameManager.GetLives() + " Lives Left";
        }
        else
        {
            text.text = "1 Life Left";
        }
    }
}

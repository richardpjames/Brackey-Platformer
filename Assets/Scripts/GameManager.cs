using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [SerializeField] private GameObject fadeInPrefab;
    [SerializeField] private GameObject fadeOutPrefab;
    private static GameObject fadeInStatic;
    private static GameObject fadeOutStatic;
    // Store the score for the game
    private static int Score = 0;
    private static int Lives = 3;

    private void Awake()
    {
        // Subscribe to events
        EventManager.OnCoinCollected += AddScore;
        EventManager.OnPlayerDeath += PlayerDeath;
        // Set up our objects
        fadeInStatic = fadeInPrefab;
        fadeOutStatic = fadeOutPrefab;
        // Ensure that this is the only instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Two methods for adding to the score and for retreiving it
    public static void AddScore()
    {
        Score++;
        EventManager.OnScoreUpdated?.Invoke(Score);
    }
    public static int GetScore()
    {
        Debug.Log(Score);
        return Score;

    }

    public static void ResetScore()
    {
        Score = 0;
    }

    public static int GetLives()
    {
        return Lives;
    }

    public static void ResetLives()
    {
        Lives = 3;
    }

    public static void LoadLevel(string sceneName)
    {
        instance.StartCoroutine(AnimateTransition(sceneName));
    }

    private static IEnumerator AnimateTransition(string sceneName)
    {
        // Start the fade out animation and wait one second
        GameObject fadeOut = Instantiate(fadeOutStatic);
        yield return new WaitForSeconds(1f);
        // Start scene loading
        SceneManager.LoadScene(sceneName);
        // Wait for the scene manager to load the scene
        while (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return null;
        }
        // After the scene is loaded remove the fade out and start the fade in
        Destroy(fadeOut);
        GameObject fadeIn = Instantiate(fadeInStatic);
        yield return new WaitForSeconds(1f);
        // Clean up
        Destroy(fadeIn);
    }

    // Unsubscribe to any events on destroy
    private void OnDestroy()
    {
        EventManager.OnCoinCollected -= AddScore;
        EventManager.OnPlayerDeath -= PlayerDeath;
    }

    private void PlayerDeath()
    {
        // If we have any lives left then just decrease and ask the player to respawn
        if(Lives > 1)
        {
            Lives--;
            EventManager.OnRespawnTriggered.Invoke();
        } 
        // Otherwise go to the game over screen and reset the number of lives
        else
        {
            LoadLevel("GameOver");
            Lives = 3;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Configurable lives count
    [SerializeField] int lives = 5;

    // Awake is called when the script instance is being loaded.
    // Singleton pattern to support life system over multiple levels.
    private void Awake() {
        if(FindObjectsOfType<GameManager>().Length > 1) {
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    // Called when the player loses a life. Controls the "game over" aspect of the game
    public void subtractLives() {
        lives -= 1;
        if (lives < 1) {
            SceneManager.LoadScene(0);
            Destroy(gameObject);
        } else {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Return int lives
    public float getLives() {
        return lives;
    }
}

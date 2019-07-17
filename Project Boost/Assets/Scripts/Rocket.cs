using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Rocket : MonoBehaviour {

    // Configuration Parameters
    [Header("Shuttle Parameters")]
    [SerializeField] float thrustFactor = 100f;
    [SerializeField] float rotationFactor = 100f;
    [SerializeField] float loadLevelDelay = 2f;
    [Header("Audio Settings")]
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;
    [Header("Particle Systems")]
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    // Cached references
    Rigidbody rigidBody;
    AudioSource audioSource;
    GameManager gameManager;
    TextMeshProUGUI livesText;

    // State variables
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if(GameObject.Find("Game Manager") != null) {
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }
        livesText = GameObject.FindObjectOfType<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        if(state == State.Alive) {
            RespondToThrustInput();
            Rotate();
        }
        if (Debug.isDebugBuild) {
            RespondToDebugKeys();
        }

        if(SceneManager.GetActiveScene().buildIndex != 0) {
            livesText.text = gameManager.getLives().ToString();
        }

        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    // Debug Keys
    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextLevel();
        }else if (Input.GetKeyDown(KeyCode.C)) {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    // The collicion handler (When the rocket hits an object)
    private void OnCollisionEnter(Collision collision) {

        if(state != State.Alive || collisionsDisabled) { return; }

        switch (collision.gameObject.tag) {
            case "Friendly": {
                    // do nothing
                    break;
                }
            case "Finish": {
                    StartSuccessSequence();
                    break;
                }
            default:
                StartDeathSequence();
                break;
        }
    }

    // The shuttle death process
    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        mainEngineParticles.Stop();
        deathParticles.Play();
        gameManager.Invoke("subtractLives", loadLevelDelay);
        //gameManager.Invoke("LoadFirstLevel", loadLevelDelay);
    }

    // Finished the level! Load the next level
    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", loadLevelDelay);
    }

    // Reset the game
    private void LoadFirstLevel() {
        SceneManager.LoadScene(0);
    }

    // Load the next level 
    private void LoadNextLevel() {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }

    // Function to handle the upwards momentum of the rocket
    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        } else {
            StopApplyingThrust();
        }
    }

    // Stop the thrust effects 
    private void StopApplyingThrust() {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    // Application of the shuttle thrust in the upwards direction (positive z)
    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * thrustFactor * Time.deltaTime);
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    // Function to handle the rotation of the rocket
    private void Rotate() {
        // This is to take manual control of the rotation
        if (Input.GetKey(KeyCode.LeftArrow)) {
            RotateManually(rotationFactor);
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            RotateManually(-rotationFactor);
        }
        // Resume environmental control of the rotation
        rigidBody.freezeRotation = false;
    }

    // The function of rotate() extracted to reduce duplicate code
    private void RotateManually(float rotationFactor) {
        rigidBody.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationFactor * Time.deltaTime);
    }
}

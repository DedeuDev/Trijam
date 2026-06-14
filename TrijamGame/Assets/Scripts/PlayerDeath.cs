using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDeath : MonoBehaviour
{
    public static event Action OnPlayerDied;

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Fall Death")]
    [SerializeField] private float bottomLimit = -0.15f;

    [Header("Death Settings")]
    [SerializeField] private bool restartSceneOnDeath = true;
    [SerializeField] private float restartDelay = 0.6f;

    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private PlayerBounds playerBounds;
    private PlayerInput playerInput;

    public AudioSource audioSource;

    public GameObject BackButton;
    public GameObject PauseButton;

    private bool isDead;

    public bool IsDead => isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        playerBounds = GetComponent<PlayerBounds>();
        playerInput = GetComponent<PlayerInput>();

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        CheckFallDeath();
    }

    private void CheckFallDeath()
    {
        if (isDead) return;
        if (targetCamera == null) return;

        Vector3 viewportPosition = targetCamera.WorldToViewportPoint(transform.position);

        if (viewportPosition.y < bottomLimit)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        audioSource.Play();

        Cursor.visible = true;

        Destroy(PauseButton);
        Destroy(BackButton);

        OnPlayerDied?.Invoke();

        DisablePlayerControl();
        StopPlayerMovement();

        if (restartSceneOnDeath)
        {
            Invoke(nameof(RestartScene), restartDelay);
        }
    }

    private void DisablePlayerControl()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerBounds != null)
        {
            playerBounds.enabled = false;
        }

        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
    }

    private void StopPlayerMovement()
    {
        Vector3 velocity = GetVelocity();

        velocity.x = 0f;
        velocity.z = 0f;

        SetVelocity(velocity);
    }

    private void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private Vector3 GetVelocity()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    private void SetVelocity(Vector3 velocity)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = velocity;
#else
        rb.velocity = velocity;
#endif
    }
}
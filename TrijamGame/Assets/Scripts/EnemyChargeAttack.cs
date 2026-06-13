using System.Collections;
using UnityEngine;

public class EnemyChargeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemySphereCore sphereCore;

    [Header("Charge Settings")]
    [SerializeField] private float warningDuration = 0.5f;
    [SerializeField] private float chargeSpeed = 18f;
    [SerializeField] private float stopDistance = 0.1f;
    [SerializeField] private float timeAfterAttack = 0.4f;

    [Header("Spin Visual")]
    [SerializeField] private Transform spinTarget;
    [SerializeField] private float warningSpinSpeed = 360f;
    [SerializeField] private float chargeSpinSpeed = 720f;
    [SerializeField] private Vector3 spinAxis = Vector3.forward;
    [SerializeField] private bool resetRotationAfterAttack = false;

    [Header("Audio Preparation")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip warningSound;
    [SerializeField] private AudioClip chargeSound;

    [Header("Debug")]
    [SerializeField] private bool allowKeyboardTest = true;

    private Quaternion originalLocalRotation;

    private bool isWarning;
    private bool isCharging;
    private bool isExecutingAttack;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (sphereCore == null)
        {
            sphereCore = GetComponent<EnemySphereCore>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (spinTarget == null)
        {
            spinTarget = transform;
        }

        originalLocalRotation = spinTarget.localRotation;
    }

    private void Update()
    {
        HandleSpinVisual();

        if (allowKeyboardTest && Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(ExecuteChargeAttack());
        }
    }

    public IEnumerator ExecuteChargeAttack()
    {
        if (isExecutingAttack) yield break;
        if (player == null) yield break;

        isExecutingAttack = true;

        if (sphereCore != null)
        {
            sphereCore.Close();
        }

        Vector3 targetPosition = player.position;
        targetPosition.z = transform.position.z;

        yield return StartCoroutine(SpinWarning());

        PlaySound(chargeSound);

        isCharging = true;

        while (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                chargeSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;

        isCharging = false;

        if (resetRotationAfterAttack)
        {
            spinTarget.localRotation = originalLocalRotation;
        }

        yield return new WaitForSeconds(timeAfterAttack);

        isExecutingAttack = false;
    }

    private IEnumerator SpinWarning()
    {
        isWarning = true;

        PlaySound(warningSound);

        yield return new WaitForSeconds(warningDuration);

        isWarning = false;
    }

    private void HandleSpinVisual()
    {
        if (spinTarget == null) return;

        if (isWarning)
        {
            RotateSpinTarget(warningSpinSpeed);
        }
        else if (isCharging)
        {
            RotateSpinTarget(chargeSpinSpeed);
        }
    }

    private void RotateSpinTarget(float spinSpeed)
    {
        spinTarget.Rotate(
            spinAxis.normalized,
            spinSpeed * Time.deltaTime,
            Space.Self
        );
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null) return;
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }
}
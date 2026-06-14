using System.Collections;
using UnityEngine;

public class EnemySphereCore : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Transform topHalf;
    [SerializeField] private Transform bottomHalf;
    [SerializeField] private Transform core;

    [Header("Open Settings")]
    [SerializeField] private float openDistance = 4f;
    [SerializeField] private float openSpeed = 10f;
    [SerializeField] private float closeSpeed = 14f;

    [Header("Attack Timing")]
    [SerializeField] private float warningDuration = 0.7f;
    [SerializeField] private float timeOpen = 1.2f;
    [SerializeField] private float timeAfterAttack = 0.4f;

    [Header("Warning Visual")]
    [SerializeField] private Transform warningShakeTarget;
    [SerializeField] private float shakeIntensity = 0.08f;
    [SerializeField] private float shakeSpeed = 45f;

    [Header("Audio Preparation")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip warningSound;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    [Header("Core Settings")]
    [SerializeField] private bool hideCoreWhenClosed = true;

    [Header("Debug")]
    [SerializeField] private bool allowKeyboardTest = true;

    private Vector3 topClosedPosition;
    private Vector3 bottomClosedPosition;

    private Vector3 topOpenPosition;
    private Vector3 bottomOpenPosition;

    private Vector3 shakeOriginalLocalPosition;

    private bool isOpen;
    private bool isWarning;
    private bool isExecutingAttack;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        topClosedPosition = topHalf.localPosition;
        bottomClosedPosition = bottomHalf.localPosition;

        if (warningShakeTarget != null)
        {
            shakeOriginalLocalPosition = warningShakeTarget.localPosition;
        }

        CalculateOpenPositions();
        CloseImmediately();
    }

    private void Update()
    {
        MoveParts();
        HandleWarningVisual();

        if (allowKeyboardTest)
        {
            HandleKeyboardTest();
        }
    }

    public IEnumerator ExecuteSplitAttack()
    {
        if (isExecutingAttack) yield break;

        isExecutingAttack = true;

        Close();

        yield return StartCoroutine(Warning());

        Open();

        yield return new WaitForSeconds(timeOpen);

        Close();

        yield return new WaitForSeconds(timeAfterAttack);

        isExecutingAttack = false;
    }

    private IEnumerator Warning()
    {
        isWarning = true;

        if (warningShakeTarget != null)
        {
            shakeOriginalLocalPosition = warningShakeTarget.localPosition;
        }

        PlaySound(warningSound);

        yield return new WaitForSeconds(warningDuration);

        isWarning = false;
        ResetWarningVisual();
    }

    private void HandleWarningVisual()
    {
        if (!isWarning) return;
        if (warningShakeTarget == null) return;

        float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
        float shakeY = Mathf.Cos(Time.time * shakeSpeed * 1.3f) * shakeIntensity;

        warningShakeTarget.localPosition = shakeOriginalLocalPosition + new Vector3(shakeX, shakeY, 0f);
    }

    private void ResetWarningVisual()
    {
        if (warningShakeTarget == null) return;

        warningShakeTarget.localPosition = shakeOriginalLocalPosition;
    }

    private void HandleKeyboardTest()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(ExecuteSplitAttack());
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Open();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Close();
        }
    }

    private void CalculateOpenPositions()
    {
        topOpenPosition = topClosedPosition + Vector3.up * openDistance;
        bottomOpenPosition = bottomClosedPosition + Vector3.down * openDistance;
    }

    private void MoveParts()
    {
        Vector3 targetTopPosition = isOpen ? topOpenPosition : topClosedPosition;
        Vector3 targetBottomPosition = isOpen ? bottomOpenPosition : bottomClosedPosition;

        float baseSpeed = isOpen ? openSpeed : closeSpeed;
        float currentSpeed = DifficultyManager.ApplySpeed(baseSpeed);

        topHalf.localPosition = Vector3.MoveTowards(
            topHalf.localPosition,
            targetTopPosition,
            currentSpeed * Time.deltaTime
        );

        bottomHalf.localPosition = Vector3.MoveTowards(
            bottomHalf.localPosition,
            targetBottomPosition,
            currentSpeed * Time.deltaTime
        );
    }

    public void Open()
    {
        isOpen = true;
        isWarning = false;
        ResetWarningVisual();
        UpdateCoreVisibility();
        PlaySound(openSound);
    }

    public void Close()
    {
        isOpen = false;
        isWarning = false;
        ResetWarningVisual();
        UpdateCoreVisibility();
        PlaySound(closeSound);
    }

    private void CloseImmediately()
    {
        isOpen = false;
        isWarning = false;

        topHalf.localPosition = topClosedPosition;
        bottomHalf.localPosition = bottomClosedPosition;

        ResetWarningVisual();
        UpdateCoreVisibility();
    }

   private void UpdateCoreVisibility()
    {
        if (core == null) return;

        if (hideCoreWhenClosed)
        {
            core.gameObject.SetActive(isOpen);
        }
        else
        {
            core.gameObject.SetActive(true);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null) return;
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }
}
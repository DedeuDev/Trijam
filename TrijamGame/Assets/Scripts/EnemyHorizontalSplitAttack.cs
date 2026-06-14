using System.Collections;
using UnityEngine;

public class EnemyHorizontalSplitAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySphereCore sphereCore;
    [SerializeField] private Transform topHalf;
    [SerializeField] private Transform bottomHalf;
    [SerializeField] private Transform core;
    [SerializeField] private Camera targetCamera;

    [Header("Attack Settings")]
    [SerializeField] private float warningDuration = 0.5f;
    [SerializeField] private float minVerticalOpenDistance = 0.8f;
    [SerializeField] private float maxVerticalOpenDistance = 2.4f;
    [SerializeField] private float openSpeed = 8f;
    [SerializeField] private float closeSpeed = 10f;
    [SerializeField] private float horizontalSpeed = 12f;
    [SerializeField] private float returnSpeed = 14f;
    [SerializeField] private float edgePadding = 0.2f;
    [SerializeField] private float verticalEdgePadding = 0.2f;
    [SerializeField] private float timeAfterAttack = 0.4f;

    [Header("Core Visual During Attack")]
    [SerializeField] private bool animateCoreDuringAttack = true;
    [SerializeField] private float coreSpinSpeed = 540f;
    [SerializeField] private Vector3 coreSpinAxis = Vector3.forward;
    [SerializeField] private float coreShakeIntensity = 0.06f;
    [SerializeField] private float coreShakeSpeed = 45f;

    [Header("Direction")]
    [SerializeField] private bool randomizeDirections = true;
    [SerializeField] private bool topGoesRight = true;

    [Header("Halves Spin Visual")]
    [SerializeField] private bool spinHalvesWhileMoving = true;
    [SerializeField] private float halvesSpinSpeed = 720f;
    [SerializeField] private Vector3 halvesSpinAxis = Vector3.forward;

    [Header("Rotation Reset")]
    [SerializeField] private bool resetHalvesRotationAfterAttack = false;
    [SerializeField] private bool resetCoreRotationAfterAttack = true;

    [Header("Audio Preparation")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip warningSound;
    [SerializeField] private AudioClip splitSound;
    [SerializeField] private AudioClip returnSound;
    [SerializeField] private AudioClip closeSound;

    [Header("Debug")]
    [SerializeField] private bool allowKeyboardTest = true;

    private Vector3 topClosedLocalPosition;
    private Vector3 bottomClosedLocalPosition;
    private Vector3 coreClosedLocalPosition;

    private Vector3 topOpenLocalPosition;
    private Vector3 bottomOpenLocalPosition;

    private Quaternion topOriginalLocalRotation;
    private Quaternion bottomOriginalLocalRotation;
    private Quaternion coreOriginalLocalRotation;

    private bool isExecutingAttack;
    private bool isMovingHorizontally;
    private bool isCoreAnimating;

    private void Awake()
    {
        if (sphereCore == null)
        {
            sphereCore = GetComponent<EnemySphereCore>();
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        topClosedLocalPosition = topHalf.localPosition;
        bottomClosedLocalPosition = bottomHalf.localPosition;
        coreClosedLocalPosition = core.localPosition;

        topOriginalLocalRotation = topHalf.localRotation;
        bottomOriginalLocalRotation = bottomHalf.localRotation;
        coreOriginalLocalRotation = core.localRotation;

        CalculateRandomOpenPositions();
    }

    private void Update()
    {
        if (isMovingHorizontally && spinHalvesWhileMoving)
        {
            SpinHalves();
        }

        if (isCoreAnimating && animateCoreDuringAttack)
        {
            AnimateCore();
        }

        if (allowKeyboardTest && Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(ExecuteHorizontalSplitAttack());
        }
    }

    public IEnumerator ExecuteHorizontalSplitAttack()
    {
        if (isExecutingAttack) yield break;

        isExecutingAttack = true;

        if (sphereCore != null)
        {
            sphereCore.Close();
            sphereCore.enabled = false;
        }

        ResetPartsToClosedPosition();

        CalculateRandomOpenPositions();
        RandomizeAttackDirection();

        core.gameObject.SetActive(true);

        PlaySound(warningSound);

        yield return new WaitForSeconds(warningDuration);

        yield return StartCoroutine(OpenVertically());

        PlaySound(splitSound);

        isCoreAnimating = true;

        yield return StartCoroutine(MoveHalvesToEdges());

        PlaySound(returnSound);

        yield return StartCoroutine(ReturnHalvesToCenter());

        isCoreAnimating = false;

        ResetCoreVisual();

        PlaySound(closeSound);

        yield return StartCoroutine(CloseVertically());

        ResetPartsToClosedPosition();

        if (sphereCore != null)
        {
            sphereCore.enabled = true;
            sphereCore.Close();
        }
        else
        {
            core.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(timeAfterAttack);

        isExecutingAttack = false;
    }

    private void CalculateRandomOpenPositions()
    {
        float topScreenLimitY = GetCameraEdgeY(1f) - verticalEdgePadding;
        float bottomScreenLimitY = GetCameraEdgeY(0f) + verticalEdgePadding;

        float topHalfExtentY = GetObjectHalfHeight(topHalf);
        float bottomHalfExtentY = GetObjectHalfHeight(bottomHalf);

        float maxTopY = topScreenLimitY - topHalfExtentY;
        float minBottomY = bottomScreenLimitY + bottomHalfExtentY;

        float currentTopY = topHalf.position.y;
        float currentBottomY = bottomHalf.position.y;

        float maxTopOpenDistance = maxTopY - currentTopY;
        float maxBottomOpenDistance = currentBottomY - minBottomY;

        float safeMaxOpenDistance = Mathf.Min(
            maxVerticalOpenDistance,
            maxTopOpenDistance,
            maxBottomOpenDistance
        );

        safeMaxOpenDistance = Mathf.Max(safeMaxOpenDistance, 0f);

        float safeMinOpenDistance = Mathf.Min(minVerticalOpenDistance, safeMaxOpenDistance);

        float randomOpenDistance = Random.Range(
            safeMinOpenDistance,
            safeMaxOpenDistance
        );

        topOpenLocalPosition = topClosedLocalPosition + Vector3.up * randomOpenDistance;
        bottomOpenLocalPosition = bottomClosedLocalPosition + Vector3.down * randomOpenDistance;
    }

    private void RandomizeAttackDirection()
    {
        if (!randomizeDirections) return;

        topGoesRight = Random.value > 0.5f;
    }

    private IEnumerator OpenVertically()
    {
        while (
            Vector3.Distance(topHalf.localPosition, topOpenLocalPosition) > 0.02f ||
            Vector3.Distance(bottomHalf.localPosition, bottomOpenLocalPosition) > 0.02f
        )
        {
            topHalf.localPosition = Vector3.MoveTowards(
                topHalf.localPosition,
                topOpenLocalPosition,
                openSpeed * Time.deltaTime
            );

            bottomHalf.localPosition = Vector3.MoveTowards(
                bottomHalf.localPosition,
                bottomOpenLocalPosition,
                openSpeed * Time.deltaTime
            );

            yield return null;
        }

        topHalf.localPosition = topOpenLocalPosition;
        bottomHalf.localPosition = bottomOpenLocalPosition;
    }

    private IEnumerator MoveHalvesToEdges()
    {
        isMovingHorizontally = true;

        float leftEdgeX = GetCameraEdgeX(0f);
        float rightEdgeX = GetCameraEdgeX(1f);

        float topHalfExtent = GetObjectHalfWidth(topHalf);
        float bottomHalfExtent = GetObjectHalfWidth(bottomHalf);

        float topLeftLimit = leftEdgeX + topHalfExtent + edgePadding;
        float topRightLimit = rightEdgeX - topHalfExtent - edgePadding;

        float bottomLeftLimit = leftEdgeX + bottomHalfExtent + edgePadding;
        float bottomRightLimit = rightEdgeX - bottomHalfExtent - edgePadding;

        float topTargetX = topGoesRight ? topRightLimit : topLeftLimit;
        float bottomTargetX = topGoesRight ? bottomLeftLimit : bottomRightLimit;

        Vector3 topTargetWorldPosition = topHalf.position;
        Vector3 bottomTargetWorldPosition = bottomHalf.position;

        topTargetWorldPosition.x = topTargetX;
        bottomTargetWorldPosition.x = bottomTargetX;

        while (
            Vector3.Distance(topHalf.position, topTargetWorldPosition) > 0.05f ||
            Vector3.Distance(bottomHalf.position, bottomTargetWorldPosition) > 0.05f
        )
        {
            topHalf.position = Vector3.MoveTowards(
                topHalf.position,
                topTargetWorldPosition,
                DifficultyManager.ApplySpeed(horizontalSpeed) * Time.deltaTime
            );

            bottomHalf.position = Vector3.MoveTowards(
                bottomHalf.position,
                bottomTargetWorldPosition,
                DifficultyManager.ApplySpeed(horizontalSpeed) * Time.deltaTime
            );

            yield return null;
        }

        topHalf.position = topTargetWorldPosition;
        bottomHalf.position = bottomTargetWorldPosition;

        isMovingHorizontally = false;
    }

    private IEnumerator ReturnHalvesToCenter()
    {
        isMovingHorizontally = true;

        while (
            Vector3.Distance(topHalf.localPosition, topOpenLocalPosition) > 0.05f ||
            Vector3.Distance(bottomHalf.localPosition, bottomOpenLocalPosition) > 0.05f
        )
        {
            topHalf.localPosition = Vector3.MoveTowards(
                topHalf.localPosition,
                topOpenLocalPosition,
                DifficultyManager.ApplySpeed(returnSpeed) * Time.deltaTime
            );

            bottomHalf.localPosition = Vector3.MoveTowards(
                bottomHalf.localPosition,
                bottomOpenLocalPosition,
                DifficultyManager.ApplySpeed(returnSpeed) * Time.deltaTime
            );

            yield return null;
        }

        topHalf.localPosition = topOpenLocalPosition;
        bottomHalf.localPosition = bottomOpenLocalPosition;

        isMovingHorizontally = false;
    }

    private IEnumerator CloseVertically()
    {
        while (
            Vector3.Distance(topHalf.localPosition, topClosedLocalPosition) > 0.02f ||
            Vector3.Distance(bottomHalf.localPosition, bottomClosedLocalPosition) > 0.02f
        )
        {
            topHalf.localPosition = Vector3.MoveTowards(
                topHalf.localPosition,
                topClosedLocalPosition,
                closeSpeed * Time.deltaTime
            );

            bottomHalf.localPosition = Vector3.MoveTowards(
                bottomHalf.localPosition,
                bottomClosedLocalPosition,
                closeSpeed * Time.deltaTime
            );

            yield return null;
        }

        topHalf.localPosition = topClosedLocalPosition;
        bottomHalf.localPosition = bottomClosedLocalPosition;
    }

    private void AnimateCore()
    {
        core.Rotate(
            coreSpinAxis.normalized,
            coreSpinSpeed * Time.deltaTime,
            Space.Self
        );

        float shakeX = Mathf.Sin(Time.time * coreShakeSpeed) * coreShakeIntensity;
        float shakeY = Mathf.Cos(Time.time * coreShakeSpeed * 1.2f) * coreShakeIntensity;

        core.localPosition = coreClosedLocalPosition + new Vector3(shakeX, shakeY, 0f);
    }

    private void ResetCoreVisual()
    {
        core.localPosition = coreClosedLocalPosition;

        if (resetCoreRotationAfterAttack)
        {
            core.localRotation = coreOriginalLocalRotation;
        }
    }

    private void SpinHalves()
    {
        topHalf.Rotate(
            halvesSpinAxis.normalized,
            halvesSpinSpeed * Time.deltaTime,
            Space.Self
        );

        bottomHalf.Rotate(
            halvesSpinAxis.normalized,
            -halvesSpinSpeed * Time.deltaTime,
            Space.Self
        );
    }

    private void ResetPartsToClosedPosition()
    {
        topHalf.localPosition = topClosedLocalPosition;
        bottomHalf.localPosition = bottomClosedLocalPosition;
        core.localPosition = coreClosedLocalPosition;

        if (resetHalvesRotationAfterAttack)
        {
            topHalf.localRotation = topOriginalLocalRotation;
            bottomHalf.localRotation = bottomOriginalLocalRotation;
        }

        if (resetCoreRotationAfterAttack)
        {
            core.localRotation = coreOriginalLocalRotation;
        }
    }

    private float GetCameraEdgeX(float viewportX)
    {
        if (targetCamera == null)
        {
            return transform.position.x;
        }

        Plane enemyPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, transform.position.z));
        Ray ray = targetCamera.ViewportPointToRay(new Vector3(viewportX, 0.5f, 0f));

        if (enemyPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            return worldPoint.x;
        }

        return transform.position.x;
    }

    private float GetCameraEdgeY(float viewportY)
    {
        if (targetCamera == null)
        {
            return transform.position.y;
        }

        Plane enemyPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, transform.position.z));
        Ray ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, viewportY, 0f));

        if (enemyPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            return worldPoint.y;
        }

        return transform.position.y;
    }

    private float GetObjectHalfWidth(Transform objectTransform)
    {
        Renderer objectRenderer = objectTransform.GetComponentInChildren<Renderer>();

        if (objectRenderer != null)
        {
            return objectRenderer.bounds.extents.x;
        }

        Collider objectCollider = objectTransform.GetComponentInChildren<Collider>();

        if (objectCollider != null)
        {
            return objectCollider.bounds.extents.x;
        }

        return 0.5f;
    }

    private float GetObjectHalfHeight(Transform objectTransform)
    {
        Renderer objectRenderer = objectTransform.GetComponentInChildren<Renderer>();

        if (objectRenderer != null)
        {
            return objectRenderer.bounds.extents.y;
        }

        Collider objectCollider = objectTransform.GetComponentInChildren<Collider>();

        if (objectCollider != null)
        {
            return objectCollider.bounds.extents.y;
        }

        return 0.5f;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null) return;
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }
}
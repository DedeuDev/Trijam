using System.Collections;
using UnityEngine;

public class EnemySphereMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stopDistance = 0.05f;

    [Header("Camera Area")]
    [SerializeField] private float minViewportX = 0.2f;
    [SerializeField] private float maxViewportX = 0.8f;
    [SerializeField] private float minViewportY = 0.35f;
    [SerializeField] private float maxViewportY = 0.8f;

    [Header("Debug")]
    [SerializeField] private bool allowKeyboardTest = true;

    private float fixedZPosition;
    private bool isMoving;

    public bool IsMoving => isMoving;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        fixedZPosition = transform.position.z;
    }

    private void Update()
    {
        if (!allowKeyboardTest) return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(MoveToRandomPoint());
        }
    }

    public IEnumerator MoveToRandomPoint()
    {
        if (targetCamera == null) yield break;

        isMoving = true;

        Vector3 targetPosition = GetRandomWorldPointInsideCamera();

        while (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                DifficultyManager.ApplySpeed(moveSpeed) * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;

        isMoving = false;
    }

    private Vector3 GetRandomWorldPointInsideCamera()
    {
        float randomViewportX = Random.Range(minViewportX, maxViewportX);
        float randomViewportY = Random.Range(minViewportY, maxViewportY);

        Vector3 viewportPoint = new Vector3(randomViewportX, randomViewportY, 0f);

        return ViewportPointToWorldPointOnEnemyPlane(viewportPoint);
    }

    private Vector3 ViewportPointToWorldPointOnEnemyPlane(Vector3 viewportPoint)
    {
        Plane enemyPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, fixedZPosition));

        Ray ray = targetCamera.ViewportPointToRay(viewportPoint);

        if (enemyPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            worldPoint.z = fixedZPosition;
            return worldPoint;
        }

        return transform.position;
    }
}
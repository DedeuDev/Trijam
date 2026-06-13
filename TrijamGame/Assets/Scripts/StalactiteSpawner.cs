using System.Collections;
using UnityEngine;

public class StalactiteSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject stalactitePrefab;
    [SerializeField] private Camera targetCamera;

    [Header("Spawn Area")]
    [SerializeField] private float minViewportX = 0.08f;
    [SerializeField] private float maxViewportX = 0.92f;
    [SerializeField] private float spawnHeightOffset = 1.5f;
    [SerializeField] private float fixedZPosition = 0f;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnInterval = 1.2f;
    [SerializeField] private float maxSpawnInterval = 2.4f;
    [SerializeField] private float startDelay = 1f;

    [Header("Settings")]
    [SerializeField] private bool spawnAutomatically = true;

    private Coroutine spawnRoutine;
    private bool isSpawning;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        PlayerDeath.OnPlayerDied += StopSpawning;
    }

    private void OnDisable()
    {
        PlayerDeath.OnPlayerDied -= StopSpawning;
    }

    private void Start()
    {
        if (spawnAutomatically)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (isSpawning) return;

        isSpawning = true;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        isSpawning = false;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (isSpawning)
        {
            SpawnStalactite();

            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);

            yield return new WaitForSeconds(randomInterval);
        }
    }

    private void SpawnStalactite()
    {
        if (stalactitePrefab == null) return;
        if (targetCamera == null) return;

        Vector3 spawnPosition = GetRandomSpawnPosition();

        Instantiate(
            stalactitePrefab,
            spawnPosition,
            stalactitePrefab.transform.rotation
        );
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomViewportX = Random.Range(minViewportX, maxViewportX);

        float topY = GetCameraEdgeY(1f);

        Vector3 spawnPosition = ViewportPointToWorldPointOnGamePlane(
            new Vector3(randomViewportX, 1f, 0f)
        );

        spawnPosition.y = topY + spawnHeightOffset;
        spawnPosition.z = fixedZPosition;

        return spawnPosition;
    }

    private Vector3 ViewportPointToWorldPointOnGamePlane(Vector3 viewportPoint)
    {
        Plane gamePlane = new Plane(Vector3.forward, new Vector3(0f, 0f, fixedZPosition));

        Ray ray = targetCamera.ViewportPointToRay(viewportPoint);

        if (gamePlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }

    private float GetCameraEdgeY(float viewportY)
    {
        Vector3 worldPoint = ViewportPointToWorldPointOnGamePlane(
            new Vector3(0.5f, viewportY, 0f)
        );

        return worldPoint.y;
    }
}
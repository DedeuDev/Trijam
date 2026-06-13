using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBounds : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Horizontal Bounds")]
    [SerializeField] private float horizontalPadding = 0.5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void FixedUpdate()
    {
        ClampPlayerToCameraView();
    }

    private void ClampPlayerToCameraView()
    {
        if (targetCamera == null) return;

        Vector3 position = rb.position;
        Vector3 velocity = GetVelocity();

        float leftX = GetCameraEdgeX(0f);
        float rightX = GetCameraEdgeX(1f);

        float minX = Mathf.Min(leftX, rightX) + horizontalPadding;
        float maxX = Mathf.Max(leftX, rightX) - horizontalPadding;

        if (position.x < minX)
        {
            position.x = minX;

            if (velocity.x < 0f)
            {
                velocity.x = 0f;
            }
        }
        else if (position.x > maxX)
        {
            position.x = maxX;

            if (velocity.x > 0f)
            {
                velocity.x = 0f;
            }
        }

        rb.position = position;
        SetVelocity(velocity);
    }

    private float GetCameraEdgeX(float viewportX)
    {
        Plane playerPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, rb.position.z));

        Ray ray = targetCamera.ViewportPointToRay(new Vector3(viewportX, 0.5f, 0f));

        if (playerPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            return worldPoint.x;
        }

        return rb.position.x;
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
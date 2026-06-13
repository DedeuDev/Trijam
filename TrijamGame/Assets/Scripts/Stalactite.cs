using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Stalactite : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float fallSpeed = 12f;
    [SerializeField] private float maxLifetime = 6f;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask destroyOnHitLayers;

    private Rigidbody rb;
    private bool hasHitSomething;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = false;

        Destroy(gameObject, maxLifetime);
    }

    private void FixedUpdate()
    {
        if (hasHitSomething) return;

        Vector3 velocity = GetVelocity();

        velocity.x = 0f;
        velocity.y = -fallSpeed;
        velocity.z = 0f;

        SetVelocity(velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    private void HandleHit(Collider other)
    {
        if (hasHitSomething) return;

        PlayerDeath playerDeath = other.GetComponentInParent<PlayerDeath>();

        if (playerDeath != null)
        {
            hasHitSomething = true;
            playerDeath.Die();
            Destroy(gameObject);
            return;
        }

        if (ShouldDestroyOnHit(other.gameObject.layer))
        {
            hasHitSomething = true;
            Destroy(gameObject);
        }
    }

    private bool ShouldDestroyOnHit(int objectLayer)
    {
        return (destroyOnHitLayers.value & (1 << objectLayer)) != 0;
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
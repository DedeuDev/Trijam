using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TryKillPlayer(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryKillPlayer(collision.collider);
    }

    private void TryKillPlayer(Collider other)
    {
        PlayerDeath playerDeath = other.GetComponentInParent<PlayerDeath>();

        if (playerDeath == null) return;

        playerDeath.Die();
    }
}
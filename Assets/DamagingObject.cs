using UnityEngine;

public class DamagingObject : MonoBehaviour, IDamaging
{
    [SerializeField] private float bounceForce = 1f;

    public void DamagePlayer(GameObject player)
    {
        // Calculate a random bounce vector
        Vector3 randomBounceVector = new Vector3(Random.Range(-1f, 1f), Random.Range(.7f, 1f), .0f).normalized;

        // Apply the bounce force to the player
        player.GetComponent<DragAndShoot>().HazzardShoot(randomBounceVector * bounceForce);
        // Assumes there's a "TakeDamage" method in the player's script
        player.GetComponent<Health>().TakeDamage();
    }
}
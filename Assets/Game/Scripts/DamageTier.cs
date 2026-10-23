using UnityEngine;

/// <summary>
/// Put on each tier collider (T1-T4). Every tier deals the same damage per second,
/// so an enemy standing inside several overlapping tiers takes stacked damage.
/// The collider must be set to "Is Trigger".
/// </summary>
public class DamageTier : MonoBehaviour
{
    [SerializeField] float damagePerSecond = 10f;

    void OnTriggerStay2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
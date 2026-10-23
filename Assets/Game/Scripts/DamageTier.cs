using UnityEngine;
public class DamageTier : MonoBehaviour
{
    [SerializeField] float damagePerSecond = 10f;

    void OnTriggerStay2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damagePerSecond * Time.deltaTime);
            Debug.Log(name + " touched " + other.name);
        }
    }
}

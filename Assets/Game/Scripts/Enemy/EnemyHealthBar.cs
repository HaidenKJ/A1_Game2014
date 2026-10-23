using UnityEngine;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Enemy enemy; // leave empty to search the parents
    [SerializeField] Transform barSquare; // the square that shrinks on X only

    Vector3 barStartScale;
    int lastShown = -1;

    void Awake()
    {
        if (enemy == null) enemy = GetComponentInParent<Enemy>();
        if (barSquare != null) barStartScale = barSquare.localScale;
    }

    void Update()
    {
        if (enemy == null) return;

        // Only redraw when the displayed number actually changes
        int shown = Mathf.CeilToInt(enemy.Health);
        if (shown == lastShown) return;
        lastShown = shown;

        float fraction = Mathf.Clamp01(enemy.Health / enemy.MaxHealth);

        if (barSquare != null)
        {
            Vector3 s = barStartScale;
            s.x = barStartScale.x * fraction;   // only X changes, Y and Z stay the same
            barSquare.localScale = s;
        }
    }
}